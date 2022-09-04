namespace DaLion.Stardew.Rings.Extensions;

#region using directives

using Common.Extensions;
using Framework;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Linq;

#endregion using directives

/// <summary>Extensions for the <see cref="CombinedRing"/> class.</summary>
public static class CombinedRingExtensions
{
    /// <summary>Check the combined ring for any resonances.</summary>
    public static IEnumerable<Resonance> CheckResonances(this CombinedRing combined)
    {
        var resonances = new Dictionary<int, Resonance>
        {
            {Resonance.Amethyst, Resonance.Amethyst},
            {Resonance.Topaz, Resonance.Topaz},
            {Resonance.Aquamarine, Resonance.Aquamarine},
            {Resonance.Jade, Resonance.Jade},
            {Resonance.Emerald, Resonance.Emerald},
            {Resonance.Ruby, Resonance.Ruby},
            {Resonance.Garnet, Resonance.Garnet}
        };

        if (combined.ParentSheetIndex != Constants.IRIDIUM_BAND_INDEX_I || combined.combinedRings.Count < 2)
            return Enumerable.Empty<Resonance>();

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
                if (r2 == r1.GetPair())
                {
                    ++resonances[r1];
                    ++resonances[r2];
                }
                else if (r2 == r1.GetAntipair())
                {
                    --resonances[r1];
                    --resonances[r2];
                }
            }
        }

        if (combined.combinedRings.Count >= 4)
        {
            var third = combined.combinedRings[0].ParentSheetIndex;
            var fourth = combined.combinedRings[1].ParentSheetIndex;
            if (Resonance.TryFromValue(third, out var r3))
            {
                if (third == fourth)
                {
                    resonances[r3] += 2;
                }
                else if (Resonance.TryFromValue(fourth, out var r4))
                {
                    if (r4 == r3.GetPair())
                    {
                        ++resonances[r3];
                        ++resonances[r4];
                    }
                    else if (r4 == r3.GetAntipair())
                    {
                        --resonances[r3];
                        --resonances[r4];
                    }
                }
            }

            if (first.Collect(second, third, fourth).Distinct().Count() == 1)
                resonances[first] += 2;

        }

        return resonances.Values;
    }
}