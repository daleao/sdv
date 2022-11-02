namespace DaLion.Redux.Framework.Professions.VirtualProperties;

#region using directives

using System.Runtime.CompilerServices;
using Netcode;

#endregion using directives

// ReSharper disable once InconsistentNaming
internal static class Farmer_HuntingTreasure
{
    internal static ConditionalWeakTable<Farmer, NetBool> Values { get; } = new();

    internal static NetBool Get_IsHuntingTreasure(this Farmer farmer)
    {
        return Values.GetOrCreateValue(farmer);
    }

    // Net types are readonly
    internal static void Set_IsHuntingTreasure(this Farmer farmer, NetBool value)
    {
    }
}
