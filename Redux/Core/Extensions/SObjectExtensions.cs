namespace DaLion.Redux.Core.Extensions;

#region using directives

using System.Collections.Generic;
using DaLion.Shared.Extensions;
using StardewModdingAPI.Utilities;
using StardewValley.Objects;

#endregion using directives

/// <summary>Extensions for the <see cref="SObject"/> class.</summary>
internal static class SObjectExtensions
{
    /// <summary>Gets the ids of animal products and derived artisan goods.</summary>
    internal static IReadOnlySet<int> AnimalDerivedProductIds { get; } = new HashSet<int>
    {
        107, // dinosaur egg
        306, // mayonnaise
        307, // duck mayonnaise
        308, // void mayonnaise
        340, // honey
        424, // cheese
        426, // goat cheese
        428, // cloth
        807, // dinosaur mayonnaise
    };

    /// <summary>Determines whether <paramref name="object"/> is an animal produce or derived artisan good.</summary>
    /// <param name="object">The <see cref="SObject"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="object"/> is produced directly or indirectly by an animal, otherwise <see langword="false"/>.</returns>
    internal static bool IsAnimalProduct(this SObject @object)
    {
        return @object.Category.IsIn(
                   SObject.EggCategory,
                   SObject.MilkCategory,
                   SObject.meatCategory,
                   SObject.sellAtPierresAndMarnies) ||
               AnimalDerivedProductIds.Contains(@object.ParentSheetIndex);
    }

    /// <summary>Determines whether the <paramref name="obj"/> is an artisan good.</summary>
    /// <param name="obj">The <see cref="SObject"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="obj"/> is created by an artisan machine, otherwise <see langword="false"/>.</returns>
    internal static bool IsArtisanGood(this SObject obj)
    {
        return obj.Category is SObject.artisanGoodsCategory or SObject.syrupCategory ||
               obj.ParentSheetIndex == 395; // coffee
    }

    /// <summary>Determines whether the <paramref name="object"/> is a bee house.</summary>
    /// <param name="object">The <see cref="SObject"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="object"/> is a Bee House, otherwise <see langword="false"/>.</returns>
    internal static bool IsBeeHouse(this SObject @object)
    {
        return @object.bigCraftable.Value && @object.ParentSheetIndex == 10;
    }

    /// <summary>Determines whether the <paramref name="object"/> is a mushroom box.</summary>
    /// <param name="object">The <see cref="SObject"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="object"/> is a Mushroom Box, otherwise <see langword="false"/>.</returns>
    internal static bool IsMushroomBox(this SObject @object)
    {
        return @object.bigCraftable.Value && @object.ParentSheetIndex == 128;
    }

    /// <summary>Determines whether <paramref name="object"/> is Spring Onion.</summary>
    /// <param name="object">The <see cref="SObject"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="object"/> is Spring Onion, otherwise <see langword="false"/>.</returns>
    internal static bool IsSpringOnion(this SObject @object)
    {
        return @object.ParentSheetIndex == 399;
    }

    /// <summary>Determines whether <paramref name="object"/> is Salmonberry or Blackberry.</summary>
    /// <param name="object">The <see cref="SObject"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="object"/> is Salmonberry or Blackberry, otherwise <see langword="false"/>.</returns>
    internal static bool IsWildBerry(this SObject @object)
    {
        return @object.ParentSheetIndex is 296 or 410;
    }

    /// <summary>Determines whether <paramref name="object"/> is an Artifact Spot.</summary>
    /// <param name="object">The <see cref="SObject"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="object"/> is an Artifact Spot, otherwise <see langword="false"/>.</returns>
    internal static bool IsArtifactSpot(this SObject @object)
    {
        return @object.ParentSheetIndex == 590;
    }

    /// <summary>
    ///     Determines whether <paramref name="object"/> is a fish typically caught with a
    ///     <see cref="StardewValley.Tools.FishingRod"/>.
    /// </summary>
    /// <param name="object">The <see cref="SObject"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="object"/> is any fish, otherwise <see langword="false"/>.</returns>
    internal static bool IsFish(this SObject @object)
    {
        return @object.Category == SObject.FishCategory;
    }

    /// <summary>Determines whether the <paramref name="obj"/> is a legendary fish.</summary>
    /// <param name="obj">The <see cref="SObject"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="obj"/> has the legendary fish context tag, otherwise <see langword="false"/>.</returns>
    internal static bool IsLegendaryFish(this SObject obj)
    {
        return obj.HasContextTag("fish_legendary");
    }

    /// <summary>
    ///     Determines whether <paramref name="object"/> is a fish typically caught with a
    ///     <see cref="StardewValley.Objects.CrabPot"/>.
    /// </summary>
    /// <param name="object">The <see cref="SObject"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="object"/> is a fish ordinarily caught by a <see cref="CrabPot"/>, otherwise <see langword="false"/>.</returns>
    internal static bool IsTrapFish(this SObject @object)
    {
        return Game1.content.Load<Dictionary<int, string>>(PathUtilities.NormalizeAssetName("Data/Fish"))
            .TryGetValue(@object.ParentSheetIndex, out var fishData) && fishData.Contains("trap");
    }

    /// <summary>Determines whether the <paramref name="obj"/> is algae or seaweed.</summary>
    /// <param name="obj">The <see cref="SObject"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="obj"/> is any of the three algae, otherwise <see langword="false"/>.</returns>
    internal static bool IsAlgae(this SObject obj)
    {
        return obj.ParentSheetIndex.IsAlgaeIndex();
    }

    /// <summary>Determines whether <paramref name="object"/> is trash.</summary>
    /// <param name="object">The <see cref="SObject"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="object"/> is any trash item, otherwise <see langword="false"/>.</returns>
    internal static bool IsTrash(this SObject @object)
    {
        return @object.Category == SObject.junkCategory;
    }

    /// <summary>Determines whether the <paramref name="object"/> is a gem or mineral.</summary>
    /// <param name="object">The <see cref="SObject"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="object"/> is a gem or mineral, otherwise <see langword="false"/>.</returns>
    internal static bool IsGemOrMineral(this SObject @object)
    {
        return @object.Category is SObject.GemCategory or SObject.mineralsCategory;
    }

    /// <summary>Determines whether <paramref name="object"/> is a foraged mineral.</summary>
    /// <param name="object">The <see cref="SObject"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="object"/> is a foraged mineral, otherwise <see langword="false"/>.</returns>
    internal static bool IsForagedMineral(this SObject @object)
    {
        return @object.Name.IsIn("Quartz", "Earth Crystal", "Frozen Tear", "Fire Quartz");
    }

    /// <summary>Determines whether <paramref name="object"/> is a resource node.</summary>
    /// <param name="object">The <see cref="SObject"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="object"/> is a mining node containing precious resources, otherwise <see langword="false"/>.</returns>
    internal static bool IsResourceNode(this SObject @object)
    {
        return Collections.ResourceNodeIds.Contains(@object.ParentSheetIndex);
    }

    /// <summary>Determines whether <paramref name="object"/> is a simple Stone.</summary>
    /// <param name="object">The <see cref="SObject"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="object"/> is a mining node not containing only stone, otherwise <see langword="false"/>.</returns>
    internal static bool IsStone(this SObject @object)
    {
        return @object.Name == "Stone";
    }

    /// <summary>Determines whether the <paramref name="object"/> is a twig.</summary>
    /// <param name="object">The <see cref="SObject"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="object"/> is twig, otherwise <see langword="false"/>.</returns>
    internal static bool IsTwig(this SObject @object)
    {
        return @object.ParentSheetIndex is 294 or 295;
    }

    /// <summary>Determines whether the <paramref name="object"/> is a weed.</summary>
    /// <param name="object">The <see cref="SObject"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="object"/> is weed, otherwise <see langword="false"/>.</returns>
    internal static bool IsWeed(this SObject @object)
    {
        return @object is not Chest && @object.Name == "Weeds";
    }
}
