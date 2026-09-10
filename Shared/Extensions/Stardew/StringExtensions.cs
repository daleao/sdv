namespace DaLion.Shared.Extensions.Stardew;

#region using directives

using DaLion.Shared.Constants;
using StardewValley;
using StardewValley.Objects;

#endregion using directives

/// <summary>Extensions for the <see cref="int"/> primitive type.</summary>
public static class StringExtensions
{
    /// <summary>Determines whether <paramref name="id"/> corresponds to Salmonberry or Blackberry.</summary>
    /// <param name="id">A <see cref="Item"/> ID.</param>
    /// <returns><see langword="true"/> if the <paramref name="id"/> corresponds to Salmonberry or Blackberry, otherwise <see langword="false"/>.</returns>
    public static bool IsWildBerryId(this string id)
    {
        id = id.QualifyId();
        return id is QIDs.Salmonberry or QIDs.Blackberry;
    }

    /// <summary>Determines whether <paramref name="id"/> corresponds to a mushroom item.</summary>
    /// <param name="id">A <see cref="Item"/> ID.</param>
    /// <returns><see langword="true"/> if the <paramref name="id"/> corresponds to a mushroom item, otherwise <see langword="false"/>.</returns>
    public static bool IsMushroomId(this string id)
    {
        id = id.QualifyId();
        return ItemContextTagManager.HasBaseTag(id, "mushroom_item") ||
            ItemContextTagManager.HasBaseTag(id, "edible_mushroom") ||
               id is QIDs.RedMushroom or QIDs.Truffle;
    }

    /// <summary>Determines whether <paramref name="id"/> corresponds to a syrup item.</summary>
    /// <param name="id">A <see cref="Item"/> ID.</param>
    /// <returns><see langword="true"/> if the <paramref name="id"/> corresponds to a syrup item, otherwise <see langword="false"/>.</returns>
    public static bool IsSyrupId(this string id)
    {
        id = id.QualifyId();
        return ItemContextTagManager.HasBaseTag(id, "syrup_item") ||
               id is QIDs.MysticSyrup;
    }

    /// <summary>Determines whether <paramref name="id"/> corresponds to a syrup item.</summary>
    /// <param name="id">A <see cref="Item"/> ID.</param>
    /// <returns><see langword="true"/> if the <paramref name="id"/> corresponds to a syrup item, otherwise <see langword="false"/>.</returns>
    public static bool IsBaitId(this string id)
    {
        id = id.QualifyId();
        return ItemContextTagManager.HasBaseTag(id, "bait_item") ||
               id is QIDs.Bait or QIDs.WildBait or QIDs.MagicBait or
               QIDs.DeluxeBait or QIDs.ChallengeBait or QIDs.SpecificBait or
               QIDs.MagnetBait;
    }

    /// <summary>Determines whether the <paramref name="id"/> corresponds to an algae or seaweed.</summary>
    /// <param name="id">A <see cref="Item"/> ID.</param>
    /// <returns><see langword="true"/> if the <paramref name="id"/> corresponds to an algae or seaweed, otherwise <see langword="false"/>.</returns>
    public static bool IsAlgaeId(this string id)
    {
        return ItemContextTagManager.HasBaseTag(id, "algae_item");
    }

    /// <summary>Determines whether the object <paramref name="id"/> corresponds to a trash id.</summary>
    /// <param name="id">A <see cref="Item"/> ID.</param>s
    /// <returns><see langword="true"/> if the <paramref name="id"/> corresponds any trash id, otherwise <see langword="false"/>.</returns>
    public static bool IsTrashId(this string id)
    {
        return ItemContextTagManager.HasBaseTag(id, "trash_item");
    }

    /// <summary>
    ///     Determines whether object <paramref name="id"/> corresponds to a fish id usually caught with a
    ///     <see cref="CrabPot"/>.
    /// </summary>
    /// <param name="id">A <see cref="Item"/> ID.</param>
    /// <returns><see langword="true"/> if the <paramref name="id"/> corresponds to a <see cref="CrabPot"/> fish, otherwise <see langword="false"/>.</returns>
    public static bool IsTrapFishId(this string id)
    {
        return ItemContextTagManager.HasBaseTag(id, "fish_crab_pot");
    }

    /// <summary>
    ///     Determines whether object <paramref name="id"/> corresponds to a legendary fish id.
    /// </summary>
    /// <param name="id">A <see cref="Item"/> ID.</param>
    /// <returns><see langword="true"/> if the <paramref name="id"/> corresponds to a legendary fish, otherwise <see langword="false"/>.</returns>
    public static bool IsBossFishId(this string id)
    {
        return ItemContextTagManager.HasBaseTag(id, "fish_legendary");
    }

    /// <summary>Determines whether the object <paramref name="id"/> corresponds to any metallic ore.</summary>
    /// <param name="id">A <see cref="Item"/> ID.</param>
    /// <returns><see langword="true"/> if the <paramref name="id"/> corresponds to either copper, iron, gold, iridium or radioactive ore, otherwise <see langword="false"/>.</returns>
    public static bool IsOreId(this string id)
    {
        id = id.QualifyId();
        return id is QIDs.CopperOre or QIDs.IronOre or QIDs.GoldOre or QIDs.IridiumOre
            or QIDs.RadioactiveOre;
    }

    private static string QualifyId(this string id)
    {
        return id.StartsWith("(O)", StringComparison.Ordinal)
            ? id
            : "(O)" + id;
    }
}
