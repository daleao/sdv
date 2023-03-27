namespace DaLion.Shared.Extensions.Xna;

#region using directives

using System;
using System.Globalization;
using Microsoft.Xna.Framework;

#endregion using directives

/// <summary>Extensions for the <see cref="string"/> primitive type.</summary>
public static class StringExtensions
{
    /// <summary>Attempts to parse an HTML color string to a <see cref="Color"/>.</summary>
    /// <param name="html">An HTML color string.</param>
    /// <param name="color">The parsed <see cref="Color"/>.</param>
    /// <returns><see langword="true"/> if the HTML string was valid, otherwise <see langword="false"/>.</returns>
    public static bool TryGetColorFromHtml(this string html, out Color color)
    {
        color = default;
        if (html[0] != '#')
        {
            Log.E("HTML code must begin with '#'.");
            return false;
        }

        int len;
        var hasAlpha = false;
        switch (html.Length)
        {
            case 4:
                len = 1;
                break;
            case 5:
                len = 1;
                hasAlpha = true;
                break;
            case 7:
                len = 2;
                break;
            case 9:
                len = 2;
                hasAlpha = true;
                break;
            default:
                Log.E($"HTML code '{html}' has invalid length.");
                return false;
        }

        byte[] array = { 255, 255, 255, 255 };
        for (var i = 0; i < 4; i++)
        {
            if (i == 3 && !hasAlpha)
            {
                break;
            }

            if (!byte.TryParse(
                    html.AsSpan().Slice((i * len) + 1, len),
                    NumberStyles.HexNumber,
                    CultureInfo.InvariantCulture,
                    out var parsed))
            {
                Log.E($"Failed to parse HTML code '{html}'.");
                return false;
            }

            array[i] = len == 2 ? parsed : (byte)(parsed * 0x11);
        }

        color.R = array[0];
        color.G = array[1];
        color.B = array[2];
        color.A = array[3];
        return true;
    }
}
