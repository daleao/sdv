namespace DaLion.Stardew.Rings.Framework.VirtualProperties;

#region using directives

using Common.Extensions;
using StardewValley.Objects;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

#endregion using directives

internal static class CombinedRing_Resonances
{
    internal static ConditionalWeakTable<CombinedRing, Dictionary<Resonance, int>?> Values = new();

    internal static IEnumerable<KeyValuePair<Resonance, int>> get_Resonances(this CombinedRing combined) =>
        Values.GetValue(combined, Create) ?? Enumerable.Empty<KeyValuePair<Resonance, int>>();

    private static Dictionary<Resonance, int>? Create(CombinedRing combined)
    {
        if (combined.ParentSheetIndex != Constants.IRIDIUM_BAND_INDEX_I || combined.combinedRings.Count < 2)
            return null;
        
        var resonances = new Dictionary<Resonance, int>
        {
            { Resonance.Amethyst, 0 },
            { Resonance.Topaz, 0 },
            { Resonance.Aquamarine, 0 },
            { Resonance.Jade, 0 },
            { Resonance.Emerald, 0 },
            { Resonance.Ruby, 0 },
            { Resonance.Garnet, 0 }
        };

        var first = combined.combinedRings[0].ParentSheetIndex;
        var second = combined.combinedRings[1].ParentSheetIndex;
        if (Resonance.TryFromValue(first, out var r1))
        {
            if (first == second)
            {
                resonances[r1] += 2;
            }
            else if (Resonance.TryFromValue(second, out var r2))
            {
                if (r2 == r1.Pair)
                {
                    ++resonances[r1];
                    ++resonances[r2];
                }
                else if (r2 == r1.Antipair)
                {
                    --resonances[r1];
                    --resonances[r2];
                }
            }
        }

        if (combined.combinedRings.Count < 4)
            return resonances.Where(pair => pair.Value > 0).ToDictionary(i => i.Key, i => i.Value);

        var third = combined.combinedRings[2].ParentSheetIndex;
        var fourth = combined.combinedRings[3].ParentSheetIndex;
        if (Resonance.TryFromValue(third, out var r3))
        {
            if (third == fourth)
            {
                resonances[r3] += 2;
            }
            else if (Resonance.TryFromValue(fourth, out var r4))
            {
                if (r4 == r3.Pair)
                {
                    ++resonances[r3];
                    ++resonances[r4];
                }
                else if (r4 == r3.Antipair)
                {
                    --resonances[r3];
                    --resonances[r4];
                }
            }
        }

        if (first.Collect(second, third, fourth).Distinct().Count() == 1) resonances[r1] += 2;

        return resonances.Where(pair => pair.Value > 0).ToDictionary(i => i.Key, i => i.Value);
    }
}