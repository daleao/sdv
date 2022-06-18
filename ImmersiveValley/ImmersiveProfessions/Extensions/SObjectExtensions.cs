namespace DaLion.Stardew.Professions.Extensions;

#region using directives

using System.Collections.Generic;
using System.Linq;
using StardewModdingAPI.Utilities;
using StardewValley;

using Common.Extensions;
using Framework;

using ObjectLookups = Framework.Utility.ObjectLookups;
using SObject = StardewValley.Object;

#endregion using directives

/// <summary>Extensions for the <see cref="SObject"/> class.</summary>
public static class SObjectExtensions
{
    /// <summary>Whether a given object is an artisan good.</summary>
    public static bool IsArtisanGood(this SObject @object)
    {
        return @object.Category == SObject.artisanGoodsCategory || @object.ParentSheetIndex == 395; // exception for coffee
    }

    /// <summary>Whether a given object is an artisan good.</summary>
    public static bool IsArtisanMachine(this SObject @object)
    {
        return ObjectLookups.ArtisanMachines.Contains(@object?.name);
    }

    /// <summary>Whether a given object is an animal produce or derived artisan good.</summary>
    public static bool IsAnimalProduct(this SObject @object)
    {
        return @object.Category.IsIn(SObject.EggCategory, SObject.MilkCategory, SObject.meatCategory, SObject.sellAtPierresAndMarnies)
               || ObjectLookups.AnimalDerivedProductIds.Contains(@object.ParentSheetIndex);
    }

    /// <summary>Whether a given object is a mushroom box.</summary>
    public static bool IsMushroomBox(this SObject @object)
    {
        return @object.bigCraftable.Value && @object.ParentSheetIndex == 128;
    }

    /// <summary>Whether a given object is salmonberry or blackberry.</summary>
    public static bool IsWildBerry(this SObject @object)
    {
        return @object.ParentSheetIndex is 296 or 410;
    }

    /// <summary>Whether a given object is a spring onion.</summary>
    public static bool IsSpringOnion(this SObject @object)
    {
        return @object.ParentSheetIndex == 399;
    }

    /// <summary>Whether a given object is a gem or mineral.</summary>
    public static bool IsGemOrMineral(this SObject @object)
    {
        return @object.Category.IsIn(SObject.GemCategory, SObject.mineralsCategory);
    }

    /// <summary>Whether a given object is a foraged mineral.</summary>
    public static bool IsForagedMineral(this SObject @object)
    {
        return @object.Name.IsIn("Quartz", "Earth Crystal", "Frozen Tear", "Fire Quartz");
    }

    /// <summary>Whether a given object is a resource node or foraged mineral.</summary>
    public static bool IsResourceNode(this SObject @object)
    {
        return ObjectLookups.ResourceNodeIds.Contains(@object.ParentSheetIndex);
    }

    /// <summary>Whether a given object is a stone.</summary>
    public static bool IsStone(this SObject @object)
    {
        return @object.Name == "Stone";
    }

    /// <summary>Whether a given object is an artifact spot.</summary>
    public static bool IsArtifactSpot(this SObject @object)
    {
        return @object.ParentSheetIndex == 590;
    }

    /// <summary>Whether a given object is a fish caught with a fishing rod.</summary>
    public static bool IsFish(this SObject @object)
    {
        return @object.Category == SObject.FishCategory;
    }

    /// <summary>Whether a given object is a crab pot fish.</summary>
    public static bool IsTrapFish(this SObject @object)
    {
        return Game1.content.Load<Dictionary<int, string>>(PathUtilities.NormalizeAssetName("Data/Fish"))
            .TryGetValue(@object.ParentSheetIndex, out var fishData) && fishData.Contains("trap");
    }

    /// <summary>Whether a given object is algae or seaweed.</summary>
    public static bool IsAlgae(this SObject @object)
    {
        return @object.ParentSheetIndex is 152 or 153 or 157;
    }

    /// <summary>Whether a given object is trash.</summary>
    public static bool IsTrash(this SObject @object)
    {
        return @object.Category == SObject.junkCategory;
    }

    /// <summary>Whether a given object is typically found in pirate treasure.</summary>
    public static bool IsPirateTreasure(this SObject @object)
    {
        return ObjectLookups.TrapperPirateTreasureTable.ContainsKey(@object.ParentSheetIndex);
    }

    /// <summary>Whether the player should track a given object.</summary>
    public static bool ShouldBeTracked(this SObject @object)
    {
        return Game1.player.HasProfession(Profession.Scavenger) &&
               (@object.IsSpawnedObject && !@object.IsForagedMineral() || @object.IsSpringOnion() || @object.IsArtifactSpot())
               || Game1.player.HasProfession(Profession.Prospector) &&
               (@object.IsStone() && @object.IsResourceNode() || @object.IsForagedMineral());
    }

    /// <summary>Whether the owner of this instance has the specified profession.</summary>
    /// <param name="index">A valid profession index.</param>
    /// <remarks>This extension is only called by emitted ILCode, so we use a simpler <see cref="int"/> interface instead of the standard <see cref="Profession"/>.</remarks>>
    public static bool DoesOwnerHaveProfession(this SObject @object, int index, bool prestiged = false)
    {
        if (!Profession.TryFromValue(index, out var profession)) return false;

        var owner = Game1.getFarmerMaybeOffline(@object.owner.Value) ?? Game1.MasterPlayer;
        return owner.HasProfession(profession, prestiged);
    }
}