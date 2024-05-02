namespace DaLion.Core.Framework.Extensions;

#region using directives

using StardewValley.Inventories;
using StardewValley.Objects;

#endregion using directives

/// <summary>Extensions for the <see cref="IInventory"/> interface.</summary>
internal static class InventoryExtensions
{
    /// <summary>Removes the specified <see cref="Ring"/> from the <paramref name="items"/>'s inventory.</summary>
    /// <param name="items">The <see cref="IInventory"/>.</param>
    /// <param name="id">The <see cref="Ring"/> id.</param>
    /// <param name="amount">How many should be consumed.</param>
    /// <returns>The leftover amount, if not enough were consumed.</returns>
    internal static int ConsumeRing(this IInventory items, string id, int amount)
    {
        for (var i = 0; i < items.Count; i++)
        {
            var item = items[i];
            if (item is not Ring || item.QualifiedItemId != id)
            {
                continue;
            }

            items[i] = null;
            if (--amount > 0)
            {
                continue;
            }

            return 0;
        }

        return amount;
    }

    /// <summary>Removes the specified <see cref="SObject"/> from the <paramref name="items"/>'s inventory.</summary>
    /// <param name="items">The <see cref="IInventory"/>.</param>
    /// <param name="id">The <see cref="SObject"/> id.</param>
    /// <param name="amount">How many should be consumed.</param>
    /// <returns>The leftover amount, if not enough were consumed.</returns>
    internal static int ConsumeObject(this IInventory items, string id, int amount)
    {
        for (var i = 0; i < items.Count; i++)
        {
            var item = items[i];
            if (item is not SObject || item.QualifiedItemId != id)
            {
                continue;
            }

            var toRemove = amount;
            amount -= item.Stack;
            items[i].Stack -= toRemove;
            if (items[i].Stack <= 0)
            {
                items[i] = null;
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
