namespace DaLion.Common.Extensions.Collections;

#region using directives

using System;
using System.Collections.Generic;
using System.Linq;

#endregion using directives

/// <summary>Extensions for the <see cref="Dictionary{TKey,TValue}"/> class.</summary>
public static class DictionaryExtensions
{
    /// <summary>Flatten pairs in a <see cref="Dictionary{TKey,TValue}" /> into a single string.</summary>
    /// <param name="keyValueSeparator">String inserted between key and value.</param>
    /// <param name="pairSeparator">String inserted between pairs.</param>
    public static string Stringify<TKey, TValue>(this Dictionary<TKey, TValue> d, string keyValueSeparator = ",",
        string pairSeparator = ";") where TKey : notnull
    {
        if (string.IsNullOrEmpty(keyValueSeparator) || string.IsNullOrEmpty(pairSeparator))
            throw new ArgumentException("Separator cannot be null or empty.");

        if (pairSeparator == keyValueSeparator)
            throw new ArgumentException("Pair separator must be different from key-value separator.");

        return d.Count <= 0 ? string.Empty : string.Join(pairSeparator, d.Select(p => $"{p.Key}{keyValueSeparator}{p.Value}"));
    }

    /// <summary>Add the value to the dictionary if the key is not yet present, or update the current value according to the specified aggregator function.</summary>
    /// <param name="key">The key.</param>
    /// <param name="value">They value.</param>
    /// <param name="aggregator">A function that defines how items should be aggregated in case the key does exit.</param>
    public static void AddOrUpdate<TKey, TValue>(this IDictionary<TKey, TValue> d, TKey key, TValue value,
        Func<TValue, TValue, TValue> aggregator) where TKey : notnull
    {
        if (!d.TryAdd(key, value)) d[key] = aggregator(d[key], value);
    }

    /// <summary>Add the specified pair to the dictionary if its key is not yet present, or update the current value according to the specified aggregator function.</summary>
    /// <param name="pair">A <see cref="KeyValuePair{TKey,TValue}"/>.</param>
    /// <param name="aggregator">A function that defines how items should be aggregated in case the pair's key does exit.</param>
    public static void AddOrUpdate<TKey, TValue>(this IDictionary<TKey, TValue> d, KeyValuePair<TKey, TValue> pair,
        Func<TValue, TValue, TValue> aggregator) where TKey : notnull
    {
        d.AddOrUpdate(pair.Key, pair.Value, aggregator);
    }
}