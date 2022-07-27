namespace DaLion.Stardew.Professions.Framework.VirtualProperties;

#region using directives

using Netcode;
using StardewValley;
using System.Runtime.CompilerServices;

#endregion using directives

public static class Farmer_HuntingTreasure
{
    internal static ConditionalWeakTable<Farmer, NetBool> Values = new();

    public static NetBool get_IsHuntingTreasure(this Farmer farmer)
    {
        return Values.GetOrCreateValue(farmer);
    }

    public static void set_IsHuntingTreasure(this Farmer farmer, NetBool newVal)
    {
        // Net types should not have a setter as they are readonly
    }
}