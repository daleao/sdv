//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2024 Bad Echo LLC. All rights reserved.
//
//      Bad Echo Technologies are licensed under the
//      GNU Affero General Public License v3.0.
//
//      See accompanying file LICENSE.md or a copy at:
//      https://www.gnu.org/licenses/agpl-3.0.html
// </copyright>
//-----------------------------------------------------------------------

namespace DaLion.Shared.Classes;

#region using directives

using System.Diagnostics.CodeAnalysis;
using Microsoft.Xna.Framework;

#endregion using directives

/// <summary>
///     Provides a tree data structure in which each internal (non-leaf) node has exactly four children, useful
///     for 2D spatial information queries.
/// </summary>
/// <typeparam name="TSpatialEntity">A type with spatial properties.</typeparam>
public sealed class Quadtree<TSpatialEntity>
{
    private readonly List<TSpatialEntity> _entities = [];
    private readonly int _bucketCapacity;
    private readonly int _maxDepth;
    private readonly Func<TSpatialEntity, Rectangle> _getEntityBounds;

    private Quadtree<TSpatialEntity>? _topLeft, _topRight, _bottomLeft, _bottomRight;

    /// <summary>Initializes a new instance of the <see cref="Quadtree{TSpatialEntity}"/> class.</summary>
    /// <param name="regionBounds">The bounding <see cref="Rectangle"/> of the region that this <see cref="Quadtree{TSpatialEntity}"/>'s contents occupy.</param>
    /// <param name="getEntityBounds">A delegate for getting the <see cref="Rectangle"/> bounds of a <typeparamref name="TSpatialEntity"/>.</param>
    /// <remarks>
    ///     This initializes the <see cref="Quadtree{TSpatialEntity}"/> instance with a default bucket capacity of <c>32</c> and a default maximum
    ///     node depth of <c>5</c>, which are reasonable general-purpose values for these settings.
    /// </remarks>
    public Quadtree(Rectangle regionBounds, Func<TSpatialEntity, Rectangle> getEntityBounds)
        : this(regionBounds, getEntityBounds, 32, 5)
    {
        this._getEntityBounds = getEntityBounds;
    }

    /// <summary>Initializes a new instance of the <see cref="Quadtree{TSpatialEntity}"/> class.</summary>
    /// <param name="regionBounds">The bounding rectangle of the region that this QuadTree's contents occupy.</param>
    /// <param name="getEntityBounds">A delegate for getting the <see cref="Rectangle"/> bounds of a <typeparamref name="TSpatialEntity"/>.</param>
    /// <param name="bucketCapacity">The maximum number of items allowed in this node before it gets quartered.</param>
    /// <param name="maxDepth">The maximum number of levels of child nodes allowed across the entire quad tree.</param>
    public Quadtree(Rectangle regionBounds, Func<TSpatialEntity, Rectangle> getEntityBounds, int bucketCapacity, int maxDepth)
    {
        this._bucketCapacity = bucketCapacity;
        this._maxDepth = maxDepth;
        this._getEntityBounds = getEntityBounds;
        this.Bounds = regionBounds;
    }

    /// <summary>Gets the bounding rectangle of the region that this <see cref="Quadtree{TSpatialEntity}"/>'s contents occupy.</summary>
    public Rectangle Bounds { get; }

    /// <summary>
    ///     Gets the level of this node within the overall <see cref="Quadtree{TSpatialEntity}"/>; that is, the number of edges on the path from this
    ///     node to the root node.
    /// </summary>
    public int Level { get; init; }

    /// <summary>Gets a value indicating whether this node is a terminal node for the tree (i.e., it has no children).</summary>
    /// <remarks>
    ///     The majority of spatial elements reside in leaf nodes, the only exceptions being elements whose bounds overlap
    ///     multiple split node boundaries.
    /// </remarks>
    [MemberNotNullWhen(false, nameof(_topLeft), nameof(_topRight), nameof(_bottomLeft), nameof(_bottomRight))]
    public bool IsLeaf => this._topLeft == null || this._topRight == null ||
                          this._bottomLeft == null || this._bottomRight == null;

    /// <summary>Inserts the specified <paramref name="entity"/> into the <see cref="Quadtree{TSpatialEntity}"/> at the appropriate node.</summary>
    /// <param name="entity">The <typeparamref name="TSpatialEntity"/> to be inserted>.</param>
    public void Insert(TSpatialEntity entity)
    {
        if (entity is null)
        {
            ThrowHelper.ThrowArgumentException($"Attempted to insert null entity {nameof(entity)}.");
        }

        if (!this.Bounds.Contains(this._getEntityBounds(entity)))
        {
            ThrowHelper.ThrowArgumentException($"Entity {nameof(entity)} is outside of QuadTree bounds");
        }

        if (this._entities.Count >= this._bucketCapacity)
        {
            this.Split();
        }

        var containingChild = this.GetContainingChild(this._getEntityBounds(entity));
        if (containingChild is not null)
        {
            containingChild.Insert(entity);
        }
        else
        {
            this._entities.Add(entity);
        }
    }

    /// <summary>Removes the specified <paramref name="entity"/> from the <see cref="Quadtree{TSpatialEntity}"/> and whichever node it's been assigned to.</summary>
    /// <param name="entity">The <typeparamref name="TSpatialEntity"/> to be removed.</param>
    /// <returns><see langword="true"/> if the <paramref name="entity"/> is successfully removed, otherwise <see langword="false"/>.</returns>
    public bool Remove(TSpatialEntity entity)
    {
        if (entity is null)
        {
            ThrowHelper.ThrowArgumentException($"Attempted to remove null entity {nameof(entity)}.");
        }

        var containingChild = this.GetContainingChild(this._getEntityBounds(entity));
        var removed = containingChild?.Remove(entity) ?? this._entities.Remove(entity);
        if (removed && this.CountEntities() <= this._bucketCapacity)
        {
            this.Merge();
        }

        return removed;
    }

    /// <summary>Updates the positions of every <typeparamref name="TSpatialEntity"/> in the <see cref="Quadtree{TSpatialEntity}"/>.</summary>
    public void Update()
    {
        foreach (var e in this.GetEntities())
        {
            this.Remove(e);
            this.Insert(e);
        }
    }

    /// <summary>Gets the nearest neighbors of the specified <paramref name="entity"/>.</summary>
    /// <param name="entity">The spatial element to find collisions for.</param>
    /// <returns>All spatial elements that collide with <c>element</c>.</returns>
    public IEnumerable<TSpatialEntity> NearestNeighbors(TSpatialEntity entity)
    {
        if (entity is null)
        {
            ThrowHelper.ThrowArgumentException($"Entity {nameof(entity)}cannot be null.");
        }

        if (this.IsLeaf)
        {
            return this._entities;
        }

        var containingChild = this.GetContainingChild(this._getEntityBounds(entity));
        return containingChild?.NearestNeighbors(entity) ?? this._entities;
    }

    /// <summary>Gets the total number of entities belonging to this and all descending nodes.</summary>
    /// <returns>The total number of entities belong to this and all descending nodes.</returns>
    public int CountEntities()
    {
        var count = this._entities.Count;
        if (this.IsLeaf)
        {
            return count;
        }

        count += this._topLeft.CountEntities();
        count += this._topRight.CountEntities();
        count += this._bottomLeft.CountEntities();
        count += this._bottomRight.CountEntities();
        return count;
    }

    /// <summary>Retrieves the entities belonging to this and all descendant nodes.</summary>
    /// <returns>A <see cref="IEnumerable{T}"/> of the <typeparamref name="TSpatialEntity"/>'s belonging to this and all descendant nodes.</returns>
    public IEnumerable<TSpatialEntity> GetEntities()
    {
        var children = new List<TSpatialEntity>();
        var nodes = new Queue<Quadtree<TSpatialEntity>>();
        nodes.Enqueue(this);
        while (nodes.TryDequeue(out var node))
        {
            if (!node.IsLeaf)
            {
                nodes.Enqueue(node._topLeft);
                nodes.Enqueue(node._topRight);
                nodes.Enqueue(node._bottomLeft);
                nodes.Enqueue(node._bottomRight);
            }

            children.AddRange(node._entities);
        }

        return children;
    }

    private void Split()
    {
        if (!this.IsLeaf || this.Level + 1 > this._maxDepth)
        {
            return;
        }

        this._topLeft = this.CreateChild(this.Bounds.Location);
        this._topRight = this.CreateChild(new Point(this.Bounds.Center.X, this.Bounds.Location.Y));
        this._bottomLeft = this.CreateChild(new Point(this.Bounds.Location.X, this.Bounds.Center.Y));
        this._bottomRight = this.CreateChild(new Point(this.Bounds.Center.X, this.Bounds.Center.Y));

        var entities = this._entities.ToList();
        foreach (var entity in entities)
        {
            var containingChild = this.GetContainingChild(this._getEntityBounds(entity));
            if (containingChild == null)
            {
                continue;
            }

            this._entities.Remove(entity);
            containingChild.Insert(entity);
        }
    }

    private Quadtree<TSpatialEntity> CreateChild(Point p)
    {
        var size = new Point(this.Bounds.Size.X / 2, this.Bounds.Size.Y / 2);
        return new Quadtree<TSpatialEntity>(
            new Rectangle(p, size),
            this._getEntityBounds,
            this._bucketCapacity,
            this._maxDepth) { Level = this.Level + 1, };
    }

    private void Merge()
    {
        if (this.IsLeaf)
        {
            return;
        }

        this._entities.AddRange(this._topLeft._entities);
        this._entities.AddRange(this._topRight._entities);
        this._entities.AddRange(this._bottomLeft._entities);
        this._entities.AddRange(this._bottomRight._entities);
        this._topLeft = this._topRight = this._bottomLeft = this._bottomRight = null;
    }

    private Quadtree<TSpatialEntity>? GetContainingChild(Rectangle bounds)
    {
        if (this.IsLeaf)
        {
            return null;
        }

        if (this._topLeft.Bounds.Contains(bounds))
        {
            return this._topLeft;
        }

        if (this._topRight.Bounds.Contains(bounds))
        {
            return this._topRight;
        }

        if (this._bottomLeft.Bounds.Contains(bounds))
        {
            return this._bottomLeft;
        }

        return this._bottomRight.Bounds.Contains(bounds) ? this._bottomRight : null;
    }
}
