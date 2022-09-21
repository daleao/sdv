namespace DaLion.Stardew.Rings.Framework.VirtualProperties;

#region using directives

using System.Runtime.CompilerServices;
using DaLion.Stardew.Rings.Framework.Resonance;
using StardewValley.Objects;

#endregion using directives

// ReSharper disable once InconsistentNaming
internal static class CombinedRing_Chord
{
    internal static ConditionalWeakTable<CombinedRing, IChord> Values { get; } = new();

    internal static IChord? Get_Chord(this CombinedRing combined)
    {
        return combined is { ParentSheetIndex: Constants.IridiumBandIndex, combinedRings.Count: >= 2 }
            ? Values.GetValue(combined, Create)
            : null;
    }

    private static IChord Create(CombinedRing combined)
    {
        var first = Gemstone.FromRing(combined.combinedRings[0].ParentSheetIndex);
        var second = Gemstone.FromRing(combined.combinedRings[1].ParentSheetIndex);
        if (combined.combinedRings.Count == 2)
        {
            return new Chord(first, second);
        }

        var third = Gemstone.FromRing(combined.combinedRings[2].ParentSheetIndex);
        if (combined.combinedRings.Count == 3)
        {
            return new Chord(first, second, third);
        }

        var fourth = Gemstone.FromRing(combined.combinedRings[3].ParentSheetIndex);
        return new Chord(first, second, third, fourth);
    }
}
