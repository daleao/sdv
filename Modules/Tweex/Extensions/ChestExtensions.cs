namespace DaLion.Overhaul.Modules.Tweex.Extensions;

#region using directives

using StardewValley.Objects;

#endregion using directives

/// <summary>Extensions for the <see cref="Chest"/> class.</summary>
internal static class ChestExtensions
{
    /// <summary>Removes the specified <see cref="Ring"/> from the <paramref name="chest"/>'s inventory.</summary>
    /// <param name="chest">The <see cref="Chest"/>.</param>
    /// <param name="index">The <see cref="Ring"/> index.</param>
    /// <param name="amount">How many should be consumed.</param>
    /// <returns>The leftover amount, if not enough were consumed.</returns>
    internal static int ConsumeRing(this Chest chest, int index, int amount)
    {
        var list = chest.items;
        for (var i = 0; i < list.Count; i++)
        {
            var item = list[i];
            if (item is not Ring || item.ParentSheetIndex != index)
            {
                continue;
            }

            list[i] = null;
            if (--amount > 0)
            {
                continue;
            }

            return 0;
        }

        return amount;
    }

    /// <summary>Removes the specified <see cref="SObject"/> from the <paramref name="chest"/>'s inventory.</summary>
    /// <param name="chest">The <see cref="Chest"/>.</param>
    /// <param name="index">The <see cref="SObject"/> index.</param>
    /// <param name="amount">How many should be consumed.</param>
    /// <returns>The leftover amount, if not enough were consumed.</returns>
    internal static int ConsumeObject(this Chest chest, int index, int amount)
    {
        var list = chest.items;
        for (var i = 0; i < list.Count; i++)
        {
            var item = list[i];
            if (item is not SObject || item.ParentSheetIndex != index)
            {
                continue;
            }

            var toRemove = amount;
            amount -= item.Stack;
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
