namespace DaLion.Harmonics.Framework.VirtualProperties;

#region using directives

using System.Runtime.CompilerServices;
using Netcode;

#endregion using directives

// ReSharper disable once InconsistentNaming
internal static class Farmer_CooldownReduction
{
    internal static ConditionalWeakTable<Farmer, NetFloat> Values { get; } = new();

    internal static NetFloat Get_CooldownReduction(this Farmer farmer)
    {
        return Values.GetOrCreateValue(farmer);
    }

    // Net types are readonly
    internal static void Set_CooldownReduction(this Farmer farmer, NetFloat value)
    {
    }
}
