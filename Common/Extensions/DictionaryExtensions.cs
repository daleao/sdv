namespace DaLion.Stardew.Common.Extensions;

#region using directives

using System;
using System.Collections.Generic;
using System.Linq;

#endregion using directives

/// <summary>Extensions for the <see cref="Dictionary{TKey,TValue}"/> class.</summary>
public static class DictionaryExtensions
{
    /// <summary>Flatten pairs in a <see cref="Dictionary{TKey,TValue}" /> into a single string.</summary>
    /// <param name="keyValueSeparator">Character inserted between key and value.</param>
    /// <param name="pairSeparator">Character inserted between pairs.</param>
    public static string ToString<TKey, TValue>(this Dictionary<TKey, TValue> d, char keyValueSeparator,
        char pairSeparator)
    {
        if (d is null) throw new ArgumentNullException(nameof(d));
        
        if (pairSeparator == keyValueSeparator)
            throw new ArgumentException("Pair separator must be different from key-value separator.");

        return !d.Any() ? string.Empty : string.Join(pairSeparator, d.Select(p => $"{p.Key}{keyValueSeparator}{p.Value}"));
    }

    /// <summary>Flatten pairs in a <see cref="Dictionary{TKey,TValue}" /> into a single string.</summary>
    /// <param name="keyValueSeparator">String inserted between key and value.</param>
    /// <param name="pairSeparator">String inserted between pairs.</param>
    public static string ToString<TKey, TValue>(this Dictionary<TKey, TValue> d, string keyValueSeparator,
        string pairSeparator)
    {
        if (d is null) throw new ArgumentNullException(nameof(d));

        if (string.IsNullOrEmpty(keyValueSeparator) || string.IsNullOrEmpty(pairSeparator))
            throw new ArgumentException("Separator cannot be null or empty.");
        
        if (pairSeparator == keyValueSeparator)
            throw new ArgumentException("Pair separator must be different from key-value separator.");

        return !d.Any() ? string.Empty : string.Join(pairSeparator, d.Select(p => $"{p.Key}{keyValueSeparator}{p.Value}"));
    }
}