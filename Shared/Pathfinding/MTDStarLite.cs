namespace DaLion.Shared.Pathfinding;

#region using directives

using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using DaLion.Shared.Extensions;
using DaLion.Shared.Extensions.Collections;
using DaLion.Shared.Extensions.Functional;
using DaLion.Shared.Extensions.Xna;
using Microsoft.Xna.Framework;
using Priority_Queue;

#endregion using directives

/// <summary>Implementation of Moving Target D* Lite pathfinding algorithm for a 2D grid based on a <see cref="GameLocation"/>.</summary>
/// <remarks>Reference paper: <see href="http://idm-lab.org/bib/abstracts/papers/aamas10a.pdf"/>.</remarks>
public sealed class MTDStarLite
{
    private const int MAX_COMPUTE_CYCLES = 1000;

    private readonly GameLocation _location;
    private readonly Func<Point, Point, int> _cost;
    private readonly Func<Point, Point, int> _heuristic = (a, b) => a.ManhattanDistance(b);
    private readonly Func<Point, bool> _isWalkable;
    private readonly SimplePriorityQueue<State, Key> _open = [];
    private readonly HashSet<State> _closed = [];
    private readonly HashSet<State> _deleted = [];
    private readonly Stack<Point> _path = [];
    private State[,] _grid;
    private State? _start;
    private State? _goal;
    private Point? _previousStep;
    private int _km;
    private bool _initialized;
    private bool _anyEdgesChanged;

    /// <summary>Initializes a new instance of the <see cref="MTDStarLite"/> class.</summary>
    /// <param name="location">The <see cref="GameLocation"/>.</param>
    /// <param name="isWalkable">A function to determine whether a <see cref="Vector2"/> tile in the <paramref name="location"/>> is walkable.</param>
    public MTDStarLite(GameLocation location, Func<GameLocation, Vector2, bool> isWalkable)
    {
        this._location = location;
        this._isWalkable = p => isWalkable.Partial(location)(p.ToVector2());
        this._cost = (a, b) => this._isWalkable(a) && this._isWalkable(b) ? 1 : int.MaxValue;
        State.Cost = (p, n) => this._cost(p, n.Position);

        var width = location.Map.Layers[0].LayerWidth;
        var height = location.Map.Layers[0].LayerHeight;
        this._grid = new State[height, width];
        for (var x = 0; x < width; x++)
        {
            for (var y = 0; y < height; y++)
            {
                this._grid[y, x] = new State(x, y);
            }
        }

        PathfinderObjectListChangedEvent.Pathfinders.AddOrUpdate(this, null);
#if DEBUG
        PathfinderRenderedWorldEvent.Pathfinder = this;
#endif
    }

    /// <summary>Gets the current start state.</summary>
    [NotNullIfNotNull(nameof(_start))]
    public State? Start
    {
        get => this._start;
        private set
        {
            if (this._start is not null)
            {
                this._start.IsStart = false;
            }

            if (value is null)
            {
                this._start = null;
                return;
            }

            this._start = value;
            this._start.IsStart = true;
        }
    }

    /// <summary>Gets the current goal state.</summary>
    [NotNullIfNotNull(nameof(_goal))]
    public State? Goal
    {
        get => this._goal;
        private set
        {
            if (this._goal is not null)
            {
                this._goal.IsGoal = false;
            }

            if (value is null)
            {
                this._goal = null;
                return;
            }

            this._goal = value;
            this._goal.IsGoal = true;
            State.Heuristic = this._heuristic.Partial<Point, int>(this._goal.Position);
        }
    }

#if DEBUG
    /// <summary>Gets the set of unchecked grid positions.</summary>
    public IReadOnlySet<State> OpenSet => this._open.ToHashSet();

    /// <summary>Gets the set of checked grid positions.</summary>
    public IReadOnlySet<State> ClosedSet => this._closed.ToHashSet();

    /// <summary>Gets the set of positions in the pre-computed optimal path.</summary>
    public IReadOnlySet<Point> PathSet => this._path.ToHashSet();

    /// <summary>Enumerates the set of all states.</summary>
    public IEnumerable<State> FullSet => this._grid.Cast<State>();
#endif

    /// <summary>Gets the next step in the optimal path from <paramref name="start"/> to <paramref name="goal"/>.</summary>
    /// <param name="start">The starting position, as a <see cref="Point"/>.</param>
    /// <param name="goal">The goal position, as a <see cref="Point"/>.</param>
    /// <returns>The <see cref="Point"/> next position in the optimal path from <paramref name="start"/> to <paramref name="goal"/>, or <see langword="null"/> if no path was found.</returns>
    public Point? Step(Point start, Point goal)
    {
        if (goal == Point.Zero || start == goal)
        {
            return null;
        }

        Point step;
        if (!this._initialized || this.Start is null || this.Goal is null)
        {
            this.Initialize(start, goal);
        }
        else if ((start == this._previousStep || this._path.Contains(start)) && this._path.Contains(goal))
        {
            if (this._anyEdgesChanged)
            {
                if (!this.ComputeCostMinimalPath())
                {
                    return null;
                }

                this._anyEdgesChanged = false;
                step = this.ReconstructPath().Pop();
            }
            else
            {
                step = this._path.Pop();
            }

            this._previousStep = step;
            return step;
        }
        else
        {
            var oldStart = this.Start;
            var oldGoal = this.Goal;
            this.Start = this._grid[start.Y, start.X];
            this.Goal = this._grid[goal.Y, goal.X];
            this._km += this._heuristic(goal, oldGoal.Position);
            if (oldStart != this.Start)
            {
                this.Start.Parent = null;
                //this.BasicDeletion(oldStart);
                this.OptimizedDeletion(oldStart);
            }
        }

        if (!this.ComputeCostMinimalPath())
        {
            return null;
        }

        step = this.ReconstructPath().Pop();
        this._previousStep = step;
        return step;
    }

    /// <summary>Updates the edges which connect to the <see cref="State"/> at the specified <paramref name="position"/>.</summary>
    /// <param name="position">The <see cref="State"/>'s position as a <see cref="Point"/>.</param>
    public void UpdateEdges(GameLocation location, Point position)
    {
        if (!ReferenceEquals(location, this._location))
        {
            return;
        }

        var state = this._grid[position.Y, position.X];
        var neighbors = state.GetNeighbors(this._grid).ToList();
        int cOld, cNew;
        foreach (var (u, v) in neighbors.Select(v => (state, v)).Concat(neighbors.Select(u => (u, state))))
        {
            cNew = u.C(v);
            cOld = cNew == 1 ? int.MaxValue : 1;
            if (v != this.Start)
            {
                if (cOld > cNew && v.RHS > u + v)
                {
                    v.Parent = u;
                    v.RHS = u + v;
                    this.UpdateState(v);
                }
                else if (v.Parent == u)
                {
                    v.RHS = int.MaxValue;
                    foreach (var sPrime in v.Predecessors(this._grid))
                    {
                        v.RHS = Math.Min(v.RHS, sPrime + v);
                    }

                    v.Parent = v.RHS == int.MaxValue ? null : v.Predecessors(this._grid).ArgMin(sPrime => sPrime + v);
                    this.UpdateState(v);
                }
            }

            this.UpdateState(u);
        }

        this._anyEdgesChanged = true;
    }

    /// <summary>Initializes the search from <paramref name="start"/> to <paramref name="goal"/>.</summary>
    /// <param name="start">The starting position, as a <see cref="Point"/>.</param>
    /// <param name="goal">The goal position, as a <see cref="Point"/>.</param>
    [MemberNotNull(nameof(Start), nameof(Goal))]
    private void Initialize(Point start, Point goal)
    {
        this._open.Clear();
        this._closed.Clear();
        this._km = 0;
        foreach (var s in this._grid)
        {
            s.RHS = s.G = int.MaxValue;
            s.Parent = null;
            s.Children.Clear();
        }

        this.Start = this._grid[start.Y, start.X];
        this.Goal = this._grid[goal.Y, goal.X];
        this.Start.RHS = 0;
        this._open.Enqueue(this.Start, this.Start.CalculateKey());
        this._initialized = true;
    }

    /// <summary>Computes the shortest path between the initialized start and goal states.</summary>
    /// <returns><see langword="true"/> if a path was found, otherwise <see langword="false"/>.</returns>
    private bool ComputeCostMinimalPath()
    {
        if (this.Start is null || this.Goal is null)
        {
            return false;
        }

        var maxSteps = MAX_COMPUTE_CYCLES;
        while (this._open.Count > 0 &&
               (this._open.First.CachedKey < this.Goal.CalculateKey(this._km) || this.Goal.RHS > this.Goal.G))
        {
            if (maxSteps-- <= 0)
            {
                break;
            }

            var u = this._open.First();
            var kOld = u.CachedKey;
            var kNew = u.CalculateKey(this._km);
            if (kOld < kNew)
            {
                this._open.UpdatePriority(u, kNew);
            }
            else if (u.G > u.RHS)
            {
                u.G = u.RHS;
                this._closed.Add(this._open.Dequeue());
                foreach (var s in u.Successors(this._grid))
                {
                    if (s == this.Start || s.RHS <= u + s)
                    {
                        continue;
                    }

                    s.Parent = u;
                    s.RHS = u + s;
                    this.UpdateState(s);
                }
            }
            else
            {
                u.G = int.MaxValue;
                foreach (var s in u.Collect(u.Successors(this._grid)))
                {
                    if (s != this.Start && s.Parent == u)
                    {
                        s.RHS = int.MaxValue;
                        foreach (var sPrime in s.Predecessors(this._grid))
                        {
                            s.RHS = Math.Min(s.RHS, sPrime + s);
                        }

                        s.Parent = s.RHS == int.MaxValue ? null : s.Predecessors(this._grid).ArgMin(sPrime => sPrime + s);
                    }

                    this.UpdateState(s);
                }
            }
        }

        return this.Goal.RHS != int.MaxValue;
    }

    /// <summary>Updates the specified <paramref name="state"/>'s priority in the queue if necessary.</summary>
    /// <param name="state">The <see cref="State"/>.</param>
    private void UpdateState(State state)
    {
        var key = state.CalculateKey(this._km);
        if (state.G != state.RHS)
        {
            if (this._open.Contains(state))
            {
                this._open.UpdatePriority(state, key);
            }
            else
            {
                this._open.Enqueue(state, key);
                this._closed.Remove(state);
            }
        }
        else if (this._open.Contains(state))
        {
            this._open.Remove(state);
            this._closed.Add(state);
        }
    }

    /// <summary>Corrects the Search Tree to account for a moving hunter and target.</summary>
    /// <param name="oldStart">The <see cref="State"/> at the root of the previous Search Tree.</param>
    /// <remarks>Basic version, which simply resets the <see cref="State.RHS"/> cost of <paramref name="oldStart"/> and inserts it back in the <see cref="_open"/> set.</remarks>
    private void BasicDeletion(State oldStart)
    {
        oldStart.RHS = int.MaxValue;
        foreach (var sPrime in oldStart.Predecessors(this._grid))
        {
            oldStart.RHS = Math.Min(oldStart.RHS, sPrime + oldStart);
        }

        oldStart.Parent = oldStart.RHS == int.MaxValue
            ? null
            : oldStart.Predecessors(this._grid).ArgMin(sPrime => sPrime + oldStart);
        this.UpdateState(oldStart);
    }

    /// <summary>Corrects the Search Tree to account for a moving hunter and target.</summary>
    /// <param name="oldStart">The <see cref="State"/> at the root of the previous Search Tree.</param>
    /// <remarks>The optimized version, which pre-emptively resets all affected states which would have been expanded by the new Search Tree.</remarks>
    private void OptimizedDeletion(State oldStart)
    {
        this._deleted.Clear();
        Stack<State> S = [];
        S.Push(oldStart);
        while (S.Count > 0)
        {
            var s = S.Pop();
            if (s == this._start)
            {
                continue;
            }

            s.RHS = s.G = int.MaxValue;
            s.Parent = null;
            if (this._open.Contains(s))
            {
                this._open.Remove(s);
                this._closed.Add(s);
            }

            this._deleted.Add(s);
            foreach (var c in s.Children)
            {
                S.Push(c);
            }
        }

        foreach (var s in this._deleted)
        {
            foreach (var sPrime in s.Predecessors(this._grid))
            {
                if (s.RHS <= sPrime + s)
                {
                    continue;
                }

                s.RHS = sPrime + s;
                s.Parent = sPrime;
            }

            if (s.RHS < int.MaxValue)
            {
                this._open.Enqueue(s, s.CalculateKey(this._km));
                this._closed.Remove(s);
            }
        }
    }

    /// <summary>Reconstructs the optimal poth from <see cref="Start"/> to <see cref="Goal"/>.</summary>
    /// <returns>The reconstructed path as a <see cref="Stack{T"/> of <see cref="State"/> instances.</returns>
    private Stack<Point> ReconstructPath()
    {
        this._path.Clear();
        var current = this.Goal;
        while (current is not null && current != this.Start)
        {
            this._path.Push(current.Position);
            current = current.Parent;
        }

        return this._path;
    }

    /// <summary>Represents a two-value priority key.</summary>
    /// <param name="k1">The first key value.</param>
    /// <param name="k2">The second key value.</param>
    public readonly struct Key(float k1, float k2) : IComparable<Key>
    {
        /// <summary>The first key value.</summary>
        public readonly float K1 = k1;

        /// <summary>The second key value.</summary>
        public readonly float K2 = k2;

        /// <summary>Compares whether the <paramref name="left"/> <see cref="Key"/> instance is less than the <paramref name="right"/> <see cref="Key"/> instance.</summary>
        /// <param name="left"><see cref="Key"/> instance on the left of the less-than sign.</param>
        /// <param name="right"><see cref="Key"/> instance on the right of the less-than sign.</param>
        /// <returns><see langword="true"/> if the <paramref name="left"/> instance is less than the <paramref name="right"/>; <see langword="false"/> otherwise.</returns>
        public static bool operator <(Key left, Key right) =>
            left.K1 < right.K1 || (left.K1.Approx(right.K1) && left.K2 < right.K2);

        /// <summary>Compares whether the  <paramref name="left"/> <see cref="Key"/> instance is greater than the <paramref name="right"/> <see cref="Key"/> instance.</summary>
        /// <param name="left"><see cref="Key"/> instance on the left of the greater-than sign.</param>
        /// <param name="right"><see cref="Key"/> instance on the right of the greater-than sign.</param>
        /// <returns><see langword="true"/> if the <paramref name="left"/> instance is greater than the <paramref name="right"/>; <see langword="false"/> otherwise.</returns>
        public static bool operator >(Key left, Key right) =>
            left.K1 > right.K1 || (left.K1.Approx(right.K1) && left.K2 > right.K2);

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
        public int CompareTo(Key other)
        {
            return this < other ? -1 : this > other ? 1 : 0;
        }

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

    /// <summary>Represents a state in a path.</summary>
    /// <remarks>Initializes a new instance of the <see cref="State"/> class.</remarks>
    /// <param name="position">The state's coordinates as a <see cref="Point"/>.</param>
    [DebuggerDisplay("Position = {Position} | G = {G} | RHS = {RHS} | H = {H} | IsWalkable = {IsWalkable}")]
    public sealed class State(Point position) : GenericPriorityQueueNode<Key>, IEquatable<State>
    {
        private State? _parent;

        /// <summary>Initializes a new instance of the <see cref="State"/> class.</summary>
        /// <param name="x">The state's X coordinate.</param>
        /// <param name="y">The state's Y coordinate.</param>
        internal State(int x, int y)
            : this(new Point(x, y))
        {
        }

        /// <summary>Gets or sets the cost function, which takes as input two state positions and returns the cost of a path between them.</summary>
        public static Func<Point, State, int> Cost { get; set; } = null!;

        /// <summary>Gets or sets the heuristic function, which takes as input a state's position and returns the cost of a path from that state to the target state.</summary>
        public static Func<Point, int> Heuristic { get; set; } = null!;

        /// <summary>Gets the total cost of a path from the starting state to this state.</summary>
        public Point Position { get; } = position;

        /// <summary>Gets the cost function for traveling from this state to some other state.</summary>
        public Func<State, int> C => Cost.Partial(this.Position);

        /// <summary>Gets the heuristic cost of a path from this state to the starting state.</summary>
        public int H => Heuristic(this.Position);

        /// <summary>Gets or sets the total cost of a path from the goal state to this state.</summary>
        public int G { get; set; } = int.MaxValue;

        /// <summary>Gets or sets the right-hand side value, which estimates this state's <see cref="G"/> based on that of its neighbors.</summary>
        public int RHS { get; set; } = int.MaxValue;

        /// <summary>Gets or sets the parent <see cref="State"/> of this instance; i.e., the previous state in the optimal path which passes through this instance.</summary>
        public State? Parent
        {
            get => this._parent;
            set
            {
                this._parent?.Children.Remove(this);
                value?.Children.Add(this);
                this._parent = value;
            }
        }

        /// <summary>Gets the children <see cref="State"/> of this instance; i.e., the states which have this instance as their <see cref="Parent"/>.</summary>
        public HashSet<State> Children { get; } = [];

        /// <summary>Gets the last <see cref="Key"/> value calculated for this state.</summary>
        public Key CachedKey { get; private set; }

        /// <summary>Gets a value indicating whether this state is the start state.</summary>
        public bool IsStart { get; internal set; }

        /// <summary>Gets a value indicating whether this state is the end state.</summary>
        public bool IsGoal { get; internal set; }

#if DEBUG
        /// <summary>Gets a value indicating whether this state is walkable.</summary>
        public bool IsWalkable => this.C(this) < int.MaxValue;
#endif

        /// <summary>Compares whether two <see cref="State" /> instances are equal.</summary>
        /// <param name="left"><see cref="State" /> instance on the left of the equal sign.</param>
        /// <param name="right"><see cref="State" /> instance on the right of the equal sign.</param>
        /// <returns><see langword="true"/> if the instances are equal; <see langword="false"/> otherwise.</returns>
        public static bool operator ==(State? left, State? right) => left?.Equals(right) ?? right is null;

        /// <summary>Compares whether two <see cref="State" /> instances are equal.</summary>
        /// <param name="left"><see cref="State" /> instance on the left of the equal sign.</param>
        /// <param name="right"><see cref="State" /> instance on the right of the equal sign.</param>
        /// <returns><see langword="true"/> if the instances are equal; <see langword="false"/> otherwise.</returns>
        public static bool operator !=(State? left, State? right) => !left?.Equals(right) ?? right is not null;

        /// <summary>Calculates the total cost to reach the <paramref name="right"/> <see cref="State"/> when traveling from the <paramref name="left"/> <see cref="State"/>, which is equal to the sum of the <see cref="G"/> cost of <paramref name="left"/> and the <see cref="C"/> cost from <paramref name="left"/> to <paramref name="right"/>.</summary>
        /// <param name="left"><see cref="Key"/> instance on the left of the plus sign.</param>
        /// <param name="right"><see cref="Key"/> instance on the right of the plus sign.</param>
        /// <returns>The sum of the <see cref="G"/> cost of <paramref name="left"/> and the <see cref="C"/> cost from <paramref name="left"/> to <paramref name="right"/>.</returns>
        public static int operator +(State left, State right) => Int32Extensions.AddWithoutOverflow(left.G, left.C(right));

        /// <summary>Calculates the key value, which is used to prioritize <see cref="State"/> instances for the pathfinding algorithm.</summary>
        /// <param name="km">The key modifier.</param>
        /// <returns>The <see cref="Key"/>.</returns>
        public Key CalculateKey(int km = 0)
        {
            this.CachedKey = new Key(
                Int32Extensions.AddWithoutOverflow(Math.Min(this.G, this.RHS), this.H, km),
                Math.Min(this.G, this.RHS));
            return this.CachedKey;
        }

        /// <summary>Gets the 4-connected neighbors to this state.</summary>
        /// <param name="grid">The grid of <see cref="State"/> instances.</param>
        /// <returns>A <see cref="IEnumerable{T}"/> of the (up to) four neighboring <see cref="State"/> instances.</returns>
        public IEnumerable<State> GetNeighbors(State[,] grid)
        {
            var (x, y) = this.Position;
            if (x > 0)
            {
                yield return grid[y, x - 1];
            }

            if (x < grid.GetLength(1) - 1)
            {
                yield return grid[y, x + 1];
            }

            if (y > 0)
            {
                yield return grid[y - 1, x];
            }

            if (y < grid.GetLength(0) - 1)
            {
                yield return grid[y + 1, x];
            }
        }

        /// <summary>Gets the 4-connected neighbors which connect to this state.</summary>
        /// <param name="grid">The grid of <see cref="State"/> instances.</param>
        /// <returns>A <see cref="IEnumerable{T}"/> of the (up to) four neighboring <see cref="State"/> instances.</returns>
        /// <remarks>This is only used for parity with the original paper.</remarks>
        public IEnumerable<State> Predecessors(State[,] grid)
        {
            return this.GetNeighbors(grid);
        }

        /// <summary>Gets the 4-connected neighbors this state is connected to.</summary>
        /// <param name="grid">The grid of <see cref="State"/> instances.</param>
        /// <returns>A <see cref="IEnumerable{T}"/> of the (up to) four neighboring <see cref="State"/> instances.</returns>
        /// <remarks>This is only used for parity with the original paper.</remarks>
        public IEnumerable<State> Successors(State[,] grid)
        {
            return this.GetNeighbors(grid);
        }

        /// <inheritdoc />
        public bool Equals(State? other)
        {
            return this.Position == other?.Position;
        }

        /// <inheritdoc />
        public override bool Equals(object? @object)
        {
            return this.Equals(@object as State);
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
