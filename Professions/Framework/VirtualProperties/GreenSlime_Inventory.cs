namespace DaLion.Professions.Framework.VirtualProperties;

#region using directives

using System.Runtime.CompilerServices;
using StardewValley.Inventories;
using StardewValley.Monsters;

#endregion using directives

// ReSharper disable once InconsistentNaming
internal static class GreenSlime_Inventory
{
    internal static ConditionalWeakTable<GreenSlime, Inventory> Values { get; } = [];

    internal static IInventory Get_Inventory(this GreenSlime slime)
    {
        return Values.GetValue(slime, Create);
    }

    internal static bool Get_HasInventorySlots(this GreenSlime slime)
    {
        return Values.GetValue(slime, Create).HasEmptySlots();
    }

    private static Inventory Create(GreenSlime _)
    {
        var inventory = new Inventory();
        for (var i = 0; i < 10; i++)
        {
            inventory.Add(null);
        }

        return inventory;
    }
}
