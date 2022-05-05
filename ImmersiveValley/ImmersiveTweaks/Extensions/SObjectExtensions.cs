namespace DaLion.Stardew.Tweaks.Extensions;

#region using directives

using StardewValley;

using Common.Extensions;
using Common.Extensions.Stardew;

using SObject = StardewValley.Object;

#endregion using directives

/// <summary>Extensions for the <see cref="SObject"/> class.</summary>
public static class SObjectExtensions
{
   /// <summary>Whether a given object is a bee house.</summary>
    public static bool IsBeeHouse(this SObject @object)
    {
        return @object.bigCraftable.Value && @object.ParentSheetIndex == 10;
    }

    /// <summary>Whether a given object is a mushroom box.</summary>
    public static bool IsMushroomBox(this SObject @object)
    {
       return @object.bigCraftable.Value && @object.ParentSheetIndex == 128;
    }

    /// <summary>Get an object quality value based on this object's age.</summary>
    public static int GetQualityFromAge(this SObject @object)
    {
        var skillFactor = 1f + Game1.player.ForagingLevel * 0.1f;
        var age = (int) (@object.ReadDataAs<int>("Age") * skillFactor * ModEntry.Config.AgeImproveQualityFactor);
        if (ModEntry.HasProfessionsMod && Game1.player.professions.Contains(Farmer.shepherd)) age *= 2;

        if (ModEntry.Config.DeterministicAgeQuality)
        {
            return age switch
            {
                >= 336 => SObject.bestQuality,
                >= 224 => SObject.highQuality,
                >= 112 => SObject.medQuality,
                _ => SObject.lowQuality
            };
        }

        return age switch
        {
            >= 336 => SObject.bestQuality,
            >= 224 => SObject.highQuality,
            >= 112 => SObject.medQuality,
            _ => SObject.lowQuality
        };
    }

    /// <summary>Read a string from this object's <see cref="ModDataDictionary" />.</summary>
    /// <param name="field">The field to read from.</param>
    /// <param name="defaultValue">The default value to return if the field does not exist.</param>
    public static string ReadData(this SObject @object, string field, string defaultValue = "")
    {
        return @object.modData.Read($"{ModEntry.Manifest.UniqueID}/{field}", defaultValue);
    }

    /// <summary>Read a field from this object's <see cref="ModDataDictionary" /> as <typeparamref name="T" />.</summary>
    /// <param name="field">The field to read from.</param>
    /// <param name="defaultValue"> The default value to return if the field does not exist.</param>
    public static T ReadDataAs<T>(this SObject @object, string field, T defaultValue = default)
    {
        return @object.modData.ReadAs($"{ModEntry.Manifest.UniqueID}/{field}", defaultValue);
    }

    /// <summary>Write to a field in this object's <see cref="ModDataDictionary" />, or remove the field if supplied with a null or empty value.</summary>
    /// <param name="field">The field to write to.</param>
    /// <param name="value">The value to write, or <c>null</c> to remove the field.</param>
    public static void WriteData(this SObject @object, string field, string value)
    {
        @object.modData.Write($"{ModEntry.Manifest.UniqueID}/{field}", value);
        Log.D($"[ModData]: Wrote {value} to {@object.Name}'s {field}.");
    }

    /// <summary>Write to a field in this object's <see cref="ModDataDictionary" />, only if it doesn't yet have a value.</summary>
    /// <param name="field">The field to write to.</param>
    /// <param name="value">The value to write, or <c>null</c> to remove the field.</param>
    public static bool WriteDataIfNotExists(this SObject @object, string field, string value)
    {
        if (@object.modData.ContainsKey($"{ModEntry.Manifest.UniqueID}/{field}"))
        {
            Log.D($"[ModData]: The data field {field} already existed.");
            return true;
        }

        @object.WriteData(field, value);
        return false;
    }

    /// <summary>Increment the value of a numeric field in this object's <see cref="ModDataDictionary" /> by an arbitrary amount.</summary>
    /// <param name="field">The field to update.</param>
    /// <param name="amount">Amount to increment by.</param>
    public static void IncrementData<T>(this SObject @object, string field, T amount)
    {
        @object.modData.Increment($"{ModEntry.Manifest.UniqueID}/{field}", amount);
        Log.D($"[ModData]: Incremented {@object.Name}'s {field} by {amount}.");
    }

    /// <summary>Increment the value of a numeric field in this object's <see cref="ModDataDictionary" /> by 1.</summary>
    /// <param name="field">The field to update.</param>
    public static void IncrementData<T>(this SObject @object, string field)
    {
        @object.modData.Increment($"{ModEntry.Manifest.UniqueID}/{field}",
            "1".Parse<T>());
        Log.D($"[ModData]: Incremented {@object.Name}'s {field} by 1.");
    }
}