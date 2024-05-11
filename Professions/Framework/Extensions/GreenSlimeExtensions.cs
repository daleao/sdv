namespace DaLion.Professions.Framework.Extensions;

#region using directives

using DaLion.Professions.Framework.VirtualProperties;
using StardewValley;
using StardewValley.Monsters;

#endregion

/// <summary>Extensions for the <see cref="GreenSlime"/> class.</summary>
internal static class GreenSlimeExtensions
{
    /// <summary>Collects the <see cref="Item"/> contained within the <paramref name="debris"/>.</summary>
    /// <param name="slime">The <see cref="GreenSlime"/>.</param>
    /// <param name="debris">The <see cref="Debris"/>.</param>
    /// <returns><see langword="true"/> if the <see cref="Debris"/> was successfully collected, otherwise <see langword="false"/>.</returns>
    internal static bool CollectDebris(this GreenSlime slime, Debris debris)
    {
        Item? item;
        if (debris.item is not null)
        {
            item = debris.item;
            debris.item = null;
        }
        else if (!string.IsNullOrEmpty(debris.itemId.Value))
        {
            item = ItemRegistry.Create(debris.itemId.Value, 1, debris.itemQuality);
        }
        else
        {
            return false;
        }

        var inventory = slime.Get_Inventory();
        for (var i = 0; i < inventory.Count; i++)
        {
            if (inventory[i] is not null)
            {
                continue;
            }

            inventory[i] = item;
            debris.item = null;
            return true;
        }

        return false;
    }
}
