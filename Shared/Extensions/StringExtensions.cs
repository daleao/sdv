namespace DaLion.Shared.Extensions;

#region using directives

using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

#endregion using directives

/// <summary>Extensions for the <see cref="string"/> primitive type.</summary>
public static class StringExtensions
{
    private static readonly Regex SWhitespace = new(@"\s+");

    /// <summary>Determines whether the string contains any of the specified sub-strings.</summary>
    /// <param name="str">The <see cref="string"/>.</param>
    /// <param name="candidates">The sub-strings to search for.</param>
    /// <returns><see langword="true"/> if <paramref name="str"/> contains at least one of the <see cref="string"/>s in <paramref name="candidates"/>, otherwise <see langword="false"/>.</returns>
    public static bool ContainsAnyOf(this string str, params string[] candidates)
    {
        return candidates.Any(str.Contains);
    }

    /// <summary>Determines whether the string contains all of the specified sub-strings.</summary>
    /// <param name="str">The <see cref="string"/>.</param>
    /// <param name="candidates">The sub-strings to search for.</param>
    /// <returns><see langword="true"/> if <paramref name="str"/> contains all of the <see cref="string"/>s in <paramref name="candidates"/>, otherwise <see langword="false"/>.</returns>
    public static bool ContainsAllOf(this string str, params string[] candidates)
    {
        return candidates.All(str.Contains);
    }

    /// <summary>Determines whether the string starts with any of the specified sub-strings.</summary>
    /// <param name="str">The <see cref="string"/>.</param>
    /// <param name="candidates">The sub-strings to check.</param>
    /// <returns><see langword="true"/> if <paramref name="str"/> begins with at least one of the <see cref="string"/>s in <paramref name="candidates"/>, otherwise <see langword="false"/>.</returns>
    public static bool StartsWithAnyOf(this string str, params string[] candidates)
    {
        return candidates.Any(str.StartsWith);
    }

    /// <summary>Capitalizes the first character in the string.</summary>
    /// <param name="str">The <see cref="string"/>.</param>
    /// <returns>A new <see cref="string"/> with a capitalized first character.</returns>
    public static string FirstCharToUpper(this string str)
    {
        return string.IsNullOrEmpty(str)
            ? str
            : char.ToUpper(str[0]) + str[1..];
    }

    /// <summary>Removes invalid file name or path characters from the string.</summary>
    /// <param name="str">The <see cref="string"/>.</param>
    /// <returns>A new <see cref="string"/> formed by filtering any invalid file name or path characters from the original.</returns>
    public static string RemoveInvalidChars(this string str)
    {
        var invalidChars = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
        return new Regex($"[{Regex.Escape(invalidChars)}]").Replace(str, string.Empty);
    }

    /// <summary>Splits a <c>camelCase</c> or <c>PascalCase</c> string into its constituent words.</summary>
    /// <param name="str">The <see cref="string"/>.</param>
    /// <returns>An array of <see cref="string"/>s pertaining to the individual words in <paramref name="str"/>.</returns>
    public static string[] SplitCamelCase(this string str)
    {
        return Regex.Split(str, @"([A-Z]+|[A-Z]?[a-z]+)(?=[A-Z]|\b)").Where(r => !string.IsNullOrEmpty(r))
            .ToArray();
    }

    /// <summary>Trims all whitespace from the string.</summary>
    /// <param name="str">The <see cref="string"/>.</param>
    /// <returns>The original <see cref="string"/> trimmed of any whitespace characters.</returns>
    public static string TrimAll(this string str)
    {
        return SWhitespace.Replace(str, string.Empty);
    }

    /// <summary>Truncates the string to the specified <paramref name="maxLength"/> if necessary, appending the desired <paramref name="truncationSuffix"/>.</summary>
    /// <param name="str">The <see cref="string"/>.</param>
    /// <param name="maxLength">The desired maximum length of the resulting <see cref="string"/>.</param>
    /// <param name="truncationSuffix">A <see cref="string"/> to be appended to the result to signify that truncation has taken place (by default ellipses).</param>
    /// <returns>The original <see cref="string"/> if it is shorter than <paramref name="maxLength"/>, or truncated after <paramref name="maxLength"/> characters and appended with <paramref name="truncationSuffix"/> otherwise.</returns>
    public static string Truncate(this string str, int maxLength, string truncationSuffix = "…")
    {
        if (!string.IsNullOrEmpty(truncationSuffix))
        {
            maxLength -= 1;
        }

        return str.Length > maxLength
            ? str[..maxLength] + truncationSuffix
            : str;
    }

    /// <summary>Parses the string to a generic type.</summary>
    /// <typeparam name="T">The expected type. This should most likely be a primitive.</typeparam>
    /// <param name="str">The <see cref="string"/>.</param>
    /// <returns>A value of type <typeparamref name="T"/> converted from <paramref name="str"/>, or <see langword="default"/>(<typeparamref name="T"/>) if empty.</returns>
    /// <exception cref="InvalidOperationException">If the conversion fails.</exception>
    public static T? Parse<T>(this string str)
    {
        if (string.IsNullOrEmpty(str))
        {
            return default(T);
        }

        var converter = TypeDescriptor.GetConverter(typeof(T));
        if (!converter.CanConvertTo(typeof(T)) || !converter.CanConvertFrom(typeof(string)))
        {
            ThrowHelper.ThrowInvalidOperationException("Cannot convert string to the specified type.");
        }

        return (T)converter.ConvertFromString(str);
    }

    /// <summary>Safely attempts to parse the string to a generic type, and returns whether the parse was successful.</summary>
    /// <typeparam name="T">The expected type. This should most likely be a primitive.</typeparam>
    /// <param name="str">The <see cref="string"/>.</param>
    /// <param name="result">The parsed value if successful, or default otherwise.</param>
    /// <returns><see langword="true"/> if the parse was successful, otherwise <see langword="false"/>.</returns>
    public static bool TryParse<T>(this string str, [NotNullWhen(true)] out T? result)
    {
        result = default(T?);
        if (string.IsNullOrEmpty(str))
        {
            return false;
        }

        var converter = TypeDescriptor.GetConverter(typeof(T));
        if (!converter.IsValid(str))
        {
            return false;
        }

        try
        {
            result = (T)converter.ConvertFromString(str);
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>Converts the string into a hash code that is reliable across different executions.</summary>
    /// <param name="str">The <see cref="string"/>.</param>
    /// <returns>A reproducible <see cref="int"/> hash of <paramref name="str"/>.</returns>
    public static int GetDeterministicHashCode(this string str)
    {
        unchecked
        {
            var hash1 = (5381 << 16) + 5381;
            var hash2 = hash1;
            for (var i = 0; i < str.Length; i += 2)
            {
                hash1 = ((hash1 << 5) + hash1) ^ str[i];
                if (i == str.Length - 1)
                {
                    break;
                }

                hash2 = ((hash2 << 5) + hash2) ^ str[i + 1];
            }

            return hash1 + (hash2 * 1566083941);
        }
    }

    /// <summary>
    ///     Splits the string using the provided <paramref name="separator"/> and parses the resulting two components into
    ///     different types.
    /// </summary>
    /// <typeparam name="T1">The expected type of the first component. This should most likely be a primitive.</typeparam>
    /// <typeparam name="T2">The expected type of the second component. This should most likely be a primitive.</typeparam>
    /// <param name="str">The <see cref="string"/>.</param>
    /// <param name="separator">The element separator.</param>
    /// <returns>The parsed components of <paramref name="str"/>, or the default values if empty.</returns>
    /// <exception cref="InvalidOperationException">If <paramref name="str"/> does not contain the expected number of components.</exception>
    /// <exception cref="InvalidOperationException">If the conversion fails.</exception>
    public static (T1, T2)? ParseTuple<T1, T2>(this string str, string separator = ",")
        where T1 : struct
        where T2 : struct
    {
        if (string.IsNullOrEmpty(str))
        {
            return (default(T1), default(T2));
        }

        var split = str.Split(separator);
        if (split.Length < 2)
        {
            ThrowHelper.ThrowInvalidOperationException("Insufficient elements after string split.");
        }

        if (split[0].TryParse<T1>(out var t) && split[1].TryParse<T2>(out var u))
        {
            return (t, u);
        }

        ThrowHelper.ThrowInvalidOperationException($"Failed to parse string {str}.");
        return null;
    }

    /// <summary>
    ///     Splits the string using the provided <paramref name="separator"/> and parses the resulting three components into
    ///     different types.
    /// </summary>
    /// <typeparam name="T1">The expected type of the first component. This should most likely be a primitive.</typeparam>
    /// <typeparam name="T2">The expected type of the second component. This should most likely be a primitive.</typeparam>
    /// <typeparam name="T3">The expected type of the third component. This should most likely be a primitive.</typeparam>
    /// <param name="str">The <see cref="string"/>.</param>
    /// <param name="separator">The element separator.</param>
    /// <returns>The parsed components of <paramref name="str"/>.</returns>
    /// <exception cref="InvalidOperationException">If <paramref name="str"/> does not contain the expected number of components.</exception>
    /// <exception cref="InvalidOperationException">If the conversion fails.</exception>
    public static (T1, T2, T3)? ParseTuple<T1, T2, T3>(this string str, string separator = ",")
        where T1 : struct
        where T2 : struct
        where T3 : struct
    {
        if (string.IsNullOrEmpty(str))
        {
            return (default(T1), default(T2), default(T3));
        }

        var split = str.Split(separator);
        if (split.Length < 3)
        {
            ThrowHelper.ThrowInvalidOperationException("Insufficient elements after string split.");
        }

        if (split[0].TryParse<T1>(out var t) && split[1].TryParse<T2>(out var u) && split[2].TryParse<T3>(out var v))
        {
            return (t, u, v);
        }

        ThrowHelper.ThrowInvalidOperationException($"Failed to parse string {str}.");
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
    /// <param name="str">The <see cref="string"/>.</param>
    /// <param name="separator">The element separator.</param>
    /// <returns>The parsed components of <paramref name="str"/>.</returns>
    /// <exception cref="InvalidOperationException">If <paramref name="str"/> does not contain the expected number of components.</exception>
    /// <exception cref="InvalidOperationException">If the conversion fails.</exception>
    public static (T1, T2, T3, T4)? ParseTuple<T1, T2, T3, T4>(this string str, string separator = ",")
        where T1 : struct
        where T2 : struct
        where T3 : struct
        where T4 : struct
    {
        if (string.IsNullOrEmpty(str))
        {
            return (default(T1), default(T2), default(T3), default(T4));
        }

        var split = str.Split(separator);
        if (split.Length < 4)
        {
            ThrowHelper.ThrowInvalidOperationException("Insufficient elements after string split.");
        }

        if (split[0].TryParse<T1>(out var t) && split[1].TryParse<T2>(out var u) && split[2].TryParse<T3>(out var v) &&
            split[3].TryParse<T4>(out var w))
        {
            return (t, u, v, w);
        }

        ThrowHelper.ThrowInvalidOperationException($"Failed to parse string {str}.");
        return null;
    }

    /// <summary>
    ///     Splits the string using the provided <paramref name="separator"/> and parses the resulting elements into a
    ///     <see cref="List{T}"/>.
    /// </summary>
    /// <typeparam name="T">The expected type of the elements. This should most likely be a primitive.</typeparam>
    /// <param name="str">The <see cref="string"/>.</param>
    /// <param name="separator">The element separator.</param>
    /// <returns>A <see cref="List{T}"/> containing the parsed elements of <paramref name="str"/>.</returns>
    /// <exception cref="InvalidOperationException">If the conversion fails.</exception>
    public static List<T?> ParseList<T>(this string str, string separator = ",")
    {
        if (string.IsNullOrEmpty(str))
        {
            return new List<T?>();
        }

        var split = str.Split(separator);
        var list = new List<T?>();
        foreach (var item in split)
        {
            if (item.TryParse<T>(out var parsed))
            {
                list.Add(parsed);
                continue;
            }

            ThrowHelper.ThrowInvalidOperationException($"Failed to parse string {str}.");
        }

        return list;
    }

    /// <summary>Parses a flattened string of key-value pairs back into a <see cref="Dictionary{TKey,TValue}"/>.</summary>
    /// <typeparam name="TKey">The expected type of the dictionary keys. This should most likely be a primitive.</typeparam>
    /// <typeparam name="TValue">The expected type of the dictionary values. This should most likely be a primitive.</typeparam>
    /// <param name="str">The <see cref="string"/>.</param>
    /// <param name="keyValueSeparator">The sub-string that separates keys and values.</param>
    /// <param name="pairSeparator">The sub-string that separates key-value pairs.</param>
    /// <returns>A <see cref="Dictionary{TKey,TValue}"/> containing the parsed <see cref="KeyValuePair{TKey,TValue}"/>s.</returns>
    /// <exception cref="ArgumentException">If <paramref name="keyValueSeparator"/> and <paramref name="pairSeparator"/> are equal.</exception>
    /// <exception cref="InvalidOperationException">If the conversion fails.</exception>
    public static Dictionary<TKey, TValue> ParseDictionary<TKey, TValue>(
        this string str, string keyValueSeparator = ",", string pairSeparator = ";")
        where TKey : notnull
    {
        if (string.IsNullOrEmpty(str))
        {
            return new Dictionary<TKey, TValue>();
        }

        if (pairSeparator == keyValueSeparator)
        {
            ThrowHelper.ThrowArgumentException("Pair separator must be different from key-value separator.");
        }

        var pairs = str
            .Split(new[] { pairSeparator }, StringSplitOptions.RemoveEmptyEntries)
            .Select(p => p.Split(new[] { keyValueSeparator }, StringSplitOptions.RemoveEmptyEntries));

        var dict = new Dictionary<TKey, TValue>();
        foreach (var p in pairs)
        {
            if (p[0].TryParse<TKey>(out var key) && !dict.ContainsKey(key) && p[1].TryParse<TValue>(out var value))
            {
                dict[key] = value;
                continue;
            }

            ThrowHelper.ThrowInvalidOperationException($"Failed to parse string {str}.");
        }

        return dict;
    }
}
