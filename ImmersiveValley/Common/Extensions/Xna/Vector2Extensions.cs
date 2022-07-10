using System.Linq;

namespace DaLion.Common.Extensions.Xna;

#region using directives

using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

#endregion using directives

/// <summary>Extensions for the <see cref="Vector2"/> class.</summary>
public static class Vector2Extensions
{
    /// <summary>Get the angle between the instance and the horizontal.</summary>
    public static double AngleWithHorizontal(this Vector2 v)
    {
        var (x, y) = v;
        return MathHelper.ToDegrees((float)Math.Atan2(0f - y, 0f - x));
    }

    /// <summary>Rotates the instance by t to a Vector2 by <paramref name="degrees" />.</summary>
    public static Vector2 Perpendicular(this Vector2 v)
    {
        var (x, y) = v;
        return new(y, -x);
    }

    /// <summary>Rotates the instance by <paramref name="degrees" />.</summary>
    public static Vector2 Rotate(this Vector2 v, double degrees)
    {
        var sin = (float)Math.Sin(degrees * Math.PI / 180);
        var cos = (float)Math.Cos(degrees * Math.PI / 180);

        var tx = v.X;
        var ty = v.Y;
        v.X = cos * tx - sin * ty;
        v.Y = sin * tx + cos * ty;

        return v;
    }

    /// <summary>Get the 4-connected neighbors of the instance.</summary>
    /// <param name="w">The width of the region.</param>
    /// <param name="h">The height of the region.</param>
    public static IEnumerable<Vector2> GetFourNeighbours(this Vector2 v, int w, int h)
    {
        var (x, y) = v;
        if (x > 0) yield return new(x - 1, y);
        if (x < w - 1) yield return new(x + 1, y);
        if (y > 0) yield return new(x, y - 1);
        if (y < h - 1) yield return new(x, y + 1);
    }

    /// <summary>Get the 8-connected neighbors of the instance.</summary>
    /// <param name="w">The width of the region.</param>
    /// <param name="h">The height of the region.</param>
    public static IEnumerable<Vector2> GetEightNeighbours(this Vector2 v, int w, int h)
    {
        var (x, y) = v;
        if (x > 0 && y > 0) yield return new(x - 1, y - 1);
        if (x > 0 && y < h - 1) yield return new(x - 1, y + 1);
        if (x < w - 1 && y > 0) yield return new(x + 1, y - 1);
        if (x < w - 1 && y < h - 1) yield return new(x + 1, y + 1);
        foreach (var neighbour in GetFourNeighbours(v, w, h)) yield return neighbour;
    }

    /// <summary>Search for region boundaries using a Flood Fill algorithm.</summary>
    /// <param name="origin">The starting point for the fill, as a <see cref="Vector2"/>.</param>
    /// <param name="width">The width of the region.</param>
    /// <param name="height">The height of the region.</param>
    /// <param name="predicate">The boundary condition.</param>
    /// <returns>The set of points belonging to the region, as <see cref="Vector2"/>.</returns>
    public static IEnumerable<Vector2> FloodFill(Vector2 origin, int width, int height, Func<Vector2, bool> predicate)
    {
        if (origin.X <= 0) origin = new(origin.X + 1, origin.Y);
        else if (origin.Y <= 0) origin = new(origin.X, origin.Y + 1);
        else if (origin.X >= width - 1) origin = new(origin.X - 1, origin.Y);
        else if (origin.Y >= height - 1) origin = new(origin.X, origin.Y - 1);

        var result = new List<Vector2>();
        var visited = new HashSet<Vector2>();
        var toVisit = new Queue<Vector2>();
        toVisit.Enqueue(origin);
        while (toVisit.Count > 0)
        {
            var tile = toVisit.Dequeue();
            if (!visited.Add(tile))
                continue;

            if (!predicate(tile)) continue;

            result.Add(tile);
            foreach (var neighbour in tile.GetEightNeighbours(width, height).Where(v => !visited.Contains(v)))
                toVisit.Enqueue(neighbour);
        }

        return result;
    }
}