namespace DaLion.Shared.Extensions.Xna;

#region using directives

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Xna.Framework;

#endregion using directives

/// <summary>Extensions for the <see cref="Vector2"/> struct.</summary>
public static class Vector2Extensions
{
    /// <summary>Gets the angle between this <paramref name="vector"/> and the horizontal, in degrees.</summary>
    /// <param name="vector">The <see cref="Vector2"/>.</param>
    /// <returns>The angle between the <paramref name="vector"/> and a horizontal line, in degrees.</returns>
    public static double AngleWithHorizontal(this Vector2 vector)
    {
        var (x, y) = vector;
        return Math.Atan2(0f - y, 0f - x) * (180 / Math.PI);
    }

    /// <summary>Gets the angle between this <paramref name="self"/> and <paramref name="other"/>, in degrees.</summary>
    /// <param name="self">The <see cref="Vector2"/>.</param>
    /// <param name="other">Some other <see cref="Vector2"/>.</param>
    /// <returns>The angle between <paramref name="self"/> and <paramref name="other"/>, in degrees.</returns>
    public static double AngleWith(this Vector2 self, Vector2 other)
    {
        var (ax, ay) = self;
        var (bx, by) = other;
        double sin = (ax * by) - (bx * ay);
        double cos = (ax * bx) + (ay * by);
        return Math.Atan2(sin, cos) * (180 / Math.PI);
    }

    /// <summary>Creates a new <see cref="Vector2"/> that is perpendicular to this one.</summary>
    /// <param name="vector">The <see cref="Vector2"/>.</param>
    /// <returns>A new <see cref="Vector2"/> obtained by a 90-degree anti-clockwise rotation of the original.</returns>
    public static Vector2 Perpendicular(this Vector2 vector)
    {
        var (x, y) = vector;
        return new Vector2(y, -x);
    }

    /// <summary>Creates a new <see cref="Vector2"/> by rotating this <paramref name="vector"/> by the specified <paramref name="angle"/>, in degrees.</summary>
    /// <param name="vector">The <see cref="Vector2"/>.</param>
    /// <param name="angle">An angle, in degrees, to rotate by.</param>
    /// <returns>A new <see cref="Vector2"/> obtained by the specified rotation of the original.</returns>
    public static Vector2 Rotate(this Vector2 vector, double angle)
    {
        var sin = (float)Math.Sin(angle * Math.PI / 180d);
        var cos = (float)Math.Cos(angle * Math.PI / 180d);
        var tx = vector.X;
        var ty = vector.Y;
        return new Vector2(
            (cos * tx) - (sin * ty),
            (sin * tx) + (cos * ty));
    }

    /// <summary>Gets the 4-connected neighboring tiles in a given region.</summary>
    /// <param name="tile">The center tile as a <see cref="Vector2"/>.</param>
    /// <param name="width">The width of the entire region.</param>
    /// <param name="height">The height of the entire region.</param>
    /// <returns>A <see cref="IEnumerable{T}"/> of the four-connected neighbors of the <paramref name="tile"/>.</returns>
    [SuppressMessage("StyleCop.CSharp.LayoutRules", "SA1503:Braces should not be omitted", Justification = "Conciseness")]
    public static IEnumerable<Vector2> GetFourNeighbors(this Vector2 tile, int width, int height)
    {
        var (x, y) = tile;
        if (x > 0f) yield return new Vector2(x - 1f, y);
        if (y > 0f) yield return new Vector2(x, y - 1f);
        if (x < width - 1f) yield return new Vector2(x + 1f, y);
        if (y < height - 1) yield return new Vector2(x, y + 1f);
    }

    /// <summary>Gets the 8-connected neighboring tiles in a given region.</summary>
    /// <param name="tile">The center tile as a <see cref="Vector2"/>.</param>
    /// <param name="width">The width of the entire region.</param>
    /// <param name="height">The height of the entire region.</param>
    /// <returns>A <see cref="IEnumerable{T}"/> of the eight-connected neighbors of the <paramref name="tile"/>.</returns>
    [SuppressMessage("StyleCop.CSharp.LayoutRules", "SA1503:Braces should not be omitted", Justification = "Conciseness")]
    public static IEnumerable<Vector2> GetEightNeighbors(this Vector2 tile, int width, int height)
    {
        var (x, y) = tile;
        if (x > 0f)
        {
            yield return new Vector2(x - 1f, y);
            if (y > 0f) yield return new Vector2(x - 1f, y - 1f);
            if (y < height - 1f) yield return new Vector2(x - 1f, y + 1f);
        }

        if (x < width - 1f)
        {
            yield return new Vector2(x + 1f, y);
            if (y > 0f) yield return new Vector2(x + 1f, y - 1f);
            if (y < height - 1f) yield return new Vector2(x + 1f, y + 1f);
        }

        if (y > 0f) yield return new Vector2(x, y - 1f);
        if (y < height - 1f) yield return new Vector2(x, y + 1f);
    }

    /// <summary>Gets the 24 neighboring tiles in a given region.</summary>
    /// <param name="tile">The center tile as a <see cref="Vector2"/>.</param>
    /// <param name="width">The width of the entire region.</param>
    /// <param name="height">The height of the entire region.</param>
    /// <returns>A <see cref="IEnumerable{T}"/> of the twenty-four-connected neighbors of the <paramref name="tile"/>.</returns>
    public static IEnumerable<Vector2> GetTwentyFourNeighbors(this Vector2 tile, int width, int height)
    {
        foreach (var neighbor in tile.GetEightNeighbors(width, height))
        {
            yield return neighbor;
        }

        var x = (int)tile.X;
        var y = (int)tile.Y;
        if (y - 2 >= 0)
        {
            for (var i = -2; i <= 2; i++)
            {
                if ((x + i).IsIn(..width))
                {
                    yield return new Vector2(x + i, y - 2);
                }
            }
        }

        if (y + 2 < height)
        {
            for (var i = -2; i <= 2; i++)
            {
                if ((x + i).IsIn(..width))
                {
                    yield return new Vector2(x + i, y + 2);
                }
            }
        }

        if (x - 2 >= 0)
        {
            for (var j = -1; j <= 1; j++)
            {
                if ((y + j).IsIn(..height))
                {
                    yield return new Vector2(x - 2, y + j);
                }
            }
        }

        if (x + 2 < width)
        {
            for (var j = -1; j <= 1; j++)
            {
                if ((y + j).IsIn(..height))
                {
                    yield return new Vector2(x + 2, y + j);
                }
            }
        }
    }

    /// <summary>Gets the 48 neighboring tiles in a given region.</summary>
    /// <param name="tile">The center tile as a <see cref="Vector2"/>.</param>
    /// <param name="width">The width of the entire region.</param>
    /// <param name="height">The height of the entire region.</param>
    /// <returns>A <see cref="IEnumerable{T}"/> of the twenty-four-connected neighbors of the <paramref name="tile"/>.</returns>
    public static IEnumerable<Vector2> GetFourtyEightNeighbors(this Vector2 tile, int width, int height)
    {
        foreach (var neighbor in tile.GetTwentyFourNeighbors(width, height))
        {
            yield return neighbor;
        }

        var x = (int)tile.X;
        var y = (int)tile.Y;
        if (y - 3 >= 0)
        {
            for (var i = -3; i <= 3; i++)
            {
                if ((x + i).IsIn(..width))
                {
                    yield return new Vector2(x + i, y - 3);
                }
            }
        }

        if (y + 3 < height)
        {
            for (var i = -3; i <= 3; i++)
            {
                if ((x + i).IsIn(..width))
                {
                    yield return new Vector2(x + i, y + 3);
                }
            }
        }

        if (x - 3 >= 0)
        {
            for (var j = -2; j <= 2; j++)
            {
                if ((y + j).IsIn(..height))
                {
                    yield return new Vector2(x - 3, y + j);
                }
            }
        }

        if (x + 3 < width)
        {
            for (var j = -2; j <= 2; j++)
            {
                if ((y + j).IsIn(..height))
                {
                    yield return new Vector2(x + 3, y + j);
                }
            }
        }
    }

    /// <summary>Gets the horizontal and vertical unit vector projections of this <paramref name="vector"></paramref>.</summary>
    /// <param name="vector">The <see cref="Vector2"/>.</param>
    /// <returns>Two unit vectors which point in the same direction as the components of <paramref name="vector"/>.</returns>
    public static (Vector2 Horizontal, Vector2 Vertical) GetUnitComponents(this Vector2 vector)
    {
        var horizontal = vector.X > 0f ? VectorUtils.RightVector() : vector.X < 0f ? VectorUtils.LeftVector() : Vector2.Zero;
        var vertical = vector.Y > 0f ? VectorUtils.DownVector() : vector.Y < 0f ? VectorUtils.UpVector() : Vector2.Zero;
        return (horizontal, vertical);
    }

    /// <summary>Searches for region boundaries using a Flood Fill algorithm.</summary>
    /// <param name="origin">The starting point for the fill, as a <see cref="Vector2"/>.</param>
    /// <param name="width">The width of the region.</param>
    /// <param name="height">The height of the region.</param>
    /// <param name="boundary">The boundary condition.</param>
    /// <param name="minDistance">The shortest Manhattan distance acceptable from the origin.</param>
    /// <param name="maxDistance">The largest Manhattan distance acceptable from the origin.</param>
    /// <param name="maxSteps">The maximum allowed number of iterations for the algorithm.</param>
    /// <returns>A <see cref="IEnumerable{T}"/> of <see cref="Vector2"/>s belonging to the enclosed region and their respective Manhattan distances from the origin.</returns>
    public static IEnumerable<(Vector2 Tile, int Distance)> FloodFill(
        this Vector2 origin,
        int width,
        int height,
        Func<Vector2, bool> boundary,
        int minDistance = 0,
        int maxDistance = int.MaxValue,
        int maxSteps = int.MaxValue)
    {
        var flooded = new List<Vector2>();
        var queue = new Queue<Vector2>([origin]);
        var distanceByTile = new Dictionary<Vector2, int> { [origin] = 0 };
        foreach (var neighbor in origin.GetFourNeighbors(width, height))
        {
            if (boundary(neighbor) && distanceByTile.TryAdd(neighbor, 1))
            {
                queue.Enqueue(neighbor);
            }
        }

        var steps = 0;
        while (queue.TryDequeue(out var tile) && steps++ < maxSteps)
        {
            if (!boundary(tile))
            {
                continue;
            }

            flooded.Add(tile);
            var distance = distanceByTile[tile];
            foreach (var neighbor in tile.GetFourNeighbors(width, height))
            {
                if (distanceByTile.TryAdd(neighbor, distance + 1))
                {
                    queue.Enqueue(neighbor);
                }
            }
        }

        return flooded
            .Select(tile => (Tile: tile, Distance: distanceByTile[tile]))
            .Where(tuple => tuple.Distance >= minDistance && tuple.Distance <= maxDistance);
    }
}
