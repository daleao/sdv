namespace DaLion.Overhaul.Modules.Rings.VirtualProperties;

#region using directives

using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using DaLion.Overhaul.Modules.Rings.Integrations;
using DaLion.Overhaul.Modules.Rings.Resonance;
using DaLion.Shared.Extensions;
using DaLion.Shared.Extensions.Collections;
using StardewValley;
using StardewValley.Objects;

#endregion using directives

// ReSharper disable once InconsistentNaming
internal static class Farmer_ResonatingChords
{
    internal static ConditionalWeakTable<Farmer, List<Chord>> ChordsByFarmer { get; } = new();

    internal static List<Chord> Get_ResonatingChords(this Farmer farmer)
    {
        return ChordsByFarmer.GetValue(farmer, GetChords);
    }

    private static List<Chord> GetChords(Farmer farmer)
    {
        var rings = WearMoreRingsIntegration.Api?.GetAllRings(farmer) ??
                    farmer.leftRing.Value.Collect(farmer.rightRing.Value);
        return rings.OfType<CombinedRing>().Select(ring => ring.Get_Chord()).WhereNotNull().ToList();
    }
}
