using System;
using Microsoft.Xna.Framework;

namespace TheLion.Stardew.Common.Extensions;

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
}