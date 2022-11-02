namespace DaLion.Shared.Extensions.Xna;

#region using directives

using Microsoft.Xna.Framework;

#endregion using directives

/// <summary>Extensions for the <see cref="Color"/> struct.</summary>
public static class ColorExtensions
{
    /// <summary>Produces a new <see cref="Color"/> by adding the RGBA values of an<paramref name="other"/>.</summary>
    /// <param name="color">The <see cref="Color"/>.</param>
    /// <param name="other">A <see cref="Color"/> to add.</param>
    /// <returns>A new <see cref="Color"/> structure whose color values are the results of the addition operation.</returns>
    public static Color Add(this Color color, Color other)
    {
        return new Color(color.R + other.R, color.G + other.G, color.B + other.B, color.A + other.A);
    }

    /// <summary>Produces a new <see cref="Color"/> by subtracting the RGBA values of an<paramref name="color"/>.</summary>
    /// <param name="color">The <see cref="Color"/>.</param>
    /// <param name="other">A <see cref="Color"/> to subtract.</param>
    /// <returns>A new <see cref="Color"/> structure whose color values are the results of the subtraction operation.</returns>
    public static Color Subtract(this Color color, Color other)
    {
        return new Color(color.R - other.R, color.G - other.G, color.B - other.B, color.A - other.A);
    }

    /// <summary>Obtains a packed ARGB representation of the <paramref name="color"/>.</summary>
    /// <param name="color">The <see cref="Color"/>.</param>
    /// <returns>A 32-bit <see cref="uint"/> value representing the packed ARGB bytes of <paramref name="color"/>.</returns>
    public static uint ToPackedValue(this Color color)
    {
        return (uint)((color.A << 24) | (color.B << 16) | (color.G << 8) | color.R);
    }

    /// <summary>Gets the complementary <see cref="Color"/>.</summary>
    /// <param name="color">The <see cref="Color"/>.</param>
    /// <returns>A new <see cref="Color"/> created by inverting the RGB values of the original.</returns>
    public static Color Inverse(this Color color)
    {
        return new Color(color.ToPackedValue() ^ 0xFFFFFFu);
    }

    /// <summary>Performs a hue rotation by <paramref name="degrees"/> degrees.</summary>
    /// <param name="color">The <see cref="Color"/>.</param>
    /// <param name="degrees">The number of degrees to rotate by.</param>
    /// <returns>A new <see cref="Color"/> with the adjusted Hue.</returns>
    public static Color ShiftHue(this Color color, float degrees)
    {
        var (h, s, v) = color.ToHsv();
        var rotated = default(Color).FromHsv((h + degrees) % 360, s, v);
        return new Color(rotated.R, rotated.G, rotated.B, color.A);
    }

    /// <summary>Changes the <paramref name="color"/>'s saturation by <paramref name="amount"/>.</summary>
    /// <param name="color">The <see cref="Color"/>.</param>
    /// <param name="amount">The amount to change by, between 0 and 1.</param>
    /// <returns>A new <see cref="Color"/> with the adjusted Saturation.</returns>
    public static Color ChangeSaturation(this Color color, float amount)
    {
        amount = Math.Clamp(amount, 0f, 1f);
        var (h, s, v) = color.ToHsv();
        var changed = default(Color).FromHsv(h, s + amount, v);
        return new Color(changed.R, changed.G, changed.B, color.A);
    }

    /// <summary>Changes the <paramref name="color"/>'s value by <paramref name="amount"/>.</summary>
    /// <param name="color">The <see cref="Color"/>.</param>
    /// <param name="amount">The amount to change by, between 0 and 1.</param>
    /// <returns>A new <see cref="Color"/> with the adjusted Value.</returns>
    public static Color ChangeValue(this Color color, float amount)
    {
        amount = Math.Clamp(amount, 0f, 1f);
        var (h, s, v) = color.ToHsv();
        var changed = default(Color).FromHsv(h, s, v + amount);
        return new Color(changed.R, changed.G, changed.B, color.A);
    }

    /// <summary>Converts RGB color values to HSV representation.</summary>
    /// <param name="color">The <see cref="Color"/>.</param>
    /// <returns>A <see cref="Tuple"/> of three <see cref="float"/> values which represent the <paramref name="color"/>'s HSV components.</returns>
    public static (float Hue, float Saturation, float Value) ToHsv(this Color color)
    {
        float hue, saturation, value;

        var min = Math.Min(Math.Min(color.R, color.G), color.B);
        var max = Math.Max(Math.Max(color.R, color.G), color.B);
        value = max / 255f;

        var chroma = max == min ? 0f : max - min;
        if (chroma != 0)
        {
            saturation = chroma / max;

            if (max == color.R)
            {
                hue = ((color.G - color.B) / chroma) % 6;
            }
            else if (max == color.G)
            {
                hue = ((color.B - color.R) / chroma) + 2;
            }
            else
            {
                hue = ((color.R - color.G) / chroma) + 4;
            }

            hue *= 60;
            if (hue < 0)
            {
                hue += 360;
            }
        }
        else
        {
            saturation = 0;
            hue = -1;
        }

        return (hue, saturation, value);
    }

    /// <summary>Initializes the <paramref name="color"/> using the specified HSV values.</summary>
    /// <param name="color">The <see cref="Color"/>.</param>
    /// <param name="hue">The color's hue.</param>
    /// <param name="saturation">The color's saturation.</param>
    /// <param name="value">The color's value.</param>
    /// <returns>The same <paramref name="color"/> instance, initialized from the specified HSV values.</returns>
    public static Color FromHsv(this Color color, float hue, float saturation, float value)
    {
        var hi = Convert.ToInt32(Math.Floor(hue / 60)) % 6;
        var f = (hue / 60) - Math.Floor(hue / 60);

        value *= 255;
        var v = Convert.ToInt32(value);
        var p = Convert.ToInt32(value * (1 - saturation));
        var q = Convert.ToInt32(value * (1 - (f * saturation)));
        var t = Convert.ToInt32(value * (1 - ((1 - f) * saturation)));

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
        }

        return color;
    }
}
