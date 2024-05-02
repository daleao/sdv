namespace DaLion.Professions.Framework.VirtualProperties;

#region using directives

using System.Runtime.CompilerServices;
using Netcode;

#endregion using directives

// ReSharper disable once InconsistentNaming
internal static class Farmer_TreasureHunt
{
    internal static ConditionalWeakTable<Farmer, NetBool> HuntingState { get; } = [];

    internal static NetBool Get_IsHuntingTreasure(this Farmer farmer)
    {
        return HuntingState.GetOrCreateValue(farmer);
    }

    // Net types are readonly
    internal static void Set_IsHuntingTreasure(this Farmer farmer, NetBool value)
    {
    }
}
