namespace DaLion.Harmonics.Framework.VirtualProperties;

#region using directives

using System.Runtime.CompilerServices;
using DaLion.Harmonics.Framework;
using StardewValley.Objects;

#endregion using directives

// ReSharper disable once InconsistentNaming
internal static class CombinedRing_Chord
{
    internal static ConditionalWeakTable<CombinedRing, Chord> Values { get; } = new();

    internal static Chord? Get_Chord(this CombinedRing combined)
    {
        return combined.ItemId == InfinityBandId && combined.combinedRings.Count > 1
            ? Values.GetValue(combined, Create)
            : null;
    }

    private static Chord Create(CombinedRing combined)
    {
        var first = Gemstone.FromRing(combined.combinedRings[0].QualifiedItemId);
        var second = Gemstone.FromRing(combined.combinedRings[1].QualifiedItemId);
        if (combined.combinedRings.Count == 2)
        {
            return new Chord(first, second);
        }

        var third = Gemstone.FromRing(combined.combinedRings[2].QualifiedItemId);
        if (combined.combinedRings.Count == 3)
        {
            return new Chord(first, second, third);
        }

        var fourth = Gemstone.FromRing(combined.combinedRings[3].QualifiedItemId);
        return new Chord(first, second, third, fourth);
    }
}
