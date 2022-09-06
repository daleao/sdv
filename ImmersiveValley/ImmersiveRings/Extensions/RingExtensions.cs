namespace DaLion.Stardew.Rings.Extensions;

#region using directives

using StardewValley.Objects;
using System.Diagnostics.CodeAnalysis;

#endregion using directives

/// <summary>Extensions for the <see cref="Ring"/> class.</summary>
public static class RingExtensions
{
    /// <summary>Whether the ring is any of the gemstone rings. </summary>
    public static bool IsGemRing(this Ring ring) =>
        ring.ParentSheetIndex is
            Constants.RUBY_RING_INDEX_I or
            Constants.AQUAMARINE_RING_INDEX_I or
            Constants.JADE_RING_INDEX_I or
            Constants.EMERALD_RING_INDEX_I or
            Constants.AMETHYST_RING_INDEX_I or
            Constants.TOPAZ_RING_INDEX_I ||
        ring.ParentSheetIndex == ModEntry.GarnetRingIndex;

    /// <summary>Check whether the ring is a combined Iridium Band and if so cast to <see cref="CombinedRing"/>.</summary>
    /// <param name="iridium">The ring as a <see cref="CombinedRing"/> instance.</param>
    /// <returns><see langword="true"/> if the ring can be casted to <see cref="CombinedRing"/>, it's index is that of Iridum Band and it contains combined gemstone rings, otherwise <see langword="false"/>.</returns>
    public static bool IsCombinedIridiumBand(this Ring ring, [NotNullWhen(true)] out CombinedRing? iridium)
    {
        if (ring is CombinedRing { ParentSheetIndex: Constants.IRIDIUM_BAND_INDEX_I } combined &&
            combined.combinedRings.Count > 0)
            iridium = combined;
        else
            iridium = null;
            
        return iridium is not null;
    }
}