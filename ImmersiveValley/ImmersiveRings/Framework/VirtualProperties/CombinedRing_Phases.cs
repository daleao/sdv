namespace DaLion.Stardew.Rings.Framework.VirtualProperties;

#region using directives

using Common.Extensions;
using StardewValley.Objects;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

#endregion using directives

internal static class CombinedRing_Phases
{
    internal class Holder : IEnumerable<Phase?>
    {
        public Phase? VerticalPhase;
        public Phase? HorizontalPhase;

        public IEnumerator<Phase?> GetEnumerator() => VerticalPhase.Collect(HorizontalPhase).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    internal static ConditionalWeakTable<CombinedRing, Holder?> Values = new();

    internal static IEnumerable<Phase?> get_Phases(this CombinedRing combined) =>
        Values.GetValue(combined, Create) ?? Enumerable.Empty<Phase?>();

    private static Holder? Create(CombinedRing combined)
    {
        if (combined.ParentSheetIndex != Constants.IRIDIUM_BAND_INDEX_I || combined.combinedRings.Count < 2)
            return null;

        var first = combined.combinedRings[0].ParentSheetIndex;
        if (!Resonance.TryFromValue(first, out var r1)) return null;
        
        var second = combined.combinedRings[1].ParentSheetIndex;
        var holder = new Holder();
        if (combined.combinedRings.Count < 4)
        {
            if (first == second)
                holder.VerticalPhase = new(r1, r1, 1);
            else if (Resonance.TryFromValue(second, out var r2) && r2 == r1.Pair)
                holder.VerticalPhase = new(r1, r2, 1);
        }
        else
        {
            var third = combined.combinedRings[2].ParentSheetIndex;
            var fourth = combined.combinedRings[3].ParentSheetIndex;
            if (first == second)
            {
                if (first == third && third == fourth)
                {
                    holder.VerticalPhase = new(r1, r1, 2);
                    return holder;
                }
                
                holder.VerticalPhase = new(r1, r1, 1);
            }
            else if (Resonance.TryFromValue(second, out var r2) && r2 == r1.Pair)
            {
                holder.VerticalPhase = new(r1, r2, 1);
            }

            if (!Resonance.TryFromValue(third, out var r3)) return holder;

            if (third == fourth)
                holder.HorizontalPhase = new(r3, r3, 1);
            else if (Resonance.TryFromValue(fourth, out var r4) && r4 == r3.Pair)
                holder.HorizontalPhase = new(r3, r4, 1);
        }

        return holder;
    }
}