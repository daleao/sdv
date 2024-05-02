namespace DaLion.Shared.Extensions.Xna;

#region using directives

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using Microsoft.Xna.Framework;

#endregion using directives

/// <summary>Extensions for the <see cref="string"/> primitive type.</summary>
public static class StringExtensions
{
    private static readonly Lazy<Dictionary<string, Color>> ColorByName = new(() =>
    {
        Dictionary<string, Color> colors = new(StringComparer.InvariantCultureIgnoreCase);
        foreach (var color in typeof(Color).GetProperties().Where(pi => pi.PropertyType == typeof(Color)))
        {
            colors[color.Name.ToLowerInvariant()] = (Color)color.GetValue(null)!;
        }

        return colors;
    });

    /// <summary>
    ///     Splits the string using the provided <paramref name="separator"/> and parses the resulting two components into
    ///     a <see cref="Point"/>.
    /// </summary>
    /// <param name="string">The <see cref="string"/>.</param>
    /// <param name="separator">The element separator.</param>
    /// <returns>The parsed <see cref="Point"/>, or the default value if empty.</returns>
    /// <exception cref="InvalidOperationException">If <paramref name="string"/> does not contain the expected number of components.</exception>
    /// <exception cref="InvalidOperationException">If the conversion fails.</exception>
    public static Point? ParsePoint(this string @string, char separator = ',')
    {
        if (string.IsNullOrEmpty(@string))
        {
            return default(Point);
        }

        var split = @string.Split(separator);
        if (split.Length < 2)
        {
            ThrowHelper.ThrowInvalidOperationException("Insufficient elements after string split.");
        }

        if (split[0].TryParse<int>(out var t) && split[1].TryParse<int>(out var u))
        {
            return new Point(t, u);
        }

        ThrowHelper.ThrowInvalidOperationException($"Failed to parse string {@string}.");
        return null;
    }

    /// <summary>Safely attempts to parse the string to a <see cref="Point"/>, and returns whether the parse was successful.</summary>
    /// <param name="string">The <see cref="string"/>.</param>
    /// <param name="result">The parsed value if successful, or <see langword="null"/> otherwise.</param>
    /// <param name="separator">The element separator.</param>
    /// <returns><see langword="true"/> if the parse was successful, otherwise <see langword="false"/>.</returns>
    public static bool TryParsePoint(this string @string, [NotNullWhen(true)] out Point? result, char separator = ',')
    {
        if (string.IsNullOrEmpty(@string))
        {
            result = null;
            return false;
        }

        var split = @string.Split(separator);
        if (split.Length < 2)
        {
            result = null;
            return false;
        }

        if (split[0].TryParse<int>(out var t) && split[1].TryParse<int>(out var u))
        {
            result = new Point(t, u);
            return true;
        }

        result = null;
        return false;
    }

    /// <summary>
    ///     Splits the string using the provided <paramref name="separator"/> and parses the resulting two components into
    ///     a <see cref="Vector2"/>.
    /// </summary>
    /// <param name="string">The <see cref="string"/>.</param>
    /// <param name="separator">The element separator.</param>
    /// <returns>The parsed <see cref="Vector2"/>, or the default value if empty.</returns>
    /// <exception cref="InvalidOperationException">If <paramref name="string"/> does not contain the expected number of components.</exception>
    /// <exception cref="InvalidOperationException">If the conversion fails.</exception>
    public static Vector2? ParseVector2(this string @string, char separator = ',')
    {
        if (string.IsNullOrEmpty(@string))
        {
            return default(Vector2);
        }

        var split = @string.Split(separator);
        if (split.Length < 2)
        {
            ThrowHelper.ThrowInvalidOperationException("Insufficient elements after string split.");
        }

        if (split[0].TryParse<float>(out var t) && split[1].TryParse<float>(out var u))
        {
            return new Vector2(t, u);
        }

        ThrowHelper.ThrowInvalidOperationException($"Failed to parse string {@string}.");
        return null;
    }

    /// <summary>Safely attempts to parse the string to a <see cref="Vector2"/>, and returns whether the parse was successful.</summary>
    /// <param name="string">The <see cref="string"/>.</param>
    /// <param name="result">The parsed value if successful, or <see langword="null"/> otherwise.</param>
    /// <param name="separator">The element separator.</param>
    /// <returns><see langword="true"/> if the parse was successful, otherwise <see langword="false"/>.</returns>
    public static bool TryParseVector2(this string @string, [NotNullWhen(true)] out Vector2? result, char separator = ',')
    {
        if (string.IsNullOrEmpty(@string))
        {
            result = null;
            return false;
        }

        var split = @string.Split(separator);
        if (split.Length < 2)
        {
            result = null;
            return false;
        }

        if (split[0].TryParse<float>(out var t) && split[1].TryParse<float>(out var u))
        {
            result = new Vector2(t, u);
            return true;
        }

        result = null;
        return false;
    }

    /// <summary>Safely attempts to parse the string to a <see cref="Color"/>, and returns whether the parse was successful.</summary>
    /// <param name="string">The <see cref="string"/>.</param>
    /// <param name="result">The parsed value if successful, or <see langword="null"/> otherwise.</param>
    /// <param name="separator">The element separator.</param>
    /// <returns><see langword="true"/> if the parse was successful, otherwise <see langword="false"/>.</returns>
    public static bool TryParseColor(this string @string, out Color result, char separator = ',')
    {
        result = default;
        if (string.IsNullOrEmpty(@string))
        {
            return false;
        }

        @string = @string.TrimAll();
        return ColorByName.Value.TryGetValue(@string, out result) || @string.TryParseFromHtml(out result) ||
               @string.TryParseFromRgba(out result, separator);
    }

    /// <summary>Attempts to parse an HTML color string to a <see cref="Color"/>.</summary>
    /// <param name="html">An HTML color string.</param>
    /// <param name="result">The parsed <see cref="Color"/>.</param>
    /// <returns><see langword="true"/> if the HTML string was valid, otherwise <see langword="false"/>.</returns>
    public static bool TryParseFromHtml(this string html, out Color result)
    {
        result = default;
        if (html[0] != '#')
        {
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
                return false;
        }

        byte[] array = [255, 255, 255, 255];
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
                return false;
            }

            array[i] = len == 2 ? parsed : (byte)(parsed * 0x11);
        }

        result.R = array[0];
        result.G = array[1];
        result.B = array[2];
        result.A = array[3];
        return true;
    }

    /// <summary>Attempts to parse a RGBA color string to a <see cref="Color"/>.</summary>
    /// <param name="rgba">A RGBA color string.</param>
    /// <param name="result">The parsed <see cref="Color"/>.</param>
    /// <param name="separator">The channel separator.</param>
    /// <returns><see langword="true"/> if the RGBA string was valid, otherwise <see langword="false"/>.</returns>
    public static bool TryParseFromRgba(this string rgba, out Color result, char separator = ',')
    {
        result = default;
        var split = rgba.SplitWithoutAllocation(separator);
        if (split.Length < 3)
        {
            return false;
        }

        var bytes = new byte[split.Length == 4 ? 4 : 3];
        for (var i = 0; i < bytes.Length; i++)
        {
            if (byte.TryParse(split[i], out var parsed))
            {
                bytes[i] = parsed;
            }
            else
            {
                return false;
            }
        }

        result = bytes.Length == 4
            ? new Color(bytes[0], bytes[1], bytes[2], bytes[3])
            : new Color(bytes[0], bytes[1], bytes[2]);
        return true;
    }
}
