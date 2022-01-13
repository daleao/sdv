using System;
using System.Linq.Expressions;
using StardewValley;

namespace DaLion.Stardew.Common.Extensions;

/// <summary>Provides extension methods for reading and writing values in <see cref="ModDataDictionary" /> fields.</summary>
public static class ModDataDictionaryExtensions
{
    /// <summary>Read a value from the <see cref="ModDataDictionary" /> as string.</summary>
    /// <param name="data">The <see cref="ModDataDictionary" />.</param>
    /// <param name="key">The dictionary key to read from.</param>
    /// <param name="defaultValue">The default value to return if the key does not exist.</param>
    /// <returns>The value of the specified key if it exists, or a default value if it doesn't.</returns>
    public static string Read(this ModDataDictionary data, string key, string defaultValue = null)
    {
        return data.TryGetValue(key, out var rawValue)
            ? rawValue
            : defaultValue;
    }

    /// <summary>Read a value from the <see cref="ModDataDictionary" /> and try to parse it as type <typeparamref name="T" />.</summary>
    /// <param name="data">The <see cref="ModDataDictionary" />.</param>
    /// <param name="key">The dictionary key to read from.</param>
    /// <param name="defaultValue">The default value to return if the key does not exist.</param>
    /// <returns>
    ///     The value of the specified key if it exists, parsed as type <typeparamref name="T" />, or a default value if
    ///     the key doesn't exist or fails to parse.
    /// </returns>
    public static T ReadAs<T>(this ModDataDictionary data, string key, T defaultValue = default)
    {
        return data.TryGetValue(key, out var rawValue) && rawValue.TryParse(out T parsedValue)
            ? parsedValue
            : defaultValue;
    }

    /// <summary>Write a value to the <see cref="ModDataDictionary" />, or remove the key if supplied with null.</summary>
    /// <param name="data">The <see cref="ModDataDictionary" />.</param>
    /// <param name="key">The dictionary key to write to.</param>
    /// <param name="value">The value to write, or <c>null</c> to remove the key.</param>
    /// <returns>Interface to <paramref name="data" />.</returns>
    public static ModDataDictionary Write(this ModDataDictionary data, string key, string value)
    {
        if (string.IsNullOrWhiteSpace(value)) data.Remove(key);
        else data[key] = value;
        return data;
    }

    /// <summary>Write a value to the <see cref="ModDataDictionary" /> only if the key does not yet exist.</summary>
    /// <param name="data">The <see cref="ModDataDictionary" />.</param>
    /// <param name="key">The dictionary key to write to.</param>
    /// <param name="value">The value to write.</param>
    /// <returns>Interface to <paramref name="data" />.</returns>
    public static ModDataDictionary WriteIfNotExists(this ModDataDictionary data, string key, string value)
    {
        if (!data.ContainsKey(key)) data[key] = value;
        return data;
    }

    /// <summary>Write a value to the <see cref="ModDataDictionary" /> only if the key does not yet exist.</summary>
    /// <param name="data">The <see cref="ModDataDictionary" />.</param>
    /// <param name="key">The dictionary key to write to.</param>
    /// <param name="value">The value to write.</param>
    /// <param name="exists">Whether the key already existed in the dictionary.</param>
    /// <returns>Interface to <paramref name="data" />.</returns>
    public static ModDataDictionary WriteIfNotExists(this ModDataDictionary data, string key, string value,
        out bool exists)
    {
        exists = data.ContainsKey(key);
        if (!exists) data[key] = value;
        return data;
    }

    /// <summary>Increment a numeric value in the <see cref="ModDataDictionary" />.</summary>
    /// <param name="data">The <see cref="ModDataDictionary" />.</param>
    /// <param name="key">The dictionary key to read update.</param>
    /// <param name="amount">Amount to increment by.</param>
    /// <returns>Interface to <paramref name="data" />.</returns>
    /// <remarks>Credit to <c>Adi Lester</c> (https://stackoverflow.com/questions/8122611/c-sharp-adding-two-generic-values).</remarks>
    public static ModDataDictionary Increment<T>(this ModDataDictionary data, string key, T amount)
    {
        var num = data.ReadAs<T>(key);

        // declare the parameters
        var paramA = Expression.Parameter(typeof(T), "a");
        var paramB = Expression.Parameter(typeof(T), "b");

        // add the parameters together
        var body = Expression.Add(paramA, paramB);

        // compile it
        var add = Expression.Lambda<Func<T, T, T>>(body, paramA, paramB).Compile();

        // call it
        data[key] = add(num, amount).ToString();

        return data;
    }
}