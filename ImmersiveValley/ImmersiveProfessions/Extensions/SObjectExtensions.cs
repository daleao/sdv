namespace DaLion.Stardew.Professions.Extensions;

#region using directives

using System.Collections.Generic;
using System.Linq;
using DaLion.Common.Extensions;
using DaLion.Common.Extensions.Stardew;
using DaLion.Stardew.Professions.Framework;
using DaLion.Stardew.Professions.Framework.Utility;
using StardewModdingAPI.Utilities;
using StardewValley.Objects;

#endregion using directives

/// <summary>Extensions for the <see cref="SObject"/> class.</summary>
public static class SObjectExtensions
{
    /// <summary>Determines whether <paramref name="obj"/> is an artisan good.</summary>
    /// <param name="obj">The <see cref="SObject"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="obj"/> is created by an artisan machine, otherwise <see langword="false"/>.</returns>
    public static bool IsArtisanGood(this SObject obj)
    {
        return obj.Category is SObject.artisanGoodsCategory or SObject.syrupCategory ||
               obj.ParentSheetIndex == 395; // exception for coffee
    }

    /// <summary>Determines whether <paramref name="obj"/> is an artisan machine.</summary>
    /// <param name="obj">The <see cref="SObject"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="obj"/> is a machine that creates artisan goods, otherwise <see langword="false"/>.</returns>
    public static bool IsArtisanMachine(this SObject obj)
    {
        return Lookups.ArtisanMachines.Contains(obj.name);
    }

    /// <summary>Determines whether <paramref name="obj"/> is an animal produce or derived artisan good.</summary>
    /// <param name="obj">The <see cref="SObject"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="obj"/> is produced directly or indirectly by an animal, otherwise <see langword="false"/>.</returns>
    public static bool IsAnimalProduct(this SObject obj)
    {
        return obj.Category.IsAnyOf(
                   SObject.EggCategory,
                   SObject.MilkCategory,
                   SObject.meatCategory,
                   SObject.sellAtPierresAndMarnies) ||
               Lookups.AnimalDerivedProductIds.Contains(obj.ParentSheetIndex);
    }

    /// <summary>Determines whether <paramref name="obj"/> is a Mushroom Box.</summary>
    /// <param name="obj">The <see cref="SObject"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="obj"/> is a Mushroom Box, otherwise <see langword="false"/>.</returns>
    public static bool IsMushroomBox(this SObject obj)
    {
        return obj.bigCraftable.Value && obj.ParentSheetIndex == 128;
    }

    /// <summary>Determines whether <paramref name="obj"/> is Salmonberry or Blackberry.</summary>
    /// <param name="obj">The <see cref="SObject"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="obj"/> is Salmonberry or Blackberry, otherwise <see langword="false"/>.</returns>
    public static bool IsWildBerry(this SObject obj)
    {
        return obj.ParentSheetIndex is 296 or 410;
    }

    /// <summary>Determines whether <paramref name="obj"/> is Spring Onion.</summary>
    /// <param name="obj">The <see cref="SObject"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="obj"/> is Spring Onion, otherwise <see langword="false"/>.</returns>
    public static bool IsSpringOnion(this SObject obj)
    {
        return obj.ParentSheetIndex == 399;
    }

    /// <summary>Determines whether <paramref name="obj"/> is a geode, gem or mineral.</summary>
    /// <param name="obj">The <see cref="SObject"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="obj"/> is a geode, gem or mineral, otherwise <see langword="false"/>.</returns>
    public static bool IsPreciousRock(this SObject obj)
    {
        return obj.Category is SObject.GemCategory or SObject.mineralsCategory || obj.Name.Contains("Geode");
    }

    /// <summary>Determines whether <paramref name="obj"/> is a foraged mineral.</summary>
    /// <param name="obj">The <see cref="SObject"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="obj"/> is a foraged mineral, otherwise <see langword="false"/>.</returns>
    public static bool IsForagedMineral(this SObject obj)
    {
        return obj.Name.IsAnyOf("Quartz", "Earth Crystal", "Frozen Tear", "Fire Quartz");
    }

    /// <summary>Determines whether <paramref name="obj"/> is a resource node.</summary>
    /// <param name="obj">The <see cref="SObject"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="obj"/> is a mining node containing precious resources, otherwise <see langword="false"/>.</returns>
    public static bool IsResourceNode(this SObject obj)
    {
        return Lookups.ResourceNodeIds.Contains(obj.ParentSheetIndex);
    }

    /// <summary>Determines whether <paramref name="obj"/> is a simple Stone.</summary>
    /// <param name="obj">The <see cref="SObject"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="obj"/> is a mining node not containing only stone, otherwise <see langword="false"/>.</returns>
    public static bool IsStone(this SObject obj)
    {
        return obj.Name == "Stone";
    }

    /// <summary>Determines whether <paramref name="obj"/> is an Artifact Spot.</summary>
    /// <param name="obj">The <see cref="SObject"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="obj"/> is an Artifact Spot, otherwise <see langword="false"/>.</returns>
    public static bool IsArtifactSpot(this SObject obj)
    {
        return obj.ParentSheetIndex == 590;
    }

    /// <summary>
    ///     Determines whether <paramref name="obj"/> is a fish typically caught with a
    ///     <see cref="StardewValley.Tools.FishingRod"/>.
    /// </summary>
    /// <param name="obj">The <see cref="SObject"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="obj"/> is any fish, otherwise <see langword="false"/>.</returns>
    public static bool IsFish(this SObject obj)
    {
        return obj.Category == SObject.FishCategory;
    }

    /// <summary>
    ///     Determines whether <paramref name="obj"/> is a fish typically caught with a
    ///     <see cref="StardewValley.Objects.CrabPot"/>.
    /// </summary>
    /// <param name="obj">The <see cref="SObject"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="obj"/> is a fish ordinarily caught by a <see cref="CrabPot"/>, otherwise <see langword="false"/>.</returns>
    public static bool IsTrapFish(this SObject obj)
    {
        return Game1.content.Load<Dictionary<int, string>>(PathUtilities.NormalizeAssetName("Data/Fish"))
            .TryGetValue(obj.ParentSheetIndex, out var fishData) && fishData.Contains("trap");
    }

    /// <summary>Determines whether <paramref name="obj"/> is algae or seaweed.</summary>
    /// <param name="obj">The <see cref="SObject"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="obj"/> is any of the three algae, otherwise <see langword="false"/>.</returns>
    public static bool IsAlgae(this SObject obj)
    {
        return obj.ParentSheetIndex is 152 or 153 or 157;
    }

    /// <summary>Determines whether <paramref name="obj"/> is trash.</summary>
    /// <param name="obj">The <see cref="SObject"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="obj"/> is any trash item, otherwise <see langword="false"/>.</returns>
    public static bool IsTrash(this SObject obj)
    {
        return obj.Category == SObject.junkCategory;
    }

    /// <summary>Determines whether <paramref name="obj"/> is typically found in pirate treasure.</summary>
    /// <param name="obj">The <see cref="SObject"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="obj"/> is any non-fish item caught from the ocean, otherwise <see langword="false"/>.</returns>
    public static bool IsPirateTreasure(this SObject obj)
    {
        return Lookups.TrapperPirateTreasureTable.ContainsKey(obj.ParentSheetIndex);
    }

    /// <summary>Determines whether the <paramref name="profession"/> should track <paramref name="obj"/>.</summary>
    /// <param name="obj">The <see cref="SObject"/>.</param>
    /// <param name="profession">Either <see cref="Profession.Scavenger"/> or <see cref="Profession.Prospector"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="obj"/> should be tracked by the <paramref name="profession"/>, otherwise <see langword="false"/>.</returns>
    public static bool ShouldBeTrackedBy(this SObject obj, Profession profession)
    {
        return (profession == Profession.Scavenger && ((obj.IsSpawnedObject && !obj.IsForagedMineral()) ||
                                                       obj.IsSpringOnion() || obj.IsArtifactSpot())) ||
               (profession == Profession.Prospector && ((obj.IsStone() && obj.IsResourceNode()) ||
                                                        obj.IsForagedMineral() || obj.IsArtifactSpot()));
    }

    /// <summary>Determines whether the owner of this <paramref name="obj"/> has the specified <paramref name="profession"/>.</summary>
    /// <param name="obj">The <see cref="SObject"/>.</param>
    /// <param name="profession">A <see cref="IProfession"/>.</param>
    /// <param name="prestiged">Whether to check for the prestiged variant.</param>
    /// <returns><see langword="true"/> if the <see cref="Farmer"/> who owns the <paramref name="obj"/> has the <paramref name="profession"/>, otherwise <see langword="false"/>.</returns>
    public static bool DoesOwnerHaveProfession(this SObject obj, IProfession profession, bool prestiged = false)
    {
        return obj.GetOwner().HasProfession(profession, prestiged);
    }

    /// <summary>Determines whether the owner of the <paramref name="obj"/> has the <see cref="Profession"/> corresponding to <paramref name="index"/>.</summary>
    /// <param name="obj">The <see cref="SObject"/>.</param>
    /// <param name="index">A valid profession index.</param>
    /// <param name="prestiged">Whether to check for the prestiged variant.</param>
    /// <returns><see langword="true"/> if the owner of <paramref name="obj"/> the <see cref="Profession"/> with the specified <paramref name="index"/>, otherwise <see langword="false"/>.</returns>
    /// <remarks>This overload exists only to be called by emitted ILCode. Expects a vanilla <see cref="Profession"/>.</remarks>
    public static bool DoesOwnerHaveProfession(this SObject obj, int index, bool prestiged = false)
    {
        return Profession.TryFromValue(index, out var profession) &&
               obj.GetOwner().HasProfession(profession, prestiged);
    }
}
