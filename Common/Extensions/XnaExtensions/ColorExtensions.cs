namespace DaLion.Common.Extensions.Xna;

#region using directives

using System;
using Microsoft.Xna.Framework;

using Classes;

#endregion using directives

public static class ColorExtensions
{
    /// <summary>Perform a hue rotation by <paramref name="amount"/> degrees.</summary>
    /// <param name="amount">The amount to rotate by, in degrees.</param>
    public static Color ShiftHue(this Color color, int amount)
    {
        var (h, s, v) = color.ToHSV();
        var rotated = Colors.FromHSV((h + amount) % 360, s, v);
        return new(rotated.R, rotated.G, rotated.B, color.A);
    }

    /// <summary>Change the color's saturation by <paramref name="amount"/>.</summary>
    /// <param name="amount">The amount to change by, between 0 and 1.</param>
    public static Color ChangeSaturation(this Color color, float amount)
    {
        amount = Math.Clamp(amount, 0f, 1f);
        var (h, s, v) = color.ToHSV();
        var changed = Colors.FromHSV(h, s + amount, v);
        return new(changed.R, changed.G, changed.B, color.A);
    }

    /// <summary>Change the color's value by <paramref name="amount"/>.</summary>
    /// <param name="amount">The amount to change by, between 0 and 1.</param>
    public static Color ChangeValue(this Color color, float amount)
    {
        amount = Math.Clamp(amount, 0f, 1f);
        var (h, s, v) = color.ToHSV();
        var changed = Colors.FromHSV(h, s, v + amount);
        return new(changed.R, changed.G, changed.B, color.A);
    }

    /// <summary>Convert RGB color values to HSV representation.</summary>
    public static (float, float, float) ToHSV(this Color color)
    {
        float hue, saturation, value;
        
        var min = Math.Min(Math.Min(color.R, color.G), color.B);
        var max = Math.Max(Math.Max(color.R, color.G), color.B);
        value = max / 255f;

        var chroma = max == min ? 0f : max - min;
        if (chroma != 0)
        {
            saturation = chroma / max;

            if (max == color.R) hue = (color.G - color.B) / chroma % 6;
            else if (max == color.G) hue = (color.B - color.R) / chroma + 2;
            else hue = (color.R - color.G) / chroma + 4;
            
            hue *= 60;
            if (hue < 0) hue += 360;
        }
        else
        {
            saturation = 0;
            hue = -1;
        }

        return (hue, saturation, value);
    }
}