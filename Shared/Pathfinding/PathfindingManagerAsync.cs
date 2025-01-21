namespace DaLion.Shared.Pathfinding;

#region using directives

using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;
using DaLion.Shared.Events;
using DaLion.Shared.Extensions.Collections;
using DaLion.Shared.Extensions.Xna;
using Microsoft.Xna.Framework;

#endregion using directives

/// <summary>Manages multiple multithreaded pathfinders based on <see cref="MovingTargetDStarLite"/>.</summary>
public sealed class PathfindingManagerAsync : IDisposable
{
    private readonly Thread _thread;
    private readonly CancellationTokenSource _cts;
    private readonly Dictionary<NPC, MovingTargetDStarLite> _pathfinders = [];
    private readonly ConcurrentQueue<(NPC Npc, Point Start, Point Goal)> _requests = [];
    private readonly ConcurrentQueue<(GameLocation Location, Point Edge)> _updates = [];
    private readonly ConcurrentDictionary<NPC, Point?> _cached = [];
    private readonly ConcurrentDictionary<NPC, (Point Start, Point Goal)> _lastRequest = [];
    private readonly Func<GameLocation, Vector2, bool>? _defaultWalkability;
    private readonly AsyncPathfinderObjectListChangedEvent _objectListChangedEvent;

    /// <summary>Initializes a new instance of the <see cref="PathfindingManagerAsync"/> class.</summary>
    /// <param name="eventManager">A <see cref="EventManager"/> instance to capture environment change data.</param>
    /// <param name="defaultWalkability">A default function to determine whether a <see cref="Vector2"/> tile in the <see cref="GameLocation"/> is walkable, to avoid specifying on every step.</param>
    public PathfindingManagerAsync(EventManager eventManager, Func<GameLocation, Vector2, bool>? defaultWalkability = null)
    {
        this._cts = new CancellationTokenSource();
        this._defaultWalkability = defaultWalkability;
        this._objectListChangedEvent = new AsyncPathfinderObjectListChangedEvent(eventManager, this);
        this._thread = new Thread(this.PathfindingLoop)
        {
            IsBackground = true,
            Priority = ThreadPriority.BelowNormal,
        };

        this._thread.Start();
    }

    /// <summary>
    /// Registers a new <see cref="NPC"/> with its own <see cref="MovingTargetDStarLite"/> pathfinder.
    /// Must be called from the main thread (or a thread-safe context) before you can schedule
    /// requests for that NPC.
    /// </summary>
    /// <param name="npc">The <see cref="NPC"/> looking for a path.</param>
    /// <param name="location">The <see cref="GameLocation"/>.</param>
    /// <param name="isWalkable">A function to determine whether a <see cref="Vector2"/> tile in the <paramref name="location"/>> is walkable.</param>
    /// <returns>An instance of <see cref="NpcPathfinder"/> to facilitate the initial request.</returns>
    public NpcPathfinder Register(NPC npc, GameLocation location, Func<GameLocation, Vector2, bool>? isWalkable = null)
    {
        if (isWalkable is null && this._defaultWalkability is null)
        {
            ThrowHelper.ThrowArgumentNullException(
                nameof(isWalkable),
                $"{nameof(isWalkable)} cannot be null if a default walkability condition is not set.");
        }

        lock (this._pathfinders)
        {
            if (!this._pathfinders.ContainsKey(npc))
            {
                this._pathfinders[npc] = new MovingTargetDStarLite(location, (isWalkable ?? this._defaultWalkability)!);
            }
        }

        this._objectListChangedEvent.Enable();
        return new NpcPathfinder(this, npc);
    }

    /// <summary>
    /// Unregisters an NPC that no longer needs pathfinding (e.g., died or despawned).
    /// </summary>
    /// <param name="npc">The <see cref="NPC"/> who no longer needs a path.</param>
    /// <returns>The <see cref="PathfindingManager"/> instance.</returns>
    public PathfindingManagerAsync Unregister(NPC npc)
    {
        lock (this._pathfinders)
        {
            this._pathfinders.Remove(npc);
            if (!this._pathfinders.Any())
            {
                this._objectListChangedEvent.Disable();
            }
        }

        this._cached.TryRemove(npc, out _);
        return this;
    }

    /// <summary>
    /// Discards the old <see cref="MovingTargetDStarLite"/> instance for the <paramref name="npc"/>
    /// and registers a new one. Useful when the <paramref name="npc"/>'s <see cref="GameLocation"/>
    /// is changed.
    /// </summary>
    /// <param name="npc">The <see cref="NPC"/> who seeks a new path.</param>
    /// <param name="location">The new <see cref="GameLocation"/>.</param>
    /// <returns>An instance of <see cref="NpcPathfinder"/> to facilitate the initial request.</returns>
    public NpcPathfinder Reregister(NPC npc, GameLocation location)
    {
        return this.Unregister(npc).Register(npc, location);
    }

    /// <summary>Public method the main thread can call to request a new path step.</summary>
    /// <param name="npc">The <see cref="NPC"/> looking for a path.</param>
    /// <param name="start">The starting position, as a <see cref="Point"/>.</param>
    /// <param name="goal">The goal position, as a <see cref="Point"/>.</param>
    /// <returns>The <see cref="PathfindingManager"/> instance.</returns>
    public PathfindingManagerAsync QueueRequest(NPC npc, Point start, Point goal)
    {
        lock (this._pathfinders)
        {
            if (!this._pathfinders.ContainsKey(npc))
            {
                ThrowHelper.ThrowInvalidOperationException($"The requested NPC {npc.Name} is not registered.");
            }
        }

        this._requests.Enqueue((npc, start, goal));
        return this;
    }

    /// <summary>Public method the main thread can call to communicate environment changes to active pathfinders.</summary>
    /// <param name="location">The <see cref="GameLocation"/> where the change occurred.</param>
    /// <param name="edge">The tile position which was changed, as a <see cref="Point"/>.</param>
    /// <returns>The <see cref="PathfindingManager"/> instance.</returns>
    public PathfindingManagerAsync QueueUpdate(GameLocation location, Point edge)
    {
        this._updates.Enqueue((location, edge));
        return this;
    }

    /// <summary>Public method the main thread can call to receive the actual next step result.</summary>
    /// <param name="npc">The <see cref="NPC"/> looking for a path.</param>
    /// <returns>The next step as a <see cref="Point"/> if one is available, or <see langword="null"/> otherwise.</returns>
    public Point? QueryStep(NPC npc)
    {
        return this._cached.GetValueOrDefault(npc);
    }

    /// <summary>Gracefully shuts down the background thread.</summary>
    public void Dispose()
    {
        this._cts.Cancel();
        this._thread.Join();
        this._cts.Dispose();
    }

    /// <summary>Enables debug rendering for the specified <paramref name="npc"/>.</summary>
    /// <param name="npc">The <see cref="NPC"/> who needs debugging.</param>
    [Conditional("DEBUG")]
    public void Debug(NPC npc)
    {
        lock (this._pathfinders)
        {
            if (this._pathfinders.TryGetValue(npc, out var pathfinder))
            {
                PathfinderOverlayRenderedWorldEvent.Pathfinder = pathfinder;
            }
        }
    }

    /// <summary>Background thread method that processes requests and updates pathfinders.</summary>
    private void PathfindingLoop()
    {
        while (!this._cts.IsCancellationRequested)
        {
            try
            {
                while (this._updates.TryDequeue(out var update))
                {
                    var (location, edge) = update;
                    lock (this._pathfinders)
                    {
                        foreach (var pathfinder in this._pathfinders.Values)
                        {
                            pathfinder.UpdateEdges(location, edge);
                        }
                    }
                }

                while (this._requests.TryDequeue(out var req))
                {
                    var (npc, start, goal) = req;
                    if (this._lastRequest.TryGetValue(npc, out var last) && last == (start, goal))
                    {
                        continue;
                    }

                    this._lastRequest[npc] = (start, goal);
                    if (this._cached.TryGetValue(npc, out var step) && step is not null && step != start)
                    {
                        continue;
                    }

                    var width = npc.currentLocation.Map.Layers[0].LayerWidth;
                    var height = npc.currentLocation.Map.Layers[0].LayerHeight;
                    lock (this._pathfinders)
                    {
                        this._pathfinders.TryGetValue(npc, out var pathfinder);
                        if (pathfinder is null)
                        {
                            continue;
                        }

                        var target = goal
                            .GetTwentyFourNeighbors(width, height)
                            .Where(t => pathfinder.IsWalkable(t))
                            .Choose();
                        if (target == default)
                        {
                            continue;
                        }

                        step = pathfinder.Step(start, target);
                        if (step is null)
                        {
                            continue;
                        }
                    }

                    this._cached[npc] = step;
                }

                Thread.Sleep(10);
            }
            catch (Exception ex)
            {
                Log.E("Error in pathfinding loop:\n" + ex);
            }
        }
    }

    /// <summary>Facilitates registration.</summary>
    /// <param name="manager">The <see cref="PathfindingManager"/>.</param>
    /// <param name="npc">The tracked <see cref="NPC"/>.</param>
    public class NpcPathfinder(PathfindingManagerAsync manager, NPC npc)
    {
        /// <summary>A wrapper for <seealso cref="PathfindingManagerAsync.QueueRequest"/>.</summary>
        /// <param name="start">The starting position, as a <see cref="Point"/>.</param>
        /// <param name="goal">The goal position, as a <see cref="Point"/>.</param>
        /// <returns>The <see cref="PathfindingManagerAsync"/> instance.</returns>
        public PathfindingManagerAsync QueueRequest(Point start, Point goal)
        {
            return manager.QueueRequest(npc, start, goal);
        }
    }
}
