using System;
using System.Collections.Generic;
using System.Linq;

namespace DaLion.Stardew.Common.Extensions;

public static class DictionaryExtensions
{
    /// <summary>Flatten pairs in a <see cref="Dictionary{TKey,TValue}" /> into a single string.</summary>
    /// <param name="keyValueSeparator">String inserted between key and value.</param>
    /// <param name="pairSeparator">String inserted between pairs.</param>
    public static string ToString<TKey, TValue>(this Dictionary<TKey, TValue> d, string keyValueSeparator,
        string pairSeparator)
    {
        if (d is null) throw new ArgumentNullException(nameof(d));

        if (!d.Any()) return string.Empty;

        if (string.IsNullOrEmpty(keyValueSeparator) || string.IsNullOrEmpty(pairSeparator))
            throw new ArgumentException("Separator cannot be null or empty.");

        return string.Join(pairSeparator, d.Select(p => $"{p.Key}{keyValueSeparator}{p.Value}"));
    }
}