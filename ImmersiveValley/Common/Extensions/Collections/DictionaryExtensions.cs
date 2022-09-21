namespace DaLion.Common.Extensions.Collections;

#region using directives

using System;
using System.Collections.Generic;
using System.Linq;
using CommunityToolkit.Diagnostics;

#endregion using directives

/// <summary>Extensions for the <see cref="Dictionary{TKey,TValue}"/> class.</summary>
public static class DictionaryExtensions
{
    /// <summary>Flattens pairs in the <paramref name="dictionary"/> into a single string.</summary>
    /// <typeparam name="TKey">The type of keys in the <paramref name="dictionary"/>.</typeparam>
    /// <typeparam name="TValue">The type of values in the <paramref name="dictionary"/>.</typeparam>
    /// <param name="dictionary">The <see cref="Dictionary{TKey,TValue}"/>.</param>
    /// <param name="keyValueSeparator">The <see cref="string"/> with which to separate keys and values.</param>
    /// <param name="pairSeparator">The <see cref="string"/> with which to separate <see cref="KeyValuePair{TKey,TValue}"/>s.</param>
    /// <returns>The entire contents of the <paramref name="dictionary"/> as one <see cref="string"/>.</returns>
    public static string Stringify<TKey, TValue>(
        this Dictionary<TKey, TValue> dictionary, string keyValueSeparator = ",", string pairSeparator = ";")
        where TKey : notnull
    {
        if (string.IsNullOrEmpty(keyValueSeparator) || string.IsNullOrEmpty(pairSeparator))
        {
            ThrowHelper.ThrowArgumentException("Separator cannot be null or empty.");
        }

        if (pairSeparator == keyValueSeparator)
        {
            ThrowHelper.ThrowArgumentException("Pair separator must be different from key-value separator.");
        }

        return dictionary.Count <= 0
            ? string.Empty
            : string.Join(pairSeparator, dictionary.Select(p => $"{p.Key}{keyValueSeparator}{p.Value}"));
    }

    /// <summary>
    ///     Adds the <paramref name="value"/> to the <paramref name="dictionary"/> if the corresponding
    ///     <paramref name="key"/> is not yet present, or update the current value according to the some
    ///     <paramref name="aggregator"/>.
    /// </summary>
    /// <typeparam name="TKey">The type of keys in the <paramref name="dictionary"/>.</typeparam>
    /// <typeparam name="TValue">The type of values in the <paramref name="dictionary"/>.</typeparam>
    /// <param name="dictionary">The <see cref="Dictionary{TKey,TValue}"/>.</param>
    /// <param name="key">The key.</param>
    /// <param name="value">They value.</param>
    /// <param name="aggregator">A function that defines how items should be aggregated in case the key does exit.</param>
    public static void AddOrUpdate<TKey, TValue>(
        this IDictionary<TKey, TValue> dictionary, TKey key, TValue value, Func<TValue, TValue, TValue> aggregator)
        where TKey : notnull
    {
        if (!dictionary.TryAdd(key, value))
        {
            dictionary[key] = aggregator(dictionary[key], value);
        }
    }

    /// <summary>
    ///     Adds the <paramref name="pair"/> to the <paramref name="dictionary"/> if its key is not yet present, or
    ///     update the current value according to the specified <paramref name="aggregator"/>.
    /// </summary>
    /// <typeparam name="TKey">The type of keys in the <paramref name="dictionary"/>.</typeparam>
    /// <typeparam name="TValue">The type of values in the <paramref name="dictionary"/>.</typeparam>
    /// <param name="dictionary">The <see cref="Dictionary{TKey,TValue}"/>.</param>
    /// <param name="pair">A <see cref="KeyValuePair{TKey,TValue}"/>.</param>
    /// <param name="aggregator">A function that defines how items should be aggregated in case the pair's key does exit.</param>
    public static void AddOrUpdate<TKey, TValue>(
        this IDictionary<TKey, TValue> dictionary, KeyValuePair<TKey, TValue> pair, Func<TValue, TValue, TValue> aggregator)
        where TKey : notnull
    {
        dictionary.AddOrUpdate(pair.Key, pair.Value, aggregator);
    }

    /// <summary>Gets the key with the highest value in the <paramref name="dictionary"/>.</summary>
    /// <typeparam name="TKey">The type of keys in the <paramref name="dictionary"/>.</typeparam>
    /// <typeparam name="TValue">The type of values in the <paramref name="dictionary"/>.</typeparam>
    /// <param name="dictionary">The <see cref="Dictionary{TKey,TValue}"/>.</param>
    /// <returns>The key corresponding to the highest value in the <paramref name="dictionary"/>.</returns>
    public static TKey MaxKey<TKey, TValue>(this IDictionary<TKey, TValue> dictionary)
        where TValue : IComparable<TValue>
    {
        return dictionary.Aggregate((a, b) => a.Value.CompareTo(b.Value) > 0 ? a : b).Key;
    }

    /// <summary>Gets the key with the lowest value in the <paramref name="dictionary"/>.</summary>
    /// <typeparam name="TKey">The type of keys in the <paramref name="dictionary"/>.</typeparam>
    /// <typeparam name="TValue">The type of values in the <paramref name="dictionary"/>, which should implement <see cref="IComparable{T}"/>.</typeparam>
    /// <param name="dictionary">The <see cref="Dictionary{TKey,TValue}"/>.</param>
    /// <returns>The key corresponding to the lowest value in the <paramref name="dictionary"/>.</returns>
    public static TKey MinKey<TKey, TValue>(this IDictionary<TKey, TValue> dictionary)
        where TValue : IComparable<TValue>
    {
        return dictionary.Aggregate((a, b) => a.Value.CompareTo(b.Value) <= 0 ? a : b).Key;
    }
}
