namespace DaLion.Common.Extensions.Xna;

#region using directives

using Microsoft.Xna.Framework;
using System;

#endregion using directives

/// <summary>Extensions for the <see cref="Color"/> class.</summary>
public static class ColorExtensions
{
    /// <summary>Perform a hue rotation by <paramref name="amount"/> degrees.</summary>
    /// <param name="amount">The amount to rotate by, in degrees.</param>
    public static Color ShiftHue(this Color color, int amount)
    {
        var (h, s, v) = color.ToHSV();
        var rotated = new Color().FromHSV((h + amount) % 360, s, v);
        return new(rotated.R, rotated.G, rotated.B, color.A);
    }

    /// <summary>Change the color's saturation by <paramref name="amount"/>.</summary>
    /// <param name="amount">The amount to change by, between 0 and 1.</param>
    public static Color ChangeSaturation(this Color color, float amount)
    {
        amount = Math.Clamp(amount, 0f, 1f);
        var (h, s, v) = color.ToHSV();
        var changed = new Color().FromHSV(h, s + amount, v);
        return new(changed.R, changed.G, changed.B, color.A);
    }

    /// <summary>Change the color's value by <paramref name="amount"/>.</summary>
    /// <param name="amount">The amount to change by, between 0 and 1.</param>
    public static Color ChangeValue(this Color color, float amount)
    {
        amount = Math.Clamp(amount, 0f, 1f);
        var (h, s, v) = color.ToHSV();
        var changed = new Color().FromHSV(h, s, v + amount);
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

    /// <summary>Initialize the <see cref="Color"/>" instance using HSV values.</summary>
    /// <param name="hue">The color's hue.</param>
    /// <param name="saturation">The color's saturation.</param>
    /// <param name="value">The color's value.</param>
    public static Color FromHSV(this Color color, float hue, float saturation, float value)
    {
        var hi = Convert.ToInt32(Math.Floor(hue / 60)) % 6;
        var f = hue / 60 - Math.Floor(hue / 60);

        value *= 255;
        var v = Convert.ToInt32(value);
        var p = Convert.ToInt32(value * (1 - saturation));
        var q = Convert.ToInt32(value * (1 - f * saturation));
        var t = Convert.ToInt32(value * (1 - (1 - f) * saturation));

        switch (hi)
        {
            case 0:
                color.R = (byte)v;
                color.G = (byte)t;
                color.B = (byte)p;
                break;
            case 1:
                color.R = (byte)q;
                color.G = (byte)v;
                color.B = (byte)p;
                break;
            case 2:
                color.R = (byte)p;
                color.G = (byte)v;
                color.B = (byte)t;
                break;
            case 3:
                color.R = (byte)p;
                color.G = (byte)q;
                color.B = (byte)v;
                break;
            case 4:
                color.R = (byte)t;
                color.G = (byte)p;
                color.B = (byte)v;
                break;
            default:
                color.R = (byte)v;
                color.G = (byte)p;
                color.B = (byte)q;
                break;
        };

        return color;
    }
}