namespace DaLion.Shared.Pathfinding;

#region using directives

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using DaLion.Shared.Extensions;
using DaLion.Shared.Extensions.Collections;
using DaLion.Shared.Extensions.Functional;
using DaLion.Shared.Extensions.Xna;
using Microsoft.Xna.Framework;
using Priority_Queue;

#endregion using directives

/// <summary>Implementation of D* Lite pathfinding algorithm for a 2D grid based on a <see cref="GameLocation"/>.</summary>
public sealed class DStarLite
{
    private const int MAX_COMPUTE_CYCLES = 1000;

    private readonly SimplePriorityQueue<Node, Key> _open = [];
    private readonly Func<Point, Point, int> _cost;
    private readonly Func<Point, Point, int> _heuristic = (a, b) => a.ManhattanDistance(b);
    private readonly Func<Point, bool> _isWalkable;
    private readonly Node[,] _grid;
    private Node _start = null!; // set during initialization
    private Node _goal = null!; // set during initialization
    private int _km;
    private int _steps;

    /// <summary>Initializes a new instance of the <see cref="DStarLite"/> class.</summary>
    /// <param name="location">The <see cref="GameLocation"/>.</param>
    /// <param name="isWalkable">A function to determine whether a <see cref="Vector2"/> tile in a given <see cref="GameLocation"/> is walkable.</param>
    public DStarLite(GameLocation location, Func<GameLocation, Vector2, bool> isWalkable)
    {
        // set cost
        this._isWalkable = p => isWalkable(location, p.ToVector2());
        this._cost = (a, b) => this._isWalkable(b) ? 1 : int.MaxValue;

        // populate grid
        var width = location.Map.DisplayWidth;
        var height = location.Map.DisplayHeight;
        this._grid = new Node[width, height];
        for (var x = 0; x < width; x++)
        {
            for (var y = 0; y < height; y++)
            {
                this._grid[y, x] = new Node(x, y);
            }
        }
    }

    /// <summary>Computes the next position in the optimal path from <paramref name="startPosition"/> to <paramref name="goalPosition"/>.</summary>
    /// <param name="startPosition">The starting position, as a <see cref="Point"/>.</param>
    /// <param name="goalPosition">The goal position, as a <see cref="Point"/>.</param>
    /// <returns>The <see cref="Point"/> next position in the optimal path from <paramref name="startPosition"/> to <paramref name="goalPosition"/>, or <see langword="null"/> if no path was found.</returns>
    public Point? Step(Point startPosition, Point goalPosition)
    {
        if (this._steps == 0)
        {
            this.Initialize(startPosition, goalPosition);
            if (!this.ComputeShortestPath(out var firstStep))
            {
                return null;
            }

            this._start = firstStep;
            this._steps++;
            return firstStep.Position;
        }

        if (this._start == this._goal)
        {
            return null; // we have reached the goal
        }

        if (this._start.Position != startPosition)
        {
            var (x, y) = startPosition;
            this._start = this._grid[y, x];
            this.UpdateVertex(this._start);
        }

        if (this._goal.Position != goalPosition)
        {
            var (x, y) = goalPosition;
            this._goal = this._grid[y, x];
            this.UpdateVertex(this._goal);
        }

        if (!this.ComputeShortestPath(out var nextStep))
        {
            return null;
        }

        this._start = nextStep;
        this._steps++;
        return nextStep.Position;
    }

    /// <summary>Updates the edges which connect to the <see cref="Node"/> at the specified <paramref name="position"/>.</summary>
    /// <param name="position">The <see cref="Node"/>'s position as a <see cref="Point"/>.</param>
    public void UpdateEdges(Point position)
    {
        this._km += 1;
        var isWalkable = this._isWalkable(position);
        var cOld = isWalkable ? int.MaxValue : 1;
        var cNew = cOld == 1 ? int.MaxValue : 1;
        var node = this._grid[position.Y, position.X];
        foreach (var (u, v) in node.GetNeighbors(this._grid).Select(u => (u, node)))
        {
            if (u != this._goal)
            {
                if (cOld > cNew)
                {
                    u.RHS = Math.Min(u.RHS, cNew + v.G);
                }
                else if (u.RHS == cOld + v.G)
                {
                    u.RHS = int.MaxValue;
                    foreach (var sPrime in u.Successors(this._grid))
                    {
                        u.RHS = Math.Min(u.RHS, u.C(sPrime) + sPrime.G);
                    }
                }
            }

            this.UpdateVertex(u);
        }
    }

    /// <summary>Initializes the search from <paramref name="start"/> to <paramref name="goal"/>.</summary>
    /// <param name="start">The starting position, as a <see cref="Point"/>.</param>
    /// <param name="goal">The goal position, as a <see cref="Point"/>.</param>
    [MemberNotNull(nameof(_start), nameof(_goal))]
    public void Initialize(Point start, Point goal)
    {
        Node.Cost = (p, n) => this._cost(p, n.Position);
        Node.Heuristic = this._heuristic.Partial<Point, int>(start);
        this._open.Clear();
        this._km = 0;
        if (this._grid[start.Y, start.X] is not { } nStart)
        {
            ThrowHelper.ThrowArgumentException($"Starting coordinates {start} are invalid for the current grid.");
            return;
        }

        if (this._grid[goal.Y, goal.X] is not { } nGoal)
        {
            ThrowHelper.ThrowArgumentException($"Goal coordinates {goal} are invalid for the current grid.");
            return;
        }

        this._start = nStart;
        this._goal = nGoal;
        this._goal.RHS = 0;
        this._open.Enqueue(this._goal, this._goal.CalculateKey());
    }

    /// <summary>Computes the shortest path between the initialized start and goal nodes.</summary>
    /// <param name="best">The best candidate node to follow in the shortest path towards the goal.</param>
    /// <returns><see langword="true"/> if a path was found, otherwise <see langword="false"/>.</returns>
    private bool ComputeShortestPath([NotNullWhen(true)] out Node? best)
    {
        var maxSteps = MAX_COMPUTE_CYCLES;
        while (this._open.Count > 0 &&
               (this._open.First.Priority < this._start.CalculateKey(this._km) || this._start.RHS > this._start.G))
        {
            if (maxSteps-- <= 0)
            {
                // path computation took too long
                best = null;
                return false;
            }

            var u = this._open.First();
            var kOld = u.Priority;
            var kNew = u.CalculateKey(this._km);
            if (kOld < kNew)
            {
                this._open.UpdatePriority(u, kNew);
            }
            else if (u.G > u.RHS)
            {
                u.G = u.RHS;
                this._open.Dequeue();
                foreach (var s in u.Predecessors(this._grid))
                {
                    if (s != this._goal)
                    {
                        s.RHS = Math.Min(s.RHS, s.C(u) + u.G);
                    }

                    this.UpdateVertex(s);
                }
            }
            else
            {
                var gOld = u.G;
                u.G = int.MaxValue;
                foreach (var s in u.Collect(u.Predecessors(this._grid)))
                {
                    if (s.RHS == s.C(u) + gOld)
                    {
                        if (s != this._goal)
                        {
                            s.RHS = int.MaxValue;
                            foreach (var sPrime in s.Successors(this._grid))
                            {
                                s.RHS = Math.Min(s.RHS, s.C(sPrime) + sPrime.G);
                            }
                        }
                    }

                    this.UpdateVertex(s);
                }
            }
        }

        if (this._start.RHS == int.MaxValue)
        {
            // there is no known path
            best = null;
            return false;
        }

        best = this._start.Successors(this._grid).ArgMin(sPrime => this._start.C(sPrime) + sPrime.G);
        return best is not null;
    }

    /// <summary>Updates the specified <paramref name="node"/>'s priority in the queue if necessary.</summary>
    /// <param name="node">The <see cref="Node"/>.</param>
    private void UpdateVertex(Node node)
    {
        var key = node.CalculateKey(this._km);
        if (node.G != node.RHS)
        {
            if (this._open.Contains(node))
            {
                this._open.UpdatePriority(node, key);
            }
            else
            {
                this._open.Enqueue(node, key);
            }
        }
        else if (this._open.Contains(node))
        {
            this._open.Remove(node);
        }
    }

    /// <summary>Represents a two-value priority key.</summary>
    /// <param name="k1">The first key value.</param>
    /// <param name="k2">The second key value.</param>
    public readonly struct Key(float k1, float k2)
    {
        /// <summary>The first key value.</summary>
        public readonly float K1 = k1;

        /// <summary>The second key value.</summary>
        public readonly float K2 = k2;

        /// <summary>Compares whether the <see cref="left"/> <see cref="Key"/> instance is less than the <paramref name="right"/> <see cref="Key"/> instance.</summary>
        /// <param name="left"><see cref="Key"/> instance on the left of the less-than sign.</param>
        /// <param name="right"><see cref="Key"/> instance on the right of the less-than sign.</param>
        /// <returns><see langword="true"/> if the <paramref name="left"/> instance is less than the <paramref name="right"/>; <see langword="false"/> otherwise.</returns>
        public static bool operator <(Key left, Key right) => left.K1 < right.K1 || (left.K1.Approx(right.K1) && left.K2 < right.K2);

        /// <summary>Compares whether the <see cref="left"/> <see cref="Key"/> instance is greater than the <paramref name="right"/> <see cref="Key"/> instance.</summary>
        /// <param name="left"><see cref="Key"/> instance on the left of the greater-than sign.</param>
        /// <param name="right"><see cref="Key"/> instance on the right of the greater-than sign.</param>
        /// <returns><see langword="true"/> if the <paramref name="left"/> instance is greater than the <paramref name="right"/>; <see langword="false"/> otherwise.</returns>
        public static bool operator >(Key left, Key right) => left.K1 > right.K1 || (left.K1.Approx(right.K1) && left.K2 > right.K2);

        /// <summary>Compares whether two <see cref="Key"/> instances are equal.</summary>
        /// <param name="left"><see cref="Key"/> instance on the left of the equal sign.</param>
        /// <param name="right"><see cref="Key"/> instance on the right of the equal sign.</param>
        /// <returns><see langword="true"/> if the instances are equal; <see langword="false"/> otherwise.</returns>
        public static bool operator ==(Key left, Key right) => left.K1.Approx(right.K1) && left.K2.Approx(right.K2);

        /// <summary>Compares whether two <see cref="Key"/> instances are not equal.</summary>
        /// <param name="left"><see cref="Key"/> instance on the left of the not equal sign.</param>
        /// <param name="right"><see cref="Key"/> instance on the right of the not equal sign.</param>
        /// <returns><see langword="true"/> if the instances are not equal; <see langword="false"/> otherwise.</returns>
        public static bool operator !=(Key left, Key right) => !(left == right);

        /// <inheritdoc />
        public override bool Equals(object? @object)
        {
            return @object is Key key && this == key;
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return HashCode.Combine(this.K1, this.K2);
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{this.K1}, {this.K2}";
        }
    }

    /// <summary>Represents a node in a path.</summary>
    /// <remarks>Initializes a new instance of the <see cref="Node"/> class.</remarks>
    /// <param name="position">The node's coordinates as a <see cref="Point"/>.</param>
    public sealed class Node(Point position) : GenericPriorityQueueNode<Key>, IEquatable<Node>
    {
        /// <summary>Initializes a new instance of the <see cref="Node"/> class.</summary>
        /// <param name="x">The node's X coordinate.</param>
        /// <param name="y">The node's Y coordinate.</param>
        public Node(int x, int y)
            : this(new Point(x, y))
        {
        }

        /// <summary>Gets or sets the cost function, which takes as input two node positions and returns the cost of a path between them.</summary>
        public static Func<Point, Node, int> Cost { get; set; } = null!;

        /// <summary>Gets or sets the heuristic function, which takes as input a node's position and returns the cost of a path from that node to the target node.</summary>
        public static Func<Point, int> Heuristic { get; set; } = null!;

        /// <summary>Gets the total cost of a path from the starting node to this node.</summary>
        public Point Position { get; } = position;

        /// <summary>Gets the cost function for traveling from this node to some other node.</summary>
        public Func<Node, int> C { get; } = Cost.Partial(position);

        /// <summary>Gets the heuristic cost of a path from this node to the starting node.</summary>
        public int H { get; } = Heuristic(position);

        /// <summary>Gets or sets the total cost of a path from the goal node to this node.</summary>
        public int G { get; set; } = int.MaxValue;

        /// <summary>Gets the right-hand side value, which estimates this node's <see cref="G"/> based on that of its neighbors.</summary>
        public int RHS { get; set; } = int.MaxValue;

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

        /// <summary>Calculates the key, which estimates the cost of a path from start to finish going through this node.</summary>
        /// <param name="km">The key modifier.</param>
        /// <returns>The <see cref="Key"/>.</returns>
        public Key CalculateKey(int km = 0)
        {
            return new Key(
                Math.Min(this.G, this.RHS) + this.H + km,
                Math.Min(this.G, this.RHS));
        }

        /// <summary>Gets the 4-connected neighbors to this node.</summary>
        /// <param name="grid">The grid of walkable <see cref="Node"/>s.</param>
        /// <returns>A <see cref="IEnumerable{T}"/> of the (up to) four neighboring <see cref="Node"/>s.</returns>
        public IEnumerable<Node> GetNeighbors(Node[,] grid)
        {
            var (x, y) = this.Position;
            if (x > 0)
            {
                yield return grid[y, x - 1];
            }

            if (x < grid.GetLength(1) - 1 && grid[y, x + 1] is { } right)
            {
                yield return right;
            }

            if (y > 0 && grid[y - 1, x] is { } top)
            {
                yield return top;
            }

            if (y < grid.GetLength(0) - 1 && grid[y + 1, x] is { } bottom)
            {
                yield return bottom;
            }
        }

        /// <summary>Gets the 4-connected neighbors which connect to this node.</summary>
        /// <param name="grid">The grid of walkable <see cref="Node"/>s.</param>
        /// <returns>A <see cref="IEnumerable{T}"/> of the (up to) four neighboring <see cref="Node"/>s.</returns>
        public IEnumerable<Node> Predecessors(Node[,] grid)
        {
            return this.GetNeighbors(grid);
        }

        /// <summary>Gets the 4-connected neighbors this node is connected to.</summary>
        /// <param name="grid">The grid of walkable <see cref="Node"/>s.</param>
        /// <returns>A <see cref="IEnumerable{T}"/> of the (up to) four neighboring <see cref="Node"/>s.</returns>
        public IEnumerable<Node> Successors(Node[,] grid)
        {
            return this.GetNeighbors(grid);
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
            return this.Position.GetHashCode();
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return this.Position.ToString();
        }
    }
}
