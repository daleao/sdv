namespace DaLion.Stardew.Professions.Framework.VirtualProperties;

#region using directives

using Netcode;
using System.Runtime.CompilerServices;

#endregion using directives

public static class Farmer_HuntingTreasure
{
    internal static ConditionalWeakTable<Farmer, NetBool> Values = new();

    public static NetBool get_IsHuntingTreasure(this Farmer farmer) => Values.GetOrCreateValue(farmer);

    // Net types are readonly
    public static void set_IsHuntingTreasure(this Farmer farmer, NetBool newVal) { }
}