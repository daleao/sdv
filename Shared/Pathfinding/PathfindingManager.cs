namespace DaLion.Shared.Pathfinding;

#region using directives

using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using DaLion.Shared.Events;
using DaLion.Shared.Extensions.Collections;
using DaLion.Shared.Extensions.Xna;
using Microsoft.Xna.Framework;

#endregion using directives

/// <summary>Manages multiple multithreaded pathfinders based on <see cref="MovingTargetDStarLite"/>.</summary>
public sealed class PathfindingManager
{
    private readonly ConditionalWeakTable<NPC, NpcPathfinder> _pathfinders = [];
    private readonly Func<GameLocation, Vector2, bool>? _defaultWalkability;
    private readonly PathfinderObjectListChangedEvent _objectListChangedEvent;

    /// <summary>Initializes a new instance of the <see cref="PathfindingManager"/> class.</summary>
    /// <param name="eventManager">A <see cref="EventManager"/> instance to capture environment change data.</param>
    /// <param name="defaultWalkability">A default function to determine whether a <see cref="Vector2"/> tile in the <see cref="GameLocation"/> is walkable, to avoid specifying on every step.</param>
    public PathfindingManager(EventManager eventManager, Func<GameLocation, Vector2, bool>? defaultWalkability = null)
    {
        this._defaultWalkability = defaultWalkability;
        this._objectListChangedEvent = new PathfinderObjectListChangedEvent(eventManager, this);
    }

    /// <summary>
    /// Registers a new <see cref="NPC"/> with its own <see cref="MovingTargetDStarLite"/> pathfinder.
    /// Must be called from the main thread (or a thread-safe context) before you can schedule
    /// requests for that NPC.
    /// </summary>
    /// <param name="npc">The <see cref="NPC"/> looking for a path.</param>
    /// <param name="location">The <see cref="GameLocation"/>.</param>
    /// <param name="isWalkable">A function to determine whether a <see cref="Vector2"/> tile in the <paramref name="location"/>> is walkable.</param>
    /// <returns>The registered <see cref="NpcPathfinder"/> instance.</returns>
    public NpcPathfinder Register(NPC npc, GameLocation location, Func<GameLocation, Vector2, bool>? isWalkable = null)
    {
        if (isWalkable is null && this._defaultWalkability is null)
        {
            ThrowHelper.ThrowArgumentNullException(
                nameof(isWalkable),
                $"{nameof(isWalkable)} cannot be null if a default walkability condition is not set.");
        }

        var pathfinder = new NpcPathfinder(this, npc, (isWalkable ?? this._defaultWalkability)!);
        this._pathfinders.AddOrUpdate(npc, pathfinder);
        this._objectListChangedEvent.Enable();
        return pathfinder;
    }

    /// <summary>
    /// Unregisters an NPC that no longer needs pathfinding (e.g., died or despawned).
    /// </summary>
    /// <param name="npc">The <see cref="NPC"/> who no longer needs a path.</param>
    public void Unregister(NPC npc)
    {
        this._pathfinders.Remove(npc);
        if (!this._pathfinders.Any())
        {
            this._objectListChangedEvent.Disable();
        }
    }

    /// <summary>
    /// Discards the old <see cref="MovingTargetDStarLite"/> instance for the <paramref name="npc"/>
    /// and registers a new one. Useful when the <paramref name="npc"/>'s <see cref="GameLocation"/>
    /// is changed.
    /// </summary>
    /// <param name="npc">The <see cref="NPC"/> who seeks a new path.</param>
    /// <param name="location">The new <see cref="GameLocation"/>.</param>
    /// <returns>The registered <see cref="NpcPathfinder"/> instance.</returns>
    public NpcPathfinder Reregister(NPC npc, GameLocation location)
    {
        this.Unregister(npc);
        return this.Register(npc, location);
    }

    /// <summary>Public method to request the next step in the path from <paramref name="start"/> to <paramref name="goal"/>.</summary>
    /// <param name="npc">The <see cref="NPC"/> looking for a path.</param>
    /// <param name="start">The starting position, as a <see cref="Point"/>.</param>
    /// <param name="goal">The goal position, as a <see cref="Point"/>.</param>
    /// <returns>The next step as a <see cref="Point"/>, or <see langword="null"/> is none was found.</returns>
    public Point? RequestFor(NPC npc, Point start, Point goal)
    {
        if (!this._pathfinders.TryGetValue(npc, out var pathfinder))
        {
            if (this._defaultWalkability is not null)
            {
                pathfinder = this._pathfinders.GetValue(
                    npc,
                    (c) => new NpcPathfinder(this, c, this._defaultWalkability));
            }
            else
            {
                return ThrowHelper.ThrowInvalidOperationException<Point?>(
                    $"The requested NPC {npc.Name} is not registered and could not be created.");
            }
        }

        var step = pathfinder.CachedStep;
        if (step is not null && step != start)
        {
            return step;
        }

        var width = npc.currentLocation.Map.Layers[0].LayerWidth;
        var height = npc.currentLocation.Map.Layers[0].LayerHeight;
        var target = goal
            .GetTwentyFourNeighbors(width, height)
            .Where(t => pathfinder.Pathfinder.IsWalkable(t))
            .Choose();
        step = pathfinder.Pathfinder.Step(start, target);
        if (step is null)
        {
            return null;
        }

        pathfinder.CachedStep = step;
        return step;
    }

    /// <summary>Public method for communicating to pathfinders about changes to the environment.</summary>
    /// <param name="location">The <see cref="GameLocation"/> where the change occurred.</param>
    /// <param name="tile">The tile which was changed, as a <see cref="Point"/>.</param>
    public void Update(GameLocation location, Point tile)
    {
        foreach (var pathfinder in this._pathfinders.Select(pair => pair.Value.Pathfinder))
        {
            pathfinder.UpdateEdges(location, tile);
        }
    }

    /// <summary>Enables debug rendering for the specified <paramref name="npc"/>.</summary>
    /// <param name="npc">The <see cref="NPC"/> who needs debugging.</param>
    [Conditional("DEBUG")]
    public void Debug(NPC npc)
    {
        if (this._pathfinders.TryGetValue(npc, out var holder))
        {
            PathfinderOverlayRenderedWorldEvent.Pathfinder = holder.Pathfinder;
        }
    }

    /// <summary>Facilitates registration and holds boxed value types for <see cref="ConditionalWeakTable{TKey,TValue}"/>.</summary>
    /// <param name="manager">The <see cref="PathfindingManager"/>.</param>
    /// <param name="npc">The tracked <see cref="NPC"/>.</param>
    /// <param name="walkability">A function to determine whether a <see cref="Vector2"/> tile in the <see cref="GameLocation"/> is walkable.</param>
    public class NpcPathfinder(PathfindingManager manager, NPC npc, Func<GameLocation, Vector2, bool> walkability)
    {
        /// <summary>Gets the <see cref="MovingTargetDStarLite"/> instance.</summary>
        public MovingTargetDStarLite Pathfinder { get; } = new(npc.currentLocation, walkability);

        /// <summary>Gets or sets the last recorded request.</summary>
        public (Point Start, Point Goal) LastRequest { get; set; }

        /// <summary>Gets the latest cached step.</summary>
        public Point? CachedStep { get; internal set; }

        /// <summary>A wrapper for <seealso cref="PathfindingManager.RequestFor"/>.</summary>
        /// <param name="start">The starting position, as a <see cref="Point"/>.</param>
        /// <param name="goal">The goal position, as a <see cref="Point"/>.</param>
        /// <returns>The next step as a <see cref="Point"/>, or <see langword="null"/> is none was found.</returns>
        public Point? RequestFor(Point start, Point goal)
        {
            return manager.RequestFor(npc, start, goal);
        }
    }
}
