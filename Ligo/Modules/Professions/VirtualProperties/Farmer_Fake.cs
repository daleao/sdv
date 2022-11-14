namespace DaLion.Ligo.Modules.Professions.VirtualProperties;

#region using directives

using System.Runtime.CompilerServices;
using Netcode;

#endregion using directives

// ReSharper disable once InconsistentNaming
internal static class Farmer_Fake
{
    internal static ConditionalWeakTable<Farmer, NetBool> Values { get; } = new();

    internal static NetBool Get_IsFake(this Farmer farmer)
    {
        return Values.GetOrCreateValue(farmer);
    }

    // Net types are readonly
    internal static void Set_IsFake(this Farmer farmer, NetBool value)
    {
    }
}
