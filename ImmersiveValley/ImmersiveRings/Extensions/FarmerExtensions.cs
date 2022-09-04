namespace DaLion.Stardew.Rings.Extensions;

#region using directives

using LinqFasterer;
using StardewValley.Objects;
using System.Collections.Generic;

#endregion using directives

/// <summary>Extensions for the <see cref="Farmer"/> class.</summary>
public static class FarmerExtensions
{
    /// <summary>Count the units of a specific ring in the farmer's inventory, or the specified list of items.</summary>
    /// <param name="index">The ring index.</param>
    /// <param name="list">Optional list of items to override the farmer's inventory.</param>
    public static int GetRingItemCount(this Farmer farmer, int index, IList<Item>? list = null)
    {
        list ??= farmer.Items;
        return list.CountF(item => item is Ring && item.ParentSheetIndex == index);
    }

    /// <summary>Remove a specified ring from the farmer's inventory.</summary>
    /// <param name="index">The ring index.</param>
    /// <param name="amount">How many rings should be consumed.</param>
    /// <returns>The leftover amount, if not enough were consumed.</returns>
    public static int ConsumeRing(this Farmer farmer, int index, int amount)
    {
        var list = farmer.Items;
        for (var i = 0; i < list.Count; ++i)
        {
            if (list[i] is not Ring || list[i].ParentSheetIndex != index) continue;

            --amount;
            list[i] = null;
            if (amount > 0) continue;

            return 0;
        }

        return amount;
    }

    /// <summary>Remove a specified object from the farmer's inventory.</summary>
    /// <param name="index">The object index.</param>
    /// <param name="amount">How many units should be consumed.</param>
    /// <returns>The leftover amount, if not enough were consumed.</returns>
    public static int ConsumeObject(this Farmer farmer, int index, int amount)
    {
        var list = farmer.Items;
        for (var i = 0; i < list.Count; ++i)
        {
            if (list[i] is not SObject || list[i].ParentSheetIndex != index) continue;

            var toRemove = amount;
            amount -= list[i].Stack;
            list[i].Stack -= toRemove;
            if (list[i].Stack <= 0)
                list[i] = null;

            if (amount > 0) continue;

            return 0;
        }

        return amount;
    }
}