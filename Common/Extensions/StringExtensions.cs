#nullable enable
namespace DaLion.Stardew.Common.Extensions;

#region using directives

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

#endregion using directives

public static class StringExtensions
{
    /// <summary>Determine if the calling string contains any of the specified substrings.</summary>
    /// <param name="candidates">A sequence of strings candidates.</param>
    public static bool ContainsAnyOf(this string s, params string[] candidates)
    {
        return candidates.Any(s.Contains);
    }

    /// <summary>Determine if the calling string starts with any of the specified substrings.</summary>
    /// <param name="candidates">A sequence of strings candidates.</param>
    public static bool StartsWithAnyOf(this string s, params string[] candidates)
    {
        return candidates.Any(s.StartsWith);
    }

    /// <summary>Capitalize the first character in the calling string.</summary>
    public static string FirstCharToUpper(this string s)
    {
        return string.IsNullOrEmpty(s)
            ? throw new ArgumentException("Argument is null or empty.")
            : s.First().ToString().ToUpper() + s[1..];
    }

    /// <summary>Removes invalid file name or path characters from the calling string.</summary>
    public static string RemoveInvalidChars(this string s)
    {
        var invalidChars = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
        return new Regex($"[{Regex.Escape(invalidChars)}]").Replace(s, string.Empty);
    }

    /// <summary>Split a camelCase or PascalCase string into its constituent words.</summary>
    public static string[] SplitCamelCase(this string s)
    {
        return Regex.Split(s, @"([A-Z]+|[A-Z]?[a-z]+)(?=[A-Z]|\b)").Where(r => !string.IsNullOrEmpty(r)).ToArray();
    }

    /// <summary>Truncate the calling string to a <paramref name="maxLength" />, ending with elipses.</summary>
    public static string Truncate(this string s, int maxLength, string truncationSuffix = "…")
    {
        return s.Length > maxLength
            ? s[..maxLength] + truncationSuffix
            : s;
    }

    /// <summary>Parse the calling string to a generic type.</summary>
    public static T Parse<T>(this string s)
    {
        if (s is null) throw new ArgumentNullException();

        var converter = TypeDescriptor.GetConverter(typeof(T));
        if (converter.CanConvertTo(typeof(T)) && converter.CanConvertFrom(typeof(string)))
            return (T) converter.ConvertFromString(s) ?? throw new InvalidCastException();

        throw new FormatException();
    }

    /// <summary>Try to parse the calling string to a generic type.</summary>
    /// <param name="result">Parsed <typeparamref name="T" />-type object if successful, else default.</param>
    /// <returns>True if parse was successful, otherwise false.</returns>
    public static bool TryParse<T>(this string s, out T? result)
    {
        result = default;
        if (string.IsNullOrEmpty(s)) return false;

        var converter = TypeDescriptor.GetConverter(typeof(T));
        if (!converter.IsValid(s))
            return false;

        try
        {
            result = (T) converter.ConvertFromString(s);
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

    /// <summary>Parse a flattened string of key-value pairs back into a <see cref="Dictionary{TKey,TValue}" />.</summary>
    /// <param name="keyValueSeparator">String that separates keys and values.</param>
    /// <param name="pairSeparator">String that separates pairs.</param>
    public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(this string s, string keyValueSeparator,
        string pairSeparator) where TKey : notnull
    {
        var pairs = s.Split(new[] {pairSeparator}, StringSplitOptions.RemoveEmptyEntries);
        return pairs.Select(p => p.Split(new[] {keyValueSeparator}, StringSplitOptions.RemoveEmptyEntries))
            .ToDictionary(p => p[0].Parse<TKey>(), p => p[1].Parse<TValue>());
    }
}