namespace DaLion.Common.Extensions;

#region using directives

using System;
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
    private static readonly Regex _sWhitespace = new(@"\s+");

    /// <summary>Determine if the string instance contains any of the specified substrings.</summary>
    /// <param name="candidates">A sequence of strings candidates.</param>
    public static bool ContainsAnyOf(this string s, params string[] candidates) =>
        candidates.Any(s.Contains);

    /// <summary>Determine if the string instance starts with any of the specified substrings.</summary>
    /// <param name="candidates">A sequence of strings candidates.</param>
    public static bool StartsWithAnyOf(this string s, params string[] candidates) =>
        candidates.Any(s.StartsWith);

    /// <summary>Determine if the string instance contains all of the specified substrings.</summary>
    /// <param name="candidates">A sequence of strings candidates.</param>
    public static bool ContainsAllOf(this string s, params string[] candidates) =>
        candidates.All(s.Contains);

    /// <summary>Capitalize the first character in the string instance.</summary>
    public static string FirstCharToUpper(this string s) =>
        string.IsNullOrEmpty(s)
            ? s
            : char.ToUpper(s[0]) + s[1..];

    /// <summary>Removes invalid file name or path characters from the string instance.</summary>
    public static string RemoveInvalidChars(this string s)
    {
        var invalidChars = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
        return new Regex($"[{Regex.Escape(invalidChars)}]").Replace(s, string.Empty);
    }

    /// <summary>Split a camelCase or PascalCase string into its constituent words.</summary>
    public static string[] SplitCamelCase(this string s) =>
        Regex.Split(s, @"([A-Z]+|[A-Z]?[a-z]+)(?=[A-Z]|\b)").Where(r => !string.IsNullOrEmpty(r)).ToArray();

    /// <summary>Trim all whitespace from the string.</summary>
    public static string TrimAll(this string s) =>
        _sWhitespace.Replace(s, "");

    /// <summary>Truncate the string instance to a <paramref name="maxLength" />, ending with ellipses.</summary>
    public static string Truncate(this string s, int maxLength, string truncationSuffix = "…") =>
        s.Length > maxLength
            ? s[..maxLength] + truncationSuffix
            : s;

    /// <summary>Parse the string instance to a generic type.</summary>
    public static T? Parse<T>(this string s)
    {
        if (string.IsNullOrEmpty(s)) ThrowHelper.ThrowArgumentException("Cannot parse null or empty string.");

        var converter = TypeDescriptor.GetConverter(typeof(T));
        if (!converter.CanConvertTo(typeof(T)) || !converter.CanConvertFrom(typeof(string)))
            ThrowHelper.ThrowInvalidOperationException("Cannot convert string to the specified type.");

        return (T)converter.ConvertFromString(s);
    }

    /// <summary>Try to parse the instance to a generic type.</summary>
    /// <param name="result">Parsed <typeparamref name="T" />-type object if successful, else default.</param>
    /// <returns><see langword="true"/> if parse was successful, otherwise <see langword="false"/>.</returns>
    public static bool TryParse<T>(this string s, [NotNullWhen(true)] out T? result)
    {
        result = default;
        if (string.IsNullOrEmpty(s)) return false;

        var converter = TypeDescriptor.GetConverter(typeof(T));
        if (!converter.IsValid(s))
            return false;

        try
        {
            result = (T)converter.ConvertFromString(s);
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>Converts a string into a hash code that is reliable across different executions.</summary>
    public static int GetDeterministicHashCode(this string s)
    {
        unchecked
        {
            var hash1 = (5381 << 16) + 5381;
            var hash2 = hash1;
            for (var i = 0; i < s.Length; i += 2)
            {
                hash1 = ((hash1 << 5) + hash1) ^ s[i];
                if (i == s.Length - 1)
                    break;
                hash2 = ((hash2 << 5) + hash2) ^ s[i + 1];
            }

            return hash1 + hash2 * 1566083941;
        }
    }

    /// <summary>Split the string with provided <paramref name="separator"/> and parse the resulting elements into a tuple.</summary>
    /// <param name="separator">A string separator.</param>
    public static (T, U)? ParseTuple<T, U>(this string s, string separator = ",")
        where T : struct
        where U : struct
    {
        if (string.IsNullOrEmpty(s)) return null;

        var split = s.Split(separator);
        if (split.Length < 2)
            ThrowHelper.ThrowInvalidOperationException("Insufficient elements after string split.");

        if (split[0].TryParse<T>(out var t) && split[1].TryParse<U>(out var u)) return (t, u);

        ThrowHelper.ThrowInvalidOperationException($"Failed to parse string {s}.");
        return null;
    }

    /// <summary>Split the string with provided <paramref name="separator"/> and parse the resulting elements into a tuple.</summary>
    /// <param name="separator">A string separator.</param>
    public static (T, U, V)? ParseTuple<T, U, V>(this string s, string separator = ",")
        where T : struct
        where U : struct
        where V : struct
    {
        if (string.IsNullOrEmpty(s)) return null;

        var split = s.Split(separator);
        if (split.Length < 3)
            ThrowHelper.ThrowInvalidOperationException("Insufficient elements after string split.");

        if (split[0].TryParse<T>(out var t) && split[1].TryParse<U>(out var u) && split[2].TryParse<V>(out var v))
            return (t, u, v);

        ThrowHelper.ThrowInvalidOperationException($"Failed to parse string {s}.");
        return null;
    }

    /// <summary>Split the string with provided <paramref name="separator"/> and parse the resulting elements into a tuple.</summary>
    /// <param name="separator">A string separator.</param>
    public static (T, U, V, W)? ParseTuple<T, U, V, W>(this string s, string separator = ",")
        where T : struct
        where U : struct
        where V : struct
        where W : struct
    {
        if (string.IsNullOrEmpty(s)) return null;

        var split = s.Split(separator);
        if (split.Length < 4)
            ThrowHelper.ThrowInvalidOperationException("Insufficient elements after string split.");

        if (split[0].TryParse<T>(out var t) && split[1].TryParse<U>(out var u) && split[2].TryParse<V>(out var v) &&
            split[3].TryParse<W>(out var w)) return (t, u, v, w);

        ThrowHelper.ThrowInvalidOperationException($"Failed to parse string {s}.");
        return null;
    }

    /// <summary>Split the string with provided <paramref name="separator"/> and parse the resulting elements into a list.</summary>
    /// <param name="separator">A string separator.</param>
    public static List<T?> ParseList<T>(this string s, string separator = ",")
    {
        if (string.IsNullOrEmpty(s)) return new();

        var split = s.Split(separator);
        var list = new List<T?>();
        foreach (var item in split)
        {
            if (item.TryParse<T>(out var parsed))
            {
                list.Add(parsed);
                continue;
            }

            ThrowHelper.ThrowInvalidOperationException($"Failed to parse string {s}.");
        }

        return list;
    }

    /// <summary>Parse a flattened string of key-value pairs back into a <see cref="Dictionary{TKey,TValue}" />.</summary>
    /// <param name="keyValueSeparator">String that separates keys and values.</param>
    /// <param name="pairSeparator">String that separates pairs.</param>
    public static Dictionary<TKey, TValue> ParseDictionary<TKey, TValue>(this string s, string keyValueSeparator = ",",
        string pairSeparator = ";") where TKey : notnull
    {
        if (string.IsNullOrEmpty(s)) return new();

        if (pairSeparator == keyValueSeparator)
            ThrowHelper.ThrowArgumentException("Pair separator must be different from key-value separator.");

        var pairs = s
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

            ThrowHelper.ThrowInvalidOperationException($"Failed to parse string {s}.");
        }

        return dict;
    }
}