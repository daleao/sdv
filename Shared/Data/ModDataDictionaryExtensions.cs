namespace DaLion.Shared.ModData;

#region using directives

using System.Linq.Expressions;
using System.Text;
using DaLion.Shared.Extensions;

#endregion using directives

/// <summary>Provides extension methods for reading and writing values in the <see cref="ModDataDictionary"/> class.</summary>
public static class ModDataDictionaryExtensions
{
    /// <summary>Reads a value from the <see cref="ModDataDictionary"/> as a <see cref="string"/>.</summary>
    /// <param name="data">The <see cref="ModDataDictionary"/>.</param>
    /// <param name="key">The dictionary key to read from.</param>
    /// <param name="defaultValue">The default value to return if the <paramref name="key"/> does not exist.</param>
    /// <returns>The value of the specified <paramref name="key"/> if it exists, or <paramref name="defaultValue"/> value if it doesn't.</returns>
    public static string Read(this ModDataDictionary data, string key, string defaultValue = "")
    {
        return data.TryGetValue(key, out var rawValue)
            ? rawValue
            : defaultValue;
    }

    /// <summary>
    ///     Reads a value from the <see cref="ModDataDictionary"/> and tries to parse it as type
    ///     <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The expected value type.</typeparam>
    /// <param name="data">The <see cref="ModDataDictionary"/>.</param>
    /// <param name="key">The dictionary key to read from.</param>
    /// <param name="defaultValue">The default value to return if the key does not exist.</param>
    /// <returns>
    ///     The value of the specified <paramref name="key"/> if it exists, parsed as type <typeparamref name="T"/>, or <paramref name="defaultValue"/> if
    ///     the <paramref name="key"/> doesn't exist or fails to parse.
    /// </returns>
    public static T Read<T>(this ModDataDictionary data, string key, T defaultValue = default)
        where T : struct
    {
        return data.TryGetValue(key, out var rawValue) && rawValue.TryParse(out T parsedValue)
            ? parsedValue
            : defaultValue;
    }

    /// <summary>
    ///     Writes a <see cref="string"/> <paramref name="value"/> to the <see cref="ModDataDictionary"/>, or removes the
    ///     corresponding <paramref name="key"/> if the <paramref name="value"/> is null or empty.
    /// </summary>
    /// <param name="data">The <see cref="ModDataDictionary"/>.</param>
    /// <param name="key">The dictionary key to write to.</param>
    /// <param name="value">The value to write, or <c>null</c> to remove the key.</param>
    /// <returns>The interface to <paramref name="data"/>.</returns>
    public static ModDataDictionary Write(this ModDataDictionary data, string key, string? value)
    {
        if (string.IsNullOrEmpty(value))
        {
            data.Remove(key);
        }
        else
        {
            data[key] = value;
        }

        return data;
    }

    /// <summary>
    ///     Writes a <paramref name="value"/> to the <see cref="ModDataDictionary"/> only if the specified
    ///     <paramref name="key"/> does not yet exist.
    /// </summary>
    /// <param name="data">The <see cref="ModDataDictionary"/>.</param>
    /// <param name="key">The dictionary key to write to.</param>
    /// <param name="value">The value to write.</param>
    /// <returns>The interface to <paramref name="data"/>.</returns>
    public static ModDataDictionary WriteIfNotExists(this ModDataDictionary data, string key, string? value)
    {
        if (!data.ContainsKey(key))
        {
            data[key] = value;
        }

        return data;
    }

    /// <summary>
    ///     Writes a <paramref name="value"/> to the <see cref="ModDataDictionary"/> only if the specified
    ///     <paramref name="key"/> does not yet exist, and returns whether or not it existed.
    /// </summary>
    /// <param name="data">The <see cref="ModDataDictionary"/>.</param>
    /// <param name="key">The dictionary key to write to.</param>
    /// <param name="value">The value to write.</param>
    /// <param name="didExist"><see langword="true"/> if the key did in fact exist in the dictionary, otherwise <see langword="false"/>.</param>
    /// <returns>The interface to <paramref name="data"/>.</returns>
    public static ModDataDictionary WriteIfNotExists(
        this ModDataDictionary data, string key, string? value, out bool didExist)
    {
        didExist = data.ContainsKey(key);
        if (!didExist)
        {
            data[key] = value;
        }

        return data;
    }

    /// <summary>
    ///     Appends a <see cref="string"/> representation of a <paramref name="value"/> to the specified <paramref name="key"/> in
    ///     the <see cref="ModDataDictionary"/>, prefixed with the chosen <paramref name="separator"/>.
    /// </summary>
    /// <typeparam name="T">The type of the <paramref name="value"/>.</typeparam>
    /// <param name="data">The <see cref="ModDataDictionary"/>.</param>
    /// <param name="key">The dictionary key to update.</param>
    /// <param name="value">The value to append.</param>
    /// <param name="separator">The <see cref="string"/> which separates values.</param>
    /// <returns>The interface to <paramref name="data"/>.</returns>
    public static ModDataDictionary Append<T>(this ModDataDictionary data, string key, T value, string separator)
    {
        return data.TryGetValue(key, out var currentValue)
            ? data.Write(key, currentValue + separator + value)
            : data.Write(key, value?.ToString());
    }

    /// <summary>Increments the value at <paramref name="key"/> by a generic <paramref name="amount"/>.</summary>
    /// <typeparam name="T">A numeric value type.</typeparam>
    /// <param name="data">The <see cref="ModDataDictionary"/>.</param>
    /// <param name="key">The dictionary key to update.</param>
    /// <param name="amount">The amount to increment by.</param>
    /// <returns>The interface to <paramref name="data"/>.</returns>
    /// <remarks>
    ///     Original code by
    ///     <see href="https://stackoverflow.com/questions/8122611/c-sharp-adding-two-generic-values">Adi Lester</see>.
    /// </remarks>
    public static ModDataDictionary Increment<T>(this ModDataDictionary data, string key, T amount)
        where T : struct
    {
        var num = data.Read<T>(key);

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

    /// <summary>Gets a <see cref="string"/> representation of the <see cref="ModDataDictionary"/>.</summary>
    /// <param name="data">The <see cref="ModDataDictionary"/>.</param>
    /// <returns>A <see cref="string"/> representation of the <see cref="ModDataDictionary"/>.</returns>
    public static string ToDebugString(this ModDataDictionary data)
    {
        StringBuilder sb = new();
        foreach (var (key, value) in data.Pairs)
        {
            sb.Append("\n\t- " + key + " = " + value);
        }

        return sb.ToString();
    }
}
