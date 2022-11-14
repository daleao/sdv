namespace DaLion.Shared.Extensions.Xna;

#region using directives

using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

#endregion using directives

/// <summary>Extensions for the <see cref="Vector2"/> struct.</summary>
public static class Vector2Extensions
{
    /// <summary>Gets the angle between the <paramref name="vector"/> and the horizontal.</summary>
    /// <param name="vector">The <see cref="Vector2"/>.</param>
    /// <returns>The angle between the <paramref name="vector"/> and a horizontal line.</returns>
    public static double AngleWithHorizontal(this Vector2 vector)
    {
        var (x, y) = vector;
        return Math.Atan2(0f - y, 0f - x) * (180 / Math.PI);
    }

    /// <summary>Gets the angle between the <paramref name="vector"/> and an<paramref name="other"/>.</summary>
    /// <param name="vector">The <see cref="Vector2"/>.</param>
    /// <param name="other">Some other <see cref="Vector2"/>.</param>
    /// <returns>The angle between <paramref name="vector"/> and <paramref name="other"/>.</returns>
    public static double AngleBetween(this Vector2 vector, Vector2 other)
    {
        var (ax, ay) = vector;
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

    /// <summary>Rotates the <paramref name="vector"/> by the specified <paramref name="angle"/>, in degrees.</summary>
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
    /// <param name="tile">The tile.</param>
    /// <param name="width">The width of the entire region.</param>
    /// <param name="height">The height of the entire region.</param>
    /// <returns>A <see cref="IEnumerable{T}"/> of the four-connected neighbors of the <paramref name="tile"/>.</returns>
    public static IEnumerable<Vector2> GetFourNeighbors(this Vector2 tile, int width, int height)
    {
        var (x, y) = tile;
        if (x > 0)
        {
            yield return new Vector2(x - 1, y);
        }

        if (x < width - 1)
        {
            yield return new Vector2(x + 1, y);
        }

        if (y > 0)
        {
            yield return new Vector2(x, y - 1);
        }

        if (y < height - 1)
        {
            yield return new Vector2(x, y + 1);
        }
    }

    /// <summary>Gets the 8-connected neighboring tiles in a given region.</summary>
    /// <param name="vector">The tile.</param>
    /// <param name="width">The width of the entire region.</param>
    /// <param name="height">The height of the entire region.</param>
    /// <returns>A <see cref="IEnumerable{T}"/> of the eight-connected neighbors of the <paramref name="vector"/>.</returns>
    public static IEnumerable<Vector2> GetEightNeighbors(this Vector2 vector, int width, int height)
    {
        var (x, y) = vector;
        if (x > 0 && y > 0)
        {
            yield return new Vector2(x - 1, y - 1);
        }

        if (x > 0 && y < height - 1)
        {
            yield return new Vector2(x - 1, y + 1);
        }

        if (x < width - 1 && y > 0)
        {
            yield return new Vector2(x + 1, y - 1);
        }

        if (x < width - 1 && y < height - 1)
        {
            yield return new Vector2(x + 1, y + 1);
        }

        foreach (var neighbour in GetFourNeighbors(vector, width, height))
        {
            yield return neighbour;
        }
    }

    /// <summary>Gets the unit vectors which point in the same direction as the components of the <see cref="Vector2"/>.</summary>
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
    /// <param name="predicate">The boundary condition.</param>
    /// <returns>The set of points belonging to the region, as <see cref="Vector2"/>.</returns>
    public static IEnumerable<Vector2> FloodFill(this Vector2 origin, int width, int height, Func<Vector2, bool> predicate)
    {
        if (origin.X <= 0)
        {
            origin = new Vector2(origin.X + 1, origin.Y);
        }
        else if (origin.Y <= 0)
        {
            origin = new Vector2(origin.X, origin.Y + 1);
        }
        else if (origin.X >= width - 1)
        {
            origin = new Vector2(origin.X - 1, origin.Y);
        }
        else if (origin.Y >= height - 1)
        {
            origin = new Vector2(origin.X, origin.Y - 1);
        }

        var result = new List<Vector2>();
        var visited = new HashSet<Vector2>();
        var toVisit = new Queue<Vector2>();
        toVisit.Enqueue(origin);
        while (toVisit.Count > 0)
        {
            var tile = toVisit.Dequeue();
            if (!visited.Add(tile))
            {
                continue;
            }

            if (!predicate(tile))
            {
                continue;
            }

            result.Add(tile);
            foreach (var neighbor in tile.GetEightNeighbors(width, height).Where(v => !visited.Contains(v)))
            {
                toVisit.Enqueue(neighbor);
            }
        }

        return result;
    }
}
