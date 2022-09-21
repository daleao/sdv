namespace DaLion.Common.Extensions.Stardew;

#region using directives

using DaLion.Common.ModData;

#endregion using directives

/// <summary>Extensions for the <see cref="SObject"/> class.</summary>
public static class ItemExtensions
{
    /// <inheritdoc cref="ModDataIO.Read(Item, string, string, string)"/>
    public static string Read(this Item item, string field, string defaultValue = "", string modId = "")
    {
        return ModDataIO.Read(item, field, defaultValue, modId);
    }

    /// <inheritdoc cref="ModDataIO.Read{T}(Item, string, T, string)"/>
    public static T Read<T>(this Item item, string field, T defaultValue = default, string modId = "")
        where T : struct
    {
        return ModDataIO.Read(item, field, defaultValue, modId);
    }

    /// <inheritdoc cref="ModDataIO.Write(Item, string, string?)"/>
    public static void Write(this Item item, string field, string? value)
    {
        ModDataIO.Write(item, field, value);
    }

    /// <inheritdoc cref="ModDataIO.WriteIfNotExists(Item, string, string?)"/>
    public static void WriteIfNotExists(this Item item, string field, string? value)
    {
        ModDataIO.WriteIfNotExists(item, field, value);
    }

    /// <inheritdoc cref="ModDataIO.Append(Item, string, string, string)"/>
    public static void Append(this Item item, string field, string value, string separator = ",")
    {
        ModDataIO.Append(item, field, value, separator);
    }

    /// <inheritdoc cref="ModDataIO.Increment{T}(Item, string, T)"/>
    public static void Increment<T>(this Item item, string field, T amount)
        where T : struct
    {
        ModDataIO.Increment(item, field, amount);
    }

    /// <inheritdoc cref="ModDataIO.Increment{T}(Item, string)"/>
    public static void Increment(this Item item, string field)
    {
        ModDataIO.Increment(item, field, 1);
    }
}
