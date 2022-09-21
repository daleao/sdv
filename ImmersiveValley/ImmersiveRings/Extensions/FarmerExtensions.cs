namespace DaLion.Stardew.Rings.Extensions;

#region using directives

using System.Collections.Generic;
using System.Linq;
using StardewValley.Objects;

#endregion using directives

/// <summary>Extensions for the <see cref="Farmer"/> class.</summary>
public static class FarmerExtensions
{
    /// <summary>
    ///     Counts the units of a specific <see cref="Ring"/> in the <paramref name="farmer"/>'s inventory, or the
    ///     specified <paramref name="list"/> of items.
    /// </summary>
    /// <param name="farmer">The <see cref="Farmer"/>.</param>
    /// <param name="index">The <see cref="Ring"/> index.</param>
    /// <param name="list">An optional list of items to override the <paramref name="farmer"/>'s inventory.</param>
    /// <returns>The number of <see cref="Ring"/>s with the specified <paramref name="index"/>.</returns>
    public static int GetRingItemCount(this Farmer farmer, int index, IList<Item>? list = null)
    {
        list ??= farmer.Items;
        return list.Count(item => item is Ring && item.ParentSheetIndex == index);
    }

    /// <summary>Removes the specified <see cref="Ring"/> from the <paramref name="farmer"/>'s inventory.</summary>
    /// <param name="farmer">The <see cref="Farmer"/>.</param>
    /// <param name="index">The <see cref="Ring"/> index.</param>
    /// <param name="amount">How many should be consumed.</param>
    /// <returns>The leftover amount, if not enough were consumed.</returns>
    public static int ConsumeRing(this Farmer farmer, int index, int amount)
    {
        var list = farmer.Items;
        for (var i = 0; i < list.Count; ++i)
        {
            if (list[i] is not Ring || list[i].ParentSheetIndex != index)
            {
                continue;
            }

            --amount;
            list[i] = null;
            if (amount > 0)
            {
                continue;
            }

            return 0;
        }

        return amount;
    }

    /// <summary>Removes the specified <see cref="SObject"/> from the <paramref name="farmer"/>'s inventory.</summary>
    /// <param name="farmer">The <see cref="Farmer"/>.</param>
    /// <param name="index">The <see cref="SObject"/> index.</param>
    /// <param name="amount">How many should be consumed.</param>
    /// <returns>The leftover amount, if not enough were consumed.</returns>
    public static int ConsumeObject(this Farmer farmer, int index, int amount)
    {
        var list = farmer.Items;
        for (var i = 0; i < list.Count; ++i)
        {
            if (list[i] is not SObject || list[i].ParentSheetIndex != index)
            {
                continue;
            }

            var toRemove = amount;
            amount -= list[i].Stack;
            list[i].Stack -= toRemove;
            if (list[i].Stack <= 0)
            {
                list[i] = null;
            }

            if (amount > 0)
            {
                continue;
            }

            return 0;
        }

        return amount;
    }
}
