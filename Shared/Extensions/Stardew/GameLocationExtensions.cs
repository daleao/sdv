namespace DaLion.Shared.Extensions.Stardew;

#region using directives

using System.Linq;
using DaLion.Shared.ModData;
using StardewValley.Monsters;

#endregion using directives

/// <summary>Extensions for the <see cref="SObject"/> class.</summary>
public static class GameLocationExtensions
{
    /// <summary>Determines whether this <paramref name="location"/> has spawned enemies.</summary>
    /// <param name="location">The <see cref="GameLocation"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="location"/> is has at least one living monster and is not a <see cref="SlimeHutch"/>, otherwise <see langword="false"/>.</returns>
    public static bool HasMonsters(this GameLocation location)
    {
        return location.characters.OfType<Monster>().Any() && location is not SlimeHutch;
    }

    /// <inheritdoc cref="ModDataIO.Read(GameLocation, string, string, string)"/>
    public static string Read(this GameLocation location, string field, string defaultValue = "", string modId = "")
    {
        return ModDataIO.Read(location, field, defaultValue, modId);
    }

    /// <inheritdoc cref="ModDataIO.Read{T}(GameLocation, string, T, string)"/>
    public static T Read<T>(this GameLocation location, string field, T defaultValue = default, string modId = "")
        where T : struct
    {
        return ModDataIO.Read(location, field, defaultValue, modId);
    }

    /// <inheritdoc cref="ModDataIO.Write(GameLocation, string, string?)"/>
    public static void Write(this GameLocation location, string field, string? value)
    {
        ModDataIO.Write(location, field, value);
    }

    /// <inheritdoc cref="ModDataIO.WriteIfNotExists(GameLocation, string, string?)"/>
    public static void WriteIfNotExists(this GameLocation location, string field, string? value)
    {
        ModDataIO.WriteIfNotExists(location, field, value);
    }

    /// <inheritdoc cref="ModDataIO.Append(GameLocation, string, string, string)"/>
    public static void Append(this GameLocation location, string field, string value, string separator = ",")
    {
        ModDataIO.Append(location, field, value, separator);
    }

    /// <inheritdoc cref="ModDataIO.Increment{T}(GameLocation, string, T)"/>
    public static void Increment<T>(this GameLocation location, string field, T amount)
        where T : struct
    {
        ModDataIO.Increment(location, field, amount);
    }

    /// <inheritdoc cref="ModDataIO.Increment{T}(GameLocation, string, T)"/>
    public static void Increment(this GameLocation location, string field)
    {
        ModDataIO.Increment(location, field, 1);
    }
}
