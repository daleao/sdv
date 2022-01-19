namespace DaLion.Stardew.Common.Extensions;

#region using directives

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

#endregion using directives

public static class Vector2Extensions
{
    public static double AngleWithHorizontal(this Vector2 v)
    {
        var (x, y) = v;
        return MathHelper.ToDegrees((float) Math.Atan2(0f - y, 0f - x));
    }

    /// <summary>Rotates the calling Vector2 by t to a Vector2 by <paramref name="degrees" />.</summary>
    public static Vector2 Perpendicular(this Vector2 v)
    {
        var (x, y) = v;
        return new(y, -x);
    }

    /// <summary>Rotates the calling <see cref="Vector2" /> by <paramref name="degrees" />.</summary>
    public static Vector2 Rotate(this ref Vector2 v, double degrees)
    {
        var sin = (float) Math.Sin(degrees * Math.PI / 180);
        var cos = (float) Math.Cos(degrees * Math.PI / 180);

        var tx = v.X;
        var ty = v.Y;
        v.X = cos * tx - sin * ty;
        v.Y = sin * tx + cos * ty;

        return v;
    }

    /// <summary>Get the 4-connected neighbors of the calling <see cref="Vector2"/>.</summary>
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

    /// <summary>Get the 8-connected neighbors of the calling <see cref="Vector2"/>.</summary>
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
}