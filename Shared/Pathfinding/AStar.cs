namespace DaLion.Shared.Pathfinding;

#region using directives

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using DaLion.Shared.Extensions.Functional;
using DaLion.Shared.Extensions.Xna;
using Microsoft.Xna.Framework;
using Priority_Queue;

#endregion using directives

/// <summary>Implementation of A* pathfinding algorithm for a 2D grid based on a <see cref="GameLocation"/>.</summary>
public sealed class AStar
{
    private readonly Stack<Point> _path = [];
    private readonly FastPriorityQueue<Node> _open;
    private readonly Func<Point, Point, int> _heuristic = (a, b) => a.ManhattanDistance(b);
    private readonly Func<Point, bool> _isWalkable;
    private readonly Node?[,] _grid;

    /// <summary>Initializes a new instance of the <see cref="AStar"/> class.</summary>
    /// <param name="location">The <see cref="GameLocation"/>.</param>
    /// <param name="isWalkable">A function to determine whether a <see cref="Vector2"/> tile in a given <see cref="GameLocation"/> is walkable.</param>
    public AStar(GameLocation location, Func<GameLocation, Vector2, bool> isWalkable)
    {
        // set cost
        this._isWalkable = p => isWalkable(location, p.ToVector2());

        // populate grid
        var width = location.Map.DisplayWidth;
        var height = location.Map.DisplayHeight;
        this._grid = new Node?[width, height];
        var tile = Vector2.Zero;
        for (var x = 0; x < width; x++)
        {
            for (var y = 0; y < height; y++)
            {
                tile.X = x;
                tile.Y = y;
                if (isWalkable(location, tile))
                {
                    this._grid[y, x] = new Node(x, y);
                }
            }
        }

        this._open = new FastPriorityQueue<Node>(this._grid.Length);
    }



    /// <summary>Attempts to find the shortest path from <paramref name="start"/> to <paramref name="goal"/>.</summary>
    /// <param name="start">The starting node's coordinates, as a <see cref="Point"/>.</param>
    /// <param name="goal">The target node's coordinates, as a <see cref="Point"/>.</param>
    /// <param name="path">The path, if found.</param>
    /// <returns><see langword="true"/> if the path was found, otherwise <see langword="false"/>.</returns>
    public bool ComputeShortestPath(Point start, Point goal, [NotNullWhen(true)] out Stack<Point>? path)
    {
        Node.Heuristic = this._heuristic.Partial<Point, int>(goal);
        this._open.Clear();
        var nStart = this._grid[start.Y, start.X];
        if (nStart is null)
        {
            ThrowHelper.ThrowArgumentException($"Starting coordinates {start} are invalid for the current grid.");
        }

        var nGoal = this._grid[goal.Y, goal.X];
        if (nGoal is null)
        {
            ThrowHelper.ThrowArgumentException($"Goal coordinates {goal} are invalid for the current grid.");
        }

        nStart.G = 0;
        this._open.Enqueue(nStart, nStart.F);
        while (this._open.Count > 0)
        {
            var current = this._open.Dequeue();
            if (current == nGoal)
            {
                path = this.ReconstructPath(nGoal);
                return true;
            }

            foreach (var neighbor in current.GetNeighbors(this._grid))
            {
                var g = current.G + 1;
                if (g >= neighbor.G)
                {
                    continue;
                }

                neighbor.Parent = current;
                neighbor.G = g;
                if (!this._open.Contains(neighbor))
                {
                    this._open.Enqueue(neighbor, g);
                }
                else
                {
                    this._open.UpdatePriority(neighbor, g);
                }
            }
        }

        path = null;
        return false;
    }

    /// <summary>Updates the grid at the specified <paramref name="position"/>.</summary>
    /// <param name="position">The <see cref="Node"/>'s position as a <see cref="Point"/>.</param>
    public void UpdateGrid(Point position)
    {
        var isWalkable = this._isWalkable(position);
        var (x, y) = position;
        this._grid[y, x] = isWalkable ? new Node(x, y) : null;
    }

    /// <summary>Reconstructs the shortest path taken up until the specified <paramref name="target"/>.</summary>
    /// <param name="target">The final <see cref="Node"/> in the path.</param>
    /// <returns>The <see cref="Stack{T}"/> of <see cref="Node"/>s traveled until <paramref name="target"/>.</returns>
    private Stack<Point> ReconstructPath(Node? target)
    {
        this._path.Clear();
        while (target is not null)
        {
            this._path.Push(target.Position);
            target = target.Parent;
        }

        return this._path;
    }

    /// <summary>Represents a node in a path.</summary>
    /// <remarks>Initializes a new instance of the <see cref="Node"/> class.</remarks>
    /// <param name="position">The node's coordinates as a <see cref="Point"/>.</param>
    public sealed class Node(Point position) : FastPriorityQueueNode, IEquatable<Node>
    {

        /// <summary>Initializes a new instance of the <see cref="Node"/> class.</summary>
        /// <param name="x">The node's X coordinate.</param>
        /// <param name="y">The node's Y coordinate.</param>
        public Node(int x, int y)
            : this(new Point(x, y))
        {
        }

        /// <summary>Gets or sets the heuristic function, which takes as input a node's position and returns the cost of a path from that node to the target node.</summary>
        public static Func<Point, int> Heuristic { get; set; } = null!;

        /// <summary>Gets the total cost of a path from the starting node to this node.</summary>
        public Point Position { get; } = position;

        /// <summary>Gets or sets the total cost of a path from the starting node to this node.</summary>
        public int G { get; set; } = int.MaxValue;

        /// <summary>Gets the heuristic cost of a path from this node to the target node.</summary>
        public int H { get; } = Heuristic(position);

        /// <summary>Gets the estimated cost of a path from start to finish going through this node.</summary>
        public int F => this.G + this.H;

        /// <summary>Gets or sets the node immediately preceding this one in the path with cost <see cref="G"/>.</summary>
        public Node? Parent { get; set; }

        /// <summary>Compares whether two <see cref="Node" /> instances are equal.</summary>
        /// <param name="left"><see cref="Node" /> instance on the left of the equal sign.</param>
        /// <param name="right"><see cref="Node" /> instance on the right of the equal sign.</param>
        /// <returns><see langword="true"/> if the instances are equal; <see langword="false"/> otherwise.</returns>
        public static bool operator ==(Node left, Node? right) => left.Equals(right);

        /// <summary>Compares whether two <see cref="Node" /> instances are equal.</summary>
        /// <param name="left"><see cref="Node" /> instance on the left of the equal sign.</param>
        /// <param name="right"><see cref="Node" /> instance on the right of the equal sign.</param>
        /// <returns><see langword="true"/> if the instances are equal; <see langword="false"/> otherwise.</returns>
        public static bool operator !=(Node left, Node? right) => !left.Equals(right);

        /// <summary>Gets the 4-connected neighbors to this node.</summary>
        /// <param name="graph">The graph of walkable <see cref="Node"/>s.</param>
        /// <returns>A <see cref="IEnumerable{T}"/> of the (up to) four neighboring <see cref="Node"/>s.</returns>
        public IEnumerable<Node> GetNeighbors(Node?[,] graph)
        {
            var (x, y) = this.Position;
            if (x > 0 && graph[y, x - 1] is { } left)
            {
                yield return left;
            }

            if (x < graph.GetLength(1) - 1 && graph[y, x + 1] is { } right)
            {
                yield return right;
            }

            if (y > 0 && graph[y - 1, x] is { } top)
            {
                yield return top;
            }

            if (y < graph.GetLength(0) - 1 && graph[y + 1, x] is { } bottom)
            {
                yield return bottom;
            }
        }

        /// <inheritdoc />
        public bool Equals(Node? other)
        {
            return this.Position == other?.Position;
        }

        /// <inheritdoc />
        public override bool Equals(object? @object)
        {
            return this.Equals(@object as Node);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return this.Position.GetHashCode() + (int)(this.F * 1000f);
        }
    }
}
