namespace DaLion.Stardew.Professions.Framework.Extensions;

#region using directives

using System.Collections.Generic;
using System.Linq;
using StardewModdingAPI.Utilities;
using StardewValley;

using Common.Extensions;

using ObjectLookups = Utility.ObjectLookups;
using SObject = StardewValley.Object;

#endregion using directives

/// <summary>Extensions for the <see cref="SObject"/> class.</summary>
internal static class SObjectExtensions
{
    /// <summary>Whether a given object is an artisan good.</summary>
    internal static bool IsArtisanGood(this SObject @object)
    {
        return @object.Category == SObject.artisanGoodsCategory || @object.ParentSheetIndex == 395; // exception for coffee
    }

    /// <summary>Whether a given object is an artisan good.</summary>
    internal static bool IsArtisanMachine(this SObject @object)
    {
        return ObjectLookups.ArtisanMachines.Contains(@object?.name);
    }

    /// <summary>Whether a given object is an animal produce or derived artisan good.</summary>
    internal static bool IsAnimalProduct(this SObject @object)
    {
        return @object.Category.IsAnyOf(SObject.EggCategory, SObject.MilkCategory, SObject.meatCategory, SObject.sellAtPierresAndMarnies)
               || ObjectLookups.AnimalDerivedProductIds.Contains(@object.ParentSheetIndex);
    }

    /// <summary>Whether a given object is a bee house.</summary>
    internal static bool IsBeeHouse(this SObject @object)
    {
        return @object.bigCraftable.Value && @object.ParentSheetIndex == 10;
    }

    /// <summary>Whether a given object is a mushroom box.</summary>
    internal static bool IsMushroomBox(this SObject @object)
    {
        return @object.bigCraftable.Value && @object.ParentSheetIndex == 128;
    }

    /// <summary>Whether a given object is salmonberry or blackberry.</summary>
    internal static bool IsWildBerry(this SObject @object)
    {
        return @object.ParentSheetIndex is 296 or 410;
    }

    /// <summary>Whether a given object is a spring onion.</summary>
    internal static bool IsSpringOnion(this SObject @object)
    {
        return @object.ParentSheetIndex == 399;
    }

    /// <summary>Whether a given object is a gem or mineral.</summary>
    internal static bool IsGemOrMineral(this SObject @object)
    {
        return @object.Category.IsAnyOf(SObject.GemCategory, SObject.mineralsCategory);
    }

    /// <summary>Whether a given object is a foraged mineral.</summary>
    internal static bool IsForagedMineral(this SObject @object)
    {
        return @object.Name.IsAnyOf("Quartz", "Earth Crystal", "Frozen Tear", "Fire Quartz");
    }

    /// <summary>Whether a given object is a resource node or foraged mineral.</summary>
    internal static bool IsResourceNode(this SObject @object)
    {
        return ObjectLookups.ResourceNodeIds.Contains(@object.ParentSheetIndex);
    }

    /// <summary>Whether a given object is a stone.</summary>
    internal static bool IsStone(this SObject @object)
    {
        return @object.Name == "Stone";
    }

    /// <summary>Whether a given object is an artifact spot.</summary>
    internal static bool IsArtifactSpot(this SObject @object)
    {
        return @object.ParentSheetIndex == 590;
    }

    /// <summary>Whether a given object is a fish caught with a fishing rod.</summary>
    internal static bool IsFish(this SObject @object)
    {
        return @object.Category == SObject.FishCategory;
    }

    /// <summary>Whether a given object is a crab pot fish.</summary>
    internal static bool IsTrapFish(this SObject @object)
    {
        return Game1.content.Load<Dictionary<int, string>>(PathUtilities.NormalizeAssetName("Data/Fish"))
            .TryGetValue(@object.ParentSheetIndex, out var fishData) && fishData.Contains("trap");
    }

    /// <summary>Whether a given object is a legendary fish.</summary>
    internal static bool IsLegendaryFish(this SObject @object)
    {
        return ObjectLookups.LegendaryFishNames.Contains(@object.Name);
    }

    /// <summary>Whether a given object is algae or seaweed.</summary>
    internal static bool IsAlgae(this SObject @object)
    {
        return @object.ParentSheetIndex is 152 or 153 or 157;
    }

    /// <summary>Whether a given object is trash.</summary>
    internal static bool IsTrash(this SObject @object)
    {
        return @object.Category == SObject.junkCategory;
    }

    /// <summary>Whether a given object is typically found in pirate treasure.</summary>
    internal static bool IsPirateTreasure(this SObject @object)
    {
        return ObjectLookups.TrapperPirateTreasureTable.ContainsKey(@object.ParentSheetIndex);
    }

    /// <summary>Whether the player should track a given object.</summary>
    internal static bool ShouldBeTracked(this SObject @object)
    {
        return Game1.player.HasProfession(Profession.Scavenger) &&
               (@object.IsSpawnedObject && !@object.IsForagedMineral() || @object.IsSpringOnion() || @object.IsArtifactSpot())
               || Game1.player.HasProfession(Profession.Prospector) &&
               (@object.IsStone() && @object.IsResourceNode() || @object.IsForagedMineral());
    }

    /// <summary>Whether the owner of this instance has the specified profession.</summary>
    /// <param name="profession">Some profession.</param>
    internal static bool DoesOwnerHaveProfession(this SObject @object, Profession profession, bool prestiged = false)
    {
        var owner = Game1.getFarmerMaybeOffline(@object.owner.Value) ?? Game1.MasterPlayer;
        return owner.professions.Contains((int) profession + (prestiged ? 100 : 0));
    }

    /// <summary>Get an object quality value based on this object's age.</summary>
    internal static int GetQualityFromAge(this SObject @object)
    {
        var age = @object.ReadDataAs<int>("Age");
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
    internal static string ReadData(this SObject @object, string field, string defaultValue = "")
    {
        return @object.modData.Read($"{ModEntry.Manifest.UniqueID}/{field}", defaultValue);
    }

    /// <summary>Read a field from this object's <see cref="ModDataDictionary" /> as <typeparamref name="T" />.</summary>
    /// <param name="field">The field to read from.</param>
    /// <param name="defaultValue"> The default value to return if the field does not exist.</param>
    internal static T ReadDataAs<T>(this SObject @object, string field, T defaultValue = default)
    {
        return @object.modData.ReadAs($"{ModEntry.Manifest.UniqueID}/{field}", defaultValue);
    }

    /// <summary>Write to a field in this object's <see cref="ModDataDictionary" />, or remove the field if supplied with a null or empty value.</summary>
    /// <param name="field">The field to write to.</param>
    /// <param name="value">The value to write, or <c>null</c> to remove the field.</param>
    internal static void WriteData(this SObject @object, string field, string value)
    {
        @object.modData.Write($"{ModEntry.Manifest.UniqueID}/{field}", value);
        Log.D($"[ModData]: Wrote {value} to {@object.Name}'s {field}.");
    }

    /// <summary>Write to a field in this object's <see cref="ModDataDictionary" />, only if it doesn't yet have a value.</summary>
    /// <param name="field">The field to write to.</param>
    /// <param name="value">The value to write, or <c>null</c> to remove the field.</param>
    internal static bool WriteDataIfNotExists(this SObject @object, string field, string value)
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
    internal static void IncrementData<T>(this SObject @object, string field, T amount)
    {
        @object.modData.Increment($"{ModEntry.Manifest.UniqueID}/{field}", amount);
        Log.D($"[ModData]: Incremented {@object.Name}'s {field} by {amount}.");
    }

    /// <summary>Increment the value of a numeric field in this object's <see cref="ModDataDictionary" /> by 1.</summary>
    /// <param name="field">The field to update.</param>
    internal static void IncrementData<T>(this SObject @object, string field)
    {
        @object.modData.Increment($"{ModEntry.Manifest.UniqueID}/{field}",
            "1".Parse<T>());
        Log.D($"[ModData]: Incremented {@object.Name}'s {field} by 1.");
    }
}