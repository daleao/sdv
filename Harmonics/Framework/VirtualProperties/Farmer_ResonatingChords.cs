namespace DaLion.Harmonics.Framework.VirtualProperties;

#region using directives

using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using DaLion.Harmonics.Framework.Integrations;
using DaLion.Shared.Extensions;
using DaLion.Shared.Extensions.Collections;
using StardewValley.Objects;

#endregion using directives

// ReSharper disable once InconsistentNaming
internal static class Farmer_ResonatingChords
{
    internal static ConditionalWeakTable<Farmer, List<Chord>> Values { get; } = new();

    internal static List<Chord> Get_ResonatingChords(this Farmer farmer)
    {
        return Values.GetValue(farmer, Create);
    }

    private static List<Chord> Create(Farmer farmer)
    {
        IEnumerable<Ring> rings;
        if (WearMoreRingsIntegration.Instance?.IsLoaded == true)
        {
            rings = [];
            WearMoreRingsIntegration.Instance.AssertLoaded();
            for (var slot = 0; slot < WearMoreRingsIntegration.Instance.ModApi.RingSlotCount(); slot++)
            {
                rings = WearMoreRingsIntegration.Instance.ModApi.GetRing(slot).Collect(rings);
            }
        }
        else
        {
            rings = farmer.leftRing.Value.Collect(farmer.rightRing.Value);
        }

        return rings.OfType<CombinedRing>().Select(ring => ring.Get_Chord()).WhereNotNull().ToList();
    }
}
