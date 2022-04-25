namespace DaLion.Common.Extensions.Xna;

#region using directives

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#endregion using directives

/// <summary>Extensions for the <see cref="Vector2"/> class.</summary>
public static class Vector2Extensions
{
    /// <summary>Get the angle between the instance and the horizontal.</summary>
    public static double AngleWithHorizontal(this Vector2 v)
    {
        var (x, y) = v;
        return MathHelper.ToDegrees((float) Math.Atan2(0f - y, 0f - x));
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
        var sin = (float) Math.Sin(degrees * Math.PI / 180);
        var cos = (float) Math.Cos(degrees * Math.PI / 180);

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

    /// <summary>Draw a border of specified height and width starting at the <see cref="Vector2"/> instance.</summary>
    /// <param name="height">The height of the border.</param>
    /// <param name="width">The width of the border.</param>
    /// <param name="pixel">The border pixel texture.</param>
    /// <param name="thickness">Border thickness.</param>
    /// <param name="color">Border color.</param>
    /// <param name="b"><see cref="SpriteBatch" /> to draw to.</param>
    public static void DrawBorder(this Vector2 v, int height, int width, Texture2D pixel, int thickness, Color color, SpriteBatch b)
    {
        var (x, y) = v;
        b.Draw(pixel, new Rectangle((int) x, (int) y, width, thickness), color); // top line
        b.Draw(pixel, new Rectangle((int) x, (int) y, thickness, height), color); // left line
        b.Draw(pixel, new Rectangle((int) x + width - thickness, (int) y, thickness, height), color); // right line
        b.Draw(pixel, new Rectangle((int) x, (int) y + height - thickness, width, thickness), color); // bottom line
    }

    /// <summary>Draw a border of specified height and width starting at the <see cref="Vector2"/> instance.</summary>
    /// <param name="height">The height of the border.</param>
    /// <param name="width">The width of the border.</param>
    /// <param name="pixel">The border pixel texture.</param>
    /// <param name="thickness">Border thickness.</param>
    /// <param name="color">Border color.</param>
    /// <param name="b"><see cref="SpriteBatch" /> to draw to.</param>
    public static void DrawBorder(this Vector2 v, int height, int width, Texture2D pixel, int thickness, Color color, SpriteBatch b, Vector2 offset)
    {
        var (x, y) = v + offset;
        b.Draw(pixel, new Rectangle((int) x, (int) y, width, thickness), color); // top line
        b.Draw(pixel, new Rectangle((int) x, (int) y, thickness, height), color); // left line
        b.Draw(pixel, new Rectangle((int) x + width - thickness, (int) y, thickness, height), color); // right line
        b.Draw(pixel, new Rectangle((int) x, (int) y + height - thickness, width, thickness), color); // bottom line
    }
}