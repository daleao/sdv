namespace DaLion.Stardew.Rings.Extensions;

#region using directives

using Framework;
using StardewValley.Objects;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

#endregion using directives

/// <summary>Extensions for the <see cref="CombinedRing"/> class.</summary>
public static class CombinedRingExtensions
{
    /// <summary>Whether the combined ring is a resonant Iridium Band.</summary>
    /// <param name="resonance">The resonant gemstone, if any.</param>
    public static bool IsResonant(this CombinedRing combined, [NotNullWhen(true)] out Resonance? resonance)
    {
        resonance = null;
        if (combined.ParentSheetIndex != Constants.IRIDIUM_BAND_INDEX_I || combined.combinedRings.Count != 4 ||
            combined.combinedRings.Any(r =>
                r.ParentSheetIndex != combined.combinedRings.First().ParentSheetIndex))
            return false;

        return Resonance.TryFromValue(combined.combinedRings.First().ParentSheetIndex, out resonance);
    }
}