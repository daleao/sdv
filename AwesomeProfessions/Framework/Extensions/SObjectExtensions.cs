using StardewModdingAPI.Utilities;
using StardewValley;
using System.Collections.Generic;
using System.Linq;
using TheLion.Stardew.Common.Extensions;
using TheLion.Stardew.Professions.Framework.Utility;
using SObject = StardewValley.Object;

namespace TheLion.Stardew.Professions.Framework.Extensions;

public static class SObjectExtensions
{
    /// <summary>Whether a given object is an artisan good.</summary>
    public static bool IsArtisanGood(this SObject obj)
    {
        return obj.Category == SObject.artisanGoodsCategory;
    }

    /// <summary>Whether a given object is an artisan good.</summary>
    public static bool IsArtisanMachine(this SObject obj)
    {
        return Objects.ArtisanMachines.Contains(obj?.name);
    }

    /// <summary>Whether a given object is an animal produce or derived artisan good.</summary>
    public static bool IsAnimalProduct(this SObject obj)
    {
        return obj.Category.IsAnyOf(SObject.EggCategory, SObject.MilkCategory, SObject.sellAtPierresAndMarnies)
               || Objects.AnimalDerivedProductIDs.Contains(obj.ParentSheetIndex);
    }

    /// <summary>Whether a given object is salmonberry or blackberry.</summary>
    public static bool IsWildBerry(this SObject obj)
    {
        return obj.ParentSheetIndex is 296 or 410;
    }

    /// <summary>Whether a given object is a spring onion.</summary>
    public static bool IsSpringOnion(this SObject obj)
    {
        return obj.ParentSheetIndex == 399;
    }

    /// <summary>Whether a given object is a gem or mineral.</summary>
    public static bool IsGemOrMineral(this SObject obj)
    {
        return obj.Category.IsAnyOf(SObject.GemCategory, SObject.mineralsCategory);
    }

    /// <summary>Whether a given object is a foraged mineral.</summary>
    public static bool IsForagedMineral(this SObject obj)
    {
        return obj.Name.IsAnyOf("Quartz", "Earth Crystal", "Frozen Tear", "Fire Quartz");
    }

    /// <summary>Whether a given object is a resource node or foraged mineral.</summary>
    public static bool IsResourceNode(this SObject obj)
    {
        return Objects.ResourceNodeIDs.Contains(obj.ParentSheetIndex);
    }

    /// <summary>Whether a given object is a stone.</summary>
    public static bool IsStone(this SObject obj)
    {
        return obj.Name == "Stone";
    }

    /// <summary>Whether a given object is an artifact spot.</summary>
    public static bool IsArtifactSpot(this SObject obj)
    {
        return obj.ParentSheetIndex == 590;
    }

    /// <summary>Whether a given object is a fish caught with a fishing rod.</summary>
    public static bool IsFish(this SObject obj)
    {
        return obj.Category == SObject.FishCategory;
    }

    /// <summary>Whether a given object is a crab pot fish.</summary>
    public static bool IsTrapFish(this SObject obj)
    {
        return Game1.content.Load<Dictionary<int, string>>(PathUtilities.NormalizeAssetName("Data/Fish"))
            .TryGetValue(obj.ParentSheetIndex, out var fishData) && fishData.Contains("trap");
    }

    /// <summary>Whether a given object is algae or seaweed.</summary>
    public static bool IsAlgae(this SObject obj)
    {
        return obj.ParentSheetIndex.IsAnyOf(152, 153, 157);
    }

    /// <summary>Whether a given object is a trash.</summary>
    public static bool IsTrash(this SObject obj)
    {
        return obj.Category == SObject.junkCategory;
    }

    /// <summary>Whether a given object is typically found in pirate treasure.</summary>
    public static bool IsPirateTreasure(this SObject obj)
    {
        return Objects.TrapperPirateTreasureTable.ContainsKey(obj.ParentSheetIndex);
    }

    /// <summary>Whether the player should track a given object.</summary>
    public static bool ShouldBeTracked(this SObject obj)
    {
        return Game1.player.HasProfession("Scavenger") &&
               (obj.IsSpawnedObject && !obj.IsForagedMineral() || obj.IsArtifactSpot())
               || Game1.player.HasProfession("Prospector") &&
               (obj.IsStone() && obj.IsResourceNode() || obj.IsForagedMineral());
    }
}