namespace DaLion.Ligo.Modules.Rings.VirtualProperties;

#region using directives

using System.Collections.Generic;
using System.Runtime.CompilerServices;
using DaLion.Ligo.Modules.Rings.Resonance;

#endregion using directives

// ReSharper disable once InconsistentNaming
internal static class Farmer_Resonances
{
    internal static ConditionalWeakTable<Farmer, List<Chord>> Values { get; } = new();

    internal static List<Chord> Get_ResonatingChords(this Farmer farmer)
    {
        return Values.GetOrCreateValue(farmer);
    }
}
