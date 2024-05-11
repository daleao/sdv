namespace DaLion.Shared.Classes;

#region using directives

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using DaLion.Shared.Extensions.Functional;
using Integrations.CustomOreNodes;
using Microsoft.Xna.Framework;
using Priority_Queue;

#endregion using directives

/// <summary>Finds the shortest path between two <see cref="Point"/>s.</summary>
public sealed class Pathfinder
{
    private readonly FastPriorityQueue<Node> _open;
    private readonly HashSet<Node> _closed = [];
    private readonly Node[,] _graph;

    /// <summary>Initializes a new instance of the <see cref="Pathfinder"/> class.</summary>
  
    /// <param name="heuristic">The heuristic function, which takes as input hte current node's position and the target node's position, and returns the cost of a path between the two.</param>
    public Pathfinder(Node[,] graph, Func<Point, Point, float> heuristic)
    {
        this._graph = graph;
        Node.Heuristic = heuristic.Partial<Point, float>(goal);
        this._open = new FastPriorityQueue<Node>(graph.Length);
    }

    /// <summary>Attempts to find the shortest path.</summary>
    /// <param name="start">The starting node's coordinates, as a <see cref="Point"/>.</param>
    /// <param name="goal">The target node's coordinates, as a <see cref="Point"/>.</param>
    /// <param name="path">The path, if found.</param>
    /// <returns><see langword="true"/> if the path was found, otherwise <see langword="false"/>.</returns>
    public bool FindPath(Point start, Point goal, [NotNullWhen(true)] out List<Point>? path)
    {
        var startNode = new Node(start) { G = 0 };
        var endNode = new Node(goal);
        this._open.Enqueue(startNode, startNode.F);
        this._closed.Add(startNode);
        while (this._open.Count > 0)
        {
            var current = this._open.Dequeue();
            if (current.Equals(this._goal))
            {
                path = this.ReconstructPath();
                return true;
            }

            foreach (var neighbor in current.GetNeighbors(graph))
            {
                var g = current.G + 1;
                if (!(g < neighbor.G))
                {
                    continue;
                }

                neighbor.Parent = current;
                neighbor.G = g;
                if (!this._closed.Contains(neighbor))
                {
                    this._open.Enqueue(neighbor, g);
                    this._closed.Add(neighbor);
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

    private List<Point> ReconstructPath()
    {
        List<Point> path = [this._goal.Position];
        var previous = this._goal.Parent;
        while (previous is not null)
        {
            path.Insert(0, previous.Position);
            previous = previous.Parent;
        }

        return path;
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
        public static Func<Point, float> Heuristic { get; set; } = null!;

        /// <summary>Gets the total cost of a path from the starting node to this node.</summary>
        public Point Position { get; } = position;

        /// <summary>Gets or sets the total cost of a path from the starting node to this node.</summary>
        public float G { get; set; } = float.MaxValue;

        /// <summary>Gets the heuristic cost of a path from this node to the target node.</summary>
        public float H { get; } = Heuristic(position);

        /// <summary>Gets the estimated cost of a path from start to finish going through this node.</summary>
        public float F => this.G + this.H;

        /// <summary>Gets or sets the node immediately preceding this one in the path with cost <see cref="G"/>.</summary>
        public Node? Parent { get; set; }

        public static bool operator ==(Node left, Node? right) => left.Equals(right);

        public static bool operator !=(Node left, Node? right) => !left.Equals(right);

        /// <summary>Gets the 4-connected neighbors to this node.</summary>
        /// <param name="graph">The graph of valid node positions.</param>
        /// <returns>A <see cref="IEnumerable{T}"/> of the (up to) four neighboring <see cref="Node"/>s.</returns>
        public IEnumerable<Node> GetNeighbors(Node[,] graph)
        {
            var (x, y) = this.Position;
            if (x > 0)
            {
                yield return graph[x - 1, y];
            }

            if (x < graph.GetLength(1) - 1)
            {
                yield return graph[x + 1, y];
            }

            if (y > 0)
            {
                yield return graph[x, y - 1];
            }

            if (y < graph.GetLength(0) - 1)
            {
                yield return graph[x, y + 1];
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
