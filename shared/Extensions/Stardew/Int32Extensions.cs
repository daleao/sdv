namespace DaLion.Shared.Extensions.Stardew;

#region using directives

using System.Collections.Generic;
using System.Linq;
using DaLion.Shared.Constants;
using StardewValley.Objects;

#endregion using directives

/// <summary>Extensions for the <see cref="int"/> primitive type.</summary>
internal static class Int32Extensions
{
    private static HashSet<int>? _trapFishIndices;

    /// <summary>
    ///     Determines whether object <paramref name="index"/> corresponds to a fish item usually caught with a
    ///     <see cref="CrabPot"/>.
    /// </summary>
    /// <param name="index">A <see cref="Item"/> index.</param>
    /// <returns><see langword="true"/> if the <paramref name="index"/> corresponds to a <see cref="CrabPot"/> fish, otherwise <see langword="false"/>.</returns>
    public static bool IsTrapFishIndex(this int index)
    {
        _trapFishIndices ??= Game1.content.Load<Dictionary<int, string>>("Data\\Fish")
            .Where(pair => pair.Value.Contains("trap"))
            .Select(pair => pair.Key)
            .ToHashSet();
        return _trapFishIndices.Contains(index);
    }

    /// <summary>Determines whether the <paramref name="index"/> corresponds to an algae or seaweed.</summary>
    /// <param name="index">A <see cref="Item"/> index.</param>
    /// <returns><see langword="true"/> if the <paramref name="index"/> corresponds to an algae or seaweed, otherwise <see langword="false"/>.</returns>
    internal static bool IsAlgaeIndex(this int index)
    {
        return index is ObjectIds.Seaweed or ObjectIds.GreenAlgae or ObjectIds.WhiteAlgae;
    }

    /// <summary>Determines whether the object <paramref name="index"/> corresponds to a trash item.</summary>
    /// <param name="index">A <see cref="Item"/> index.</param>
    /// <returns><see langword="true"/> if the <paramref name="index"/> corresponds any trash item, otherwise <see langword="false"/>.</returns>
    internal static bool IsTrashIndex(this int index)
    {
        return index.IsIn(167..172);
    }

    /// <summary>Determines whether the object <paramref name="index"/> corresponds to any metallic ore.</summary>
    /// <param name="index">A <see cref="Item"/> index.</param>
    /// <returns><see langword="true"/> if the <paramref name="index"/> corresponds to either copper, iron, gold, iridium or radioactive ore, otherwise <see langword="false"/>.</returns>
    internal static bool IsOre(this int index)
    {
        return index is ObjectIds.CopperOre or ObjectIds.IronOre or ObjectIds.GoldOre or ObjectIds.IridiumOre
            or ObjectIds.RadioactiveOre;
    }
}
