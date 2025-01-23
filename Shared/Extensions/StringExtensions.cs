namespace DaLion.Shared.Extensions;

#region using directives

using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using DaLion.Shared.Extensions.Memory;

#endregion using directives

/// <summary>Extensions for the <see cref="string"/> primitive type.</summary>
public static class StringExtensions
{
    private static readonly Regex SWhitespace = new(@"\s+", RegexOptions.Compiled);

    /// <summary>Replaces the <see cref="char"/>s starting at the specified <paramref name="position"/> with those from the specified <paramref name="newValue"/>.</summary>
    /// <param name="string">The <see cref="string"/>.</param>
    /// <param name="position">The <see cref="char"/> index.</param>
    /// <param name="newValue">A replacement <see cref="string"/>.</param>
    /// <returns>A new <see cref="string"/> where the <c>N</c> <see cref="char"/>s starting at <paramref name="position"/> are replaced with <paramref name="newValue"/>, where <c>N</c> is the length of <paramref name="newValue"/>.</returns>
    public static string ReplaceAt(this string @string, int position, string newValue)
    {
        if (string.IsNullOrEmpty(newValue))
        {
            return @string;
        }

        if (position + newValue.Length > @string.Length)
        {
            ThrowHelper.ThrowArgumentOutOfRangeException();
        }

        return @string.Remove(position, newValue.Length).Insert(position, newValue);
    }

    /// <summary>Determines whether the string contains any of the specified sub-strings.</summary>
    /// <param name="string">The <see cref="string"/>.</param>
    /// <param name="candidates">The sub-strings to search for.</param>
    /// <returns><see langword="true"/> if <paramref name="string"/> contains at least one of the <see cref="string"/>s in <paramref name="candidates"/>, otherwise <see langword="false"/>.</returns>
    public static bool ContainsAnyOf(this string @string, params string[] candidates)
    {
        return candidates.Any(@string.Contains);
    }

    /// <summary>Determines whether the string contains all the specified sub-strings.</summary>
    /// <param name="string">The <see cref="string"/>.</param>
    /// <param name="candidates">The sub-strings to search for.</param>
    /// <returns><see langword="true"/> if <paramref name="string"/> contains all of the <see cref="string"/>s in <paramref name="candidates"/>, otherwise <see langword="false"/>.</returns>
    public static bool ContainsAllOf(this string @string, params string[] candidates)
    {
        return candidates.All(@string.Contains);
    }

    /// <summary>Determines whether the string starts with any of the specified sub-strings.</summary>
    /// <param name="string">The <see cref="string"/>.</param>
    /// <param name="candidates">The sub-strings to check.</param>
    /// <returns><see langword="true"/> if <paramref name="string"/> begins with at least one of the <see cref="string"/>s in <paramref name="candidates"/>, otherwise <see langword="false"/>.</returns>
    public static bool StartsWithAnyOf(this string @string, params string[] candidates)
    {
        return candidates.Any(@string.StartsWith);
    }

    /// <summary>Finds the index of the <paramref name="n"/>th occurrence of the character <paramref name="ch"/> in the string.</summary>
    /// <param name="string">The <see cref="string"/>.</param>
    /// <param name="ch">The <see cref="char"/> to find.</param>
    /// <param name="n">The occurrence number.</param>
    /// <param name="start">The starting index for the search within the string.</param>
    /// <returns>The integer index of the <paramref name="n"/>th occurrence of the character <paramref name="ch"/>.</returns>
    public static int NthIndexOf(this string @string, char ch, int n, int start = 0)
    {
        var idx = @string.IndexOf(ch, start);
        while (idx >= 0 && --n > 0)
        {
            idx = @string.IndexOf(ch, start + idx + 1);
        }

        return idx;
    }

    /// <summary>Capitalizes the first character in the string.</summary>
    /// <param name="string">The <see cref="string"/>.</param>
    /// <returns>A new <see cref="string"/> with a capitalized first character.</returns>
    public static string FirstCharToUpper(this string @string)
    {
        return string.IsNullOrEmpty(@string)
            ? @string
            : char.ToUpper(@string[0]) + @string[1..];
    }

    /// <summary>Gets the index of the first whitespace character.</summary>
    /// <param name="string">The <see cref="string"/>.</param>
    /// <returns>The index of the first whitespace character, or -1 if not found.</returns>
    public static int IndexOfWhitespace(this string @string)
    {
        return @string.AsSpan().IndexOfWhitespace();
    }

    /// <summary>Removes invalid file name or path characters from the string.</summary>
    /// <param name="string">The <see cref="string"/>.</param>
    /// <returns>A new <see cref="string"/> formed by filtering any invalid file name or path characters from the original.</returns>
    public static string RemoveInvalidFileNameOrPathChars(this string @string)
    {
        var invalidChars = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
        return new Regex($"[{Regex.Escape(invalidChars)}]").Replace(@string, string.Empty);
    }

    /// <summary>Splits a <c>camelCase</c> or <c>PascalCase</c> token into an array of its constituent words.</summary>
    /// <param name="string">The <see cref="string"/>.</param>
    /// <returns>An array of <see cref="string"/>s pertaining to the individual words in <paramref name="string"/>.</returns>
    public static string[] SplitCamelCase(this string @string)
    {
        return Regex
            .Split(@string, @"([A-Z]+|[A-Z]?[a-z]+)(?=[A-Z]|\b)")
            .Where(s => !string.IsNullOrEmpty(s))
            .ToArray();
    }

    /// <summary>Inserts spaces in between words in a <c>camelCase</c> or <c>PascalCase</c> token.</summary>
    /// <param name="string">The <see cref="string"/>.</param>
    /// <returns>The token parsed into words.</returns>
    public static string ParseCamelCase(this string @string)
    {
        return Regex.Replace(
            Regex.Replace(
                @string,
                @"(\P{Ll})(\P{Ll}\p{Ll})",
                "$1 $2"),
            @"(\p{Ll})(\P{Ll})",
            "$1 $2");
    }

    /// <summary>Convert the <c>camelCase</c> or <c>PascalCase</c> token into a <c>snake_case</c> token.</summary>
    /// <param name="string">The <see cref="string"/>.</param>
    /// <returns>The <c>camelCase</c> token parsed into words.</returns>
    public static string CamelToSnakeCase(this string @string)
    {
        return Regex.Replace(
            Regex.Replace(
                @string,
                "(.)([A-Z][a-z]+)",
                "$1_$2"),
            "([a-z0-9])([A-Z])",
            "$1_$2").ToLower();
    }

    /// <summary>Trims all whitespace from the string.</summary>
    /// <param name="str">The <see cref="string"/>.</param>
    /// <returns>The original <see cref="string"/> trimmed of any whitespace characters.</returns>
    public static string TrimAll(this string str)
    {
        return SWhitespace.Replace(str, string.Empty);
    }

    /// <summary>Truncates the string to the specified <paramref name="maxLength"/> if necessary, appending the desired <paramref name="truncationSuffix"/>.</summary>
    /// <param name="string">The <see cref="string"/>.</param>
    /// <param name="maxLength">The desired maximum length of the resulting <see cref="string"/>.</param>
    /// <param name="truncationSuffix">A <see cref="string"/> to be appended to the result to signify that truncation has taken place (by default ellipses).</param>
    /// <returns>The original <see cref="string"/> if it is shorter than <paramref name="maxLength"/>, or truncated after <paramref name="maxLength"/> characters and appended with <paramref name="truncationSuffix"/> otherwise.</returns>
    public static string Truncate(this string @string, int maxLength, string truncationSuffix = "…")
    {
        if (!string.IsNullOrEmpty(truncationSuffix))
        {
            maxLength -= 1;
        }

        return @string.Length > maxLength
            ? @string[..maxLength] + truncationSuffix
            : @string;
    }

    /// <summary>Parses the string to a generic type.</summary>
    /// <typeparam name="T">The expected type. This should most likely be a primitive.</typeparam>
    /// <param name="string">The <see cref="string"/>.</param>
    /// <returns>A value of type <typeparamref name="T"/> converted from <paramref name="string"/>, or <see langword="default"/>(<typeparamref name="T"/>) if empty.</returns>
    /// <exception cref="InvalidOperationException">If the conversion fails.</exception>
    public static T? Parse<T>(this string @string)
    {
        if (string.IsNullOrEmpty(@string))
        {
            return default(T);
        }

        var converter = TypeDescriptor.GetConverter(typeof(T));
        if (!converter.CanConvertTo(typeof(T)) || !converter.CanConvertFrom(typeof(string)))
        {
            ThrowHelper.ThrowInvalidOperationException("Cannot convert string to the specified type.");
        }

        return (T?)converter.ConvertFromString(@string);
    }

    /// <summary>Safely attempts to parse the string to a generic type, and returns whether the parse was successful.</summary>
    /// <typeparam name="T">The expected type. This should most likely be a primitive.</typeparam>
    /// <param name="string">The <see cref="string"/>.</param>
    /// <param name="result">The parsed value if successful, or default otherwise.</param>
    /// <returns><see langword="true"/> if the parse was successful, otherwise <see langword="false"/>.</returns>
    public static bool TryParse<T>(this string @string, [NotNullWhen(true)] out T? result)
    {
        result = default;
        if (string.IsNullOrEmpty(@string))
        {
            return false;
        }

        var converter = TypeDescriptor.GetConverter(typeof(T));
        if (!converter.IsValid(@string))
        {
            return false;
        }

        try
        {
            result = (T?)converter.ConvertFromString(@string);
            return result is not null;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    ///     Splits the string using the provided <paramref name="separator"/> and parses the resulting two components into
    ///     different types.
    /// </summary>
    /// <typeparam name="T1">The expected type of the first component. This should most likely be a primitive.</typeparam>
    /// <typeparam name="T2">The expected type of the second component. This should most likely be a primitive.</typeparam>
    /// <param name="string">The <see cref="string"/>.</param>
    /// <param name="separator">The element separator.</param>
    /// <returns>The parsed components of <paramref name="string"/>, or the default values if empty.</returns>
    /// <exception cref="InvalidOperationException">If <paramref name="string"/> does not contain the expected number of components.</exception>
    /// <exception cref="InvalidOperationException">If the conversion fails.</exception>
    public static (T1?, T2?)? ParseTuple<T1, T2>(this string @string, char separator = ',')
    {
        if (string.IsNullOrEmpty(@string))
        {
            return (default(T1), default(T2));
        }

        var split = @string.Split(separator);
        if (split.Length < 2)
        {
            ThrowHelper.ThrowInvalidOperationException("Insufficient elements after string split.");
        }

        if (split[0].TryParse<T1>(out var t) && split[1].TryParse<T2>(out var u))
        {
            return (t, u);
        }

        ThrowHelper.ThrowInvalidOperationException($"Failed to parse string {@string}.");
        return null;
    }

    /// <summary>
    ///     Splits the string using the provided <paramref name="separator"/> and parses the resulting three components into
    ///     different types.
    /// </summary>
    /// <typeparam name="T1">The expected type of the first component. This should most likely be a primitive.</typeparam>
    /// <typeparam name="T2">The expected type of the second component. This should most likely be a primitive.</typeparam>
    /// <typeparam name="T3">The expected type of the third component. This should most likely be a primitive.</typeparam>
    /// <param name="string">The <see cref="string"/>.</param>
    /// <param name="separator">The element separator.</param>
    /// <returns>The parsed components of <paramref name="string"/>.</returns>
    /// <exception cref="InvalidOperationException">If <paramref name="string"/> does not contain the expected number of components.</exception>
    /// <exception cref="InvalidOperationException">If the conversion fails.</exception>
    public static (T1?, T2?, T3?)? ParseTuple<T1, T2, T3>(this string @string, char separator = ',')
    {
        if (string.IsNullOrEmpty(@string))
        {
            return (default(T1), default(T2), default(T3));
        }

        var split = @string.Split(separator);
        if (split.Length < 3)
        {
            ThrowHelper.ThrowInvalidOperationException("Insufficient elements after string split.");
        }

        if (split[0].TryParse<T1>(out var t) && split[1].TryParse<T2>(out var u) && split[2].TryParse<T3>(out var v))
        {
            return (t, u, v);
        }

        ThrowHelper.ThrowInvalidOperationException($"Failed to parse string {@string}.");
        return null;
    }

    /// <summary>
    ///     Splits the string using the provided <paramref name="separator"/> and parses the resulting four components into
    ///     different types.
    /// </summary>
    /// <typeparam name="T1">The expected type of the first component. This should most likely be a primitive.</typeparam>
    /// <typeparam name="T2">The expected type of the second component. This should most likely be a primitive.</typeparam>
    /// <typeparam name="T3">The expected type of the third component. This should most likely be a primitive.</typeparam>
    /// <typeparam name="T4">The expected type of the fourth component. This should most likely be a primitive.</typeparam>
    /// <param name="string">The <see cref="string"/>.</param>
    /// <param name="separator">The element separator.</param>
    /// <returns>The parsed components of <paramref name="string"/>.</returns>
    /// <exception cref="InvalidOperationException">If <paramref name="string"/> does not contain the expected number of components.</exception>
    /// <exception cref="InvalidOperationException">If the conversion fails.</exception>
    public static (T1?, T2?, T3?, T4?)? ParseTuple<T1, T2, T3, T4>(this string @string, char separator = ',')
    {
        if (string.IsNullOrEmpty(@string))
        {
            return (default(T1), default(T2), default(T3), default(T4));
        }

        var split = @string.Split(separator);
        if (split.Length < 4)
        {
            ThrowHelper.ThrowInvalidOperationException("Insufficient elements after string split.");
        }

        if (split[0].TryParse<T1>(out var t) && split[1].TryParse<T2>(out var u) && split[2].TryParse<T3>(out var v) &&
            split[3].TryParse<T4>(out var w))
        {
            return (t, u, v, w);
        }

        ThrowHelper.ThrowInvalidOperationException($"Failed to parse string {@string}.");
        return null;
    }

    /// <summary>
    ///     Splits the string using the provided <paramref name="separator"/> and parses the resulting elements into a
    ///     <see cref="List{T}"/>.
    /// </summary>
    /// <typeparam name="T">The expected type of the elements. This should most likely be a primitive.</typeparam>
    /// <param name="string">The <see cref="string"/>.</param>
    /// <param name="separator">The element separator.</param>
    /// <returns>A <see cref="List{T}"/> containing the parsed elements of <paramref name="string"/>.</returns>
    /// <exception cref="InvalidOperationException">If the conversion fails.</exception>
    public static List<T?> ParseList<T>(this string @string, char separator = ',')
    {
        if (string.IsNullOrEmpty(@string))
        {
            return [];
        }

        var split = @string.Split(separator);
        var list = new List<T?>();
        foreach (var s in split)
        {
            if (s.TryParse<T>(out var parsed))
            {
                list.Add(parsed);
                continue;
            }

            ThrowHelper.ThrowInvalidOperationException($"Failed to parse string {@string}.");
        }

        return list;
    }

    /// <summary>Parses a flattened string of key-value pairs back into a <see cref="Dictionary{TKey,TValue}"/>.</summary>
    /// <typeparam name="TKey">The expected type of the dictionary keys. This should most likely be a primitive.</typeparam>
    /// <typeparam name="TValue">The expected type of the dictionary values. This should most likely be a primitive.</typeparam>
    /// <param name="string">The <see cref="string"/>.</param>
    /// <param name="keyValueSeparator">The sub-string that separates keys and values.</param>
    /// <param name="pairSeparator">The sub-string that separates key-value pairs.</param>
    /// <returns>A <see cref="Dictionary{TKey,TValue}"/> containing the parsed <see cref="KeyValuePair{TKey,TValue}"/>s.</returns>
    /// <exception cref="ArgumentException">If <paramref name="keyValueSeparator"/> and <paramref name="pairSeparator"/> are equal.</exception>
    /// <exception cref="InvalidOperationException">If the conversion fails.</exception>
    public static Dictionary<TKey, TValue> ParseDictionary<TKey, TValue>(
        this string @string, char keyValueSeparator = ',', char pairSeparator = ';')
        where TKey : notnull
    {
        if (string.IsNullOrEmpty(@string))
        {
            return new Dictionary<TKey, TValue>();
        }

        if (pairSeparator == keyValueSeparator)
        {
            ThrowHelper.ThrowArgumentException("Pair separator must be different from key-value separator.");
        }

        var pairs = @string
            .Split(new[] { pairSeparator }, StringSplitOptions.RemoveEmptyEntries)
            .Select(p => p.Split(new[] { keyValueSeparator }, StringSplitOptions.RemoveEmptyEntries));

        var dict = new Dictionary<TKey, TValue>();
        foreach (var pair in pairs)
        {
            if (pair[0].TryParse<TKey>(out var key) && !dict.ContainsKey(key) &&
                pair[1].TryParse<TValue>(out var value))
            {
                dict[key] = value;
                continue;
            }

            ThrowHelper.ThrowInvalidOperationException($"Failed to parse string {@string}.");
        }

        return dict;
    }

    /// <summary>Splits a <see cref="string"/> into its constituent substrings based on the specified <paramref name="splitter"/>, without additional memory allocation.</summary>
    /// <param name="string">The <see cref="string"/>.</param>
    /// <param name="splitter">A <see cref="char"/>s that will be used to split the <paramref name="string"/>.</param>
    /// <returns>A <see cref="SpanSplitter"/> object that can be used to iterate through and access the substrings within the <paramref name="string"/>, without additional memory allocation.</returns>
    public static SpanSplitter SplitWithoutAllocation(this string @string, char splitter)
    {
        return @string.AsSpan().Split(splitter);
    }

    /// <summary>Converts the string into a hash code that is reliable across different executions.</summary>
    /// <param name="string">The <see cref="string"/>.</param>
    /// <returns>A reproducible <see cref="int"/> hash of <paramref name="string"/>.</returns>
    public static int GetDeterministicHashCode(this string @string)
    {
        unchecked
        {
            var hash1 = (5381 << 16) + 5381;
            var hash2 = hash1;
            for (var i = 0; i < @string.Length; i += 2)
            {
                hash1 = ((hash1 << 5) + hash1) ^ @string[i];
                if (i == @string.Length - 1)
                {
                    break;
                }

                hash2 = ((hash2 << 5) + hash2) ^ @string[i + 1];
            }

            return hash1 + (hash2 * 1566083941);
        }
    }
}
