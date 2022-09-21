namespace DaLion.Stardew.Tools.Configs;

/// <summary>Configs related to the <see cref="StardewValley.Tools.WateringCan"/>.</summary>
public sealed class WateringCanConfig
{
    /// <summary>Gets or sets the area of affected tiles at each power level for the WateringCan, in units lengths x units radius.</summary>
    /// <remarks>Note that radius extends to both sides of the farmer.</remarks>
    public int[][] AffectedTiles { get; set; } = { new[] { 3, 0 }, new[] { 5, 0 }, new[] { 3, 1 }, new[] { 6, 1 }, new[] { 5, 2 } };

    /// <summary>Gets or sets a value indicating whether the Watering Can can be enchanted with Swift.</summary>
    public bool AllowSwiftEnchantment { get; set; } = true;

    /// <summary>Gets or sets a value indicating whether use custom tile area for the Watering Can. Keep this at false if using defaults to improve performance.</summary>
    public bool OverrideAffectedTiles { get; set; } = false;

    /// <summary>Gets or sets a value indicating whether determines whether the Watering Can can be enchanted with Master.</summary>
    public bool AllowMasterEnchantment { get; set; } = true;
}
