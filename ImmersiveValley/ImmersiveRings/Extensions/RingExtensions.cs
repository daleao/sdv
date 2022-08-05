namespace DaLion.Stardew.Rings.Extensions;

#region using directives

using StardewValley.Objects;

#endregion using directives

/// <summary>Extensions for the <see cref="Ring"/> class.</summary>
public static class RingExtensions
{
    /// <summary>Whether the ring is any of the gemstone rings. </summary>
    public static bool IsGemRing(this Ring ring) =>
        ring.ParentSheetIndex is Constants.RUBY_RING_INDEX_I or Constants.AQUAMARINE_RING_INDEX_I
            or Constants.JADE_RING_INDEX_I or Constants.EMERALD_RING_INDEX_I or Constants.AMETHYST_RING_INDEX_I
            or Constants.TOPAZ_RING_INDEX_I || ring.ParentSheetIndex == ModEntry.GarnetRingIndex;
}