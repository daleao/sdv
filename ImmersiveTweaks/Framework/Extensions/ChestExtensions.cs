namespace DaLion.Stardew.Tweaks.Framework.Extensions;

#region using directives

using StardewValley.Objects;

#endregion using directives

/// <summary>Extensions for the <see cref="Chest"/> class.</summary>
internal static class ChestExtensions
{
    /// <summary>Remove a specified ring from the chest's inventory.</summary>
    /// <param name="index">The ring index.</param>
    /// <param name="amount">How many rings should be consumed.</param>
    /// <returns>Returns the leftover amount, if not enough were consumed.</returns>
    internal static int ConsumeRing(this Chest chest, int index, int amount)
    {
        var list = chest.items;
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
}