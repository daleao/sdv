namespace DaLion.Stardew.FishPonds.Framework.Extensions;

#region using directives

using StardewValley;
using StardewValley.Buildings;

using Common.Extensions;

#endregion using directives

/// <summary>Extensions for the <see cref="Building"/> class.</summary>
internal static class BuildingExtensions
{
    /// <summary>Read a string from this building's <see cref="ModDataDictionary" />.</summary>
    /// <param name="field">The field to read from.</param>
    /// <param name="defaultValue">The default value to return if the field does not exist.</param>
    internal static string ReadData(this Building building, string field, string defaultValue = "")
    {
        return building.modData.Read($"{ModEntry.Manifest.UniqueID}/{field}", defaultValue);
    }

    /// <summary>Read a field from this building's <see cref="ModDataDictionary" /> as <typeparamref name="T" />.</summary>
    /// <param name="field">The field to read from.</param>
    /// <param name="defaultValue"> The default value to return if the field does not exist.</param>
    internal static T ReadDataAs<T>(this Building building, string field, T defaultValue = default)
    {
        return building.modData.ReadAs($"{ModEntry.Manifest.UniqueID}/{field}", defaultValue);
    }

    /// <summary>Write to a field in this building's <see cref="ModDataDictionary" />, or remove the field if supplied with a null or empty value.</summary>
    /// <param name="field">The field to write to.</param>
    /// <param name="value">The value to write, or <c>null</c> to remove the field.</param>
    internal static void WriteData(this Building building, string field, string value)
    {
        building.modData.Write($"{ModEntry.Manifest.UniqueID}/{field}", value);
        Log.D($"[ModData]: Wrote {value} to {building.nameOfIndoors}'s {field}.");
    }

    /// <summary>Write to a field in this building's <see cref="ModDataDictionary" />, only if it doesn't yet have a value.</summary>
    /// <param name="field">The field to write to.</param>
    /// <param name="value">The value to write, or <c>null</c> to remove the field.</param>
    internal static bool WriteDataIfNotExists(this Building building, string field, string value)
    {
        if (building.modData.ContainsKey($"{ModEntry.Manifest.UniqueID}/{field}"))
        {
            Log.D($"[ModData]: The data field {field} already existed.");
            return true;
        }
        
        building.WriteData(field, value);
        return false;
    }

    /// <summary>Increment the value of a numeric field in this building's <see cref="ModDataDictionary" /> by an arbitrary amount.</summary>
    /// <param name="field">The field to update.</param>
    /// <param name="amount">Amount to increment by.</param>
    internal static void IncrementData<T>(this Building building, string field, T amount)
    {
        building.modData.Increment($"{ModEntry.Manifest.UniqueID}/{field}", amount);
        Log.D($"[ModData]: Incremented {building.nameOfIndoors}'s {field} by {amount}.");
    }

    /// <summary>Increment the value of a numeric field in this building's <see cref="ModDataDictionary" /> by 1.</summary>
    /// <param name="field">The field to update.</param>
    internal static void IncrementData<T>(this Building building, string field)
    {
        building.modData.Increment($"{ModEntry.Manifest.UniqueID}/{field}",
            "1".Parse<T>());
        Log.D($"[ModData]: Incremented {building.nameOfIndoors}'s {field} by 1.");
    }
}