namespace DaLion.Shared.Extensions.Stardew;

#region using directives

using DaLion.Shared.Constants;
using DaLion.Shared.Enums;
using StardewValley.Objects;

#endregion using directives

/// <summary>Extensions for the <see cref="Item"/> class.</summary>
public static class ItemExtensions
{
    /// <summary>Determines whether the <paramref name="item"/> is an artisan good.</summary>
    /// <param name="item">The <see cref="Item"/>.</param>
    /// <param name="excludeFlowers">Whether to exclude flower crops.</param>
    /// <param name="includeForage">Whether to include wild forage items.</param>
    /// <returns><see langword="true"/> if the <paramref name="item"/> is created by an artisan machine, otherwise <see langword="false"/>.</returns>
    public static bool IsCrop(this Item item, bool excludeFlowers = false, bool includeForage = false)
    {
        return item.Category is (int)ObjectCategory.Vegetables or (int)ObjectCategory.Fruits ||
            (!excludeFlowers && item.Category == (int)ObjectCategory.Flowers) ||
            (includeForage && item.Category == (int)ObjectCategory.Greens);
    }

    /// <summary>Determines whether the <paramref name="item"/> is an artisan good.</summary>
    /// <param name="item">The <see cref="Item"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="item"/> is created by an artisan machine, otherwise <see langword="false"/>.</returns>
    public static bool IsArtisanGood(this Item item)
    {
        return item.Category is (int)ObjectCategory.ArtisanGoods ||
               item.QualifiedItemId == QIDs.Coffee;
    }

    /// <summary>Determines whether the <paramref name="item"/> is Salmonberry or Blackberry.</summary>
    /// <param name="item">The <see cref="Item"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="item"/> is Salmonberry or Blackberry, otherwise <see langword="false"/>.</returns>
    public static bool IsWildBerry(this Item item)
    {
        return item.QualifiedItemId.IsWildBerryId();
    }

    /// <summary>Determines whether the <paramref name="item"/> is a mushroom.</summary>
    /// <param name="item">The <see cref="Item"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="item"/> is a mushroom, otherwise <see langword="false"/>.</returns>
    public static bool IsMushroom(this Item item)
    {
        return item.HasContextTag("mushroom_item") || item.HasContextTag("edible_mushroom") ||
            item.QualifiedItemId is QIDs.RedMushroom or QIDs.Truffle;
    }

    /// <summary>Determines whether the <paramref name="item"/> is a syrup.</summary>
    /// <param name="item">The <see cref="Item"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="item"/> is a syrup, otherwise <see langword="false"/>.</returns>
    public static bool IsSyrup(this Item item)
    {
        return item.Category == SObject.syrupCategory || item.HasContextTag("syrup_item") ||
            item.QualifiedItemId == QIDs.MysticSyrup;
    }

    /// <summary>Determines whether the <paramref name="item"/> is a bait.</summary>
    /// <param name="item">The <see cref="Item"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="item"/> is a bait, otherwise <see langword="false"/>.</returns>
    public static bool IsBait(this Item item)
    {
        return item.HasContextTag("bait_item") || item.QualifiedItemId is
            QIDs.Bait or QIDs.WildBait or QIDs.MagicBait or QIDs.DeluxeBait or
            QIDs.ChallengeBait or QIDs.SpecificBait or QIDs.MagnetBait;
    }

    /// <summary>
    ///     Determines whether the <paramref name="item"/> is a fish typically caught with a
    ///     <see cref="StardewValley.Tools.FishingRod"/>.
    /// </summary>
    /// <param name="item">The <see cref="Item"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="item"/> is any fish, otherwise <see langword="false"/>.</returns>
    public static bool IsFish(this Item item)
    {
        return item.Category == SObject.FishCategory || item.HasContextTag("fish_item");
    }

    /// <summary>
    ///     Determines whether the <paramref name="item"/> is a fish typically caught with a
    ///     <see cref="CrabPot"/>.
    /// </summary>
    /// <param name="item">The <see cref="Item"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="item"/> is a <see cref="CrabPot"/> fish, otherwise <see langword="false"/>.</returns>
    public static bool IsTrapFish(this Item item)
    {
        return item.HasContextTag("fish_crab_pot");
    }

    /// <summary>
    ///     Determines whether the <paramref name="item"/> is a legendary fish.
    /// </summary>
    /// <param name="item">The <see cref="Item"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="item"/> is a legendary fish, otherwise <see langword="false"/>.</returns>
    public static bool IsBossFish(this Item item)
    {
        return item.HasContextTag("fish_legendary");
    }

    /// <summary>Determines whether the <paramref name="item"/> is trash.</summary>
    /// <param name="item">The <see cref="Item"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="item"/> is any trash item, otherwise <see langword="false"/>.</returns>
    public static bool IsTrash(this Item item)
    {
        return item.Category == SObject.junkCategory;
    }

    /// <summary>Determines whether the <paramref name="item"/> is algae or seaweed.</summary>
    /// <param name="item">The <see cref="Item"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="item"/> is any of the three algae, otherwise <see langword="false"/>.</returns>
    public static bool IsAlgae(this Item item)
    {
        return item.QualifiedItemId.IsAlgaeId();
    }

    /// <summary>Determines whether the <paramref name="item"/> is a gem or mineral.</summary>
    /// <param name="item">The <see cref="Item"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="item"/> is a gem or mineral, otherwise <see langword="false"/>.</returns>
    public static bool IsGemOrMineral(this Item item)
    {
        return item.Category is SObject.GemCategory or SObject.mineralsCategory;
    }

    /// <summary>Determines whether the <paramref name="item"/> is a foraged mineral.</summary>
    /// <param name="item">The <see cref="Item"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="item"/> is a Quartz, Fire Quartz, Frozen Tear or Earth Crystal, otherwise <see langword="false"/>.</returns>
    public static bool IsForagedMineral(this Item item)
    {
        return item.QualifiedItemId is QIDs.Quartz or QIDs.FireQuartz or QIDs.FrozenTear
            or QIDs.EarthCrystal;
    }

    /// <summary>Determines whether the <paramref name="item"/> is a forage item typically found at the beach.</summary>
    /// <param name="item">The <see cref="Item"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="item"/> is a forage item typically found at the beach, otherwise <see langword="false"/>.</returns>
    public static bool IsBeachForage(this Item item)
    {
        return item.HasContextTag("forage_item_beach");
    }

    /// <summary>Determines whether the <paramref name="item"/> is a twig.</summary>
    /// <param name="item">The <see cref="Item"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="item"/> is twig, otherwise <see langword="false"/>.</returns>
    public static bool IsTwig(this Item item)
    {
        return item.QualifiedItemId is QIDs.Twig0 or QIDs.Twig1;
    }

    /// <summary>Determines whether the <paramref name="item"/> is a weed.</summary>
    /// <param name="item">The <see cref="Item"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="item"/> is weed, otherwise <see langword="false"/>.</returns>
    public static bool IsWeed(this Item item)
    {
        return item is not Chest && item.Name == "Weeds";
    }

    /// <summary>Determines whether the <paramref name="item"/> has any of the specified context <paramref name="tags"/>.</summary>
    /// <param name="item">The <see cref="Item"/>.</param>
    /// <param name="tags">A collection of <see cref="string"/> context tags.</param>
    /// <returns><see langword="true"/> if the <paramref name="item"/> has any of the specified context <paramref name="tags"/>, otherwise <see langword="false"/>.</returns>
    public static bool HasAnyContextTag(this Item item, IEnumerable<string> tags)
    {
        return tags.Any(item.HasContextTag);
    }
}
