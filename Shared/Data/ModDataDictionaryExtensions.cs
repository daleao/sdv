namespace DaLion.Shared.Data;

#region using directives

using System.Text;
using DaLion.Shared.Extensions;
using StardewValley.Mods;

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
    /// <param name="value">The value to write, or <see langword="null"/> to remove the key.</param>
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

    /// <summary>Gets a <see cref="string"/> representation of the <see cref="ModDataDictionary"/>.</summary>
    /// <param name="data">The <see cref="ModDataDictionary"/>.</param>
    /// <param name="predicate">An optional condition with which to filter out data entry keys.</param>
    /// <returns>A <see cref="string"/> representation of the <see cref="ModDataDictionary"/>.</returns>
    public static string ToDebugString(this ModDataDictionary data, Func<string, bool>? predicate = null)
    {
        StringBuilder sb = new();
        foreach (var (key, value) in data.Pairs)
        {
            if (predicate is null || predicate(key))
            {
                sb.Append("\n\t- " + key + " = " + value);
            }
        }

        return sb.ToString();
    }
}
