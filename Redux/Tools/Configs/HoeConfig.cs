namespace DaLion.Redux.Tools.Configs;

#region using directives

using Newtonsoft.Json;

#endregion using directives

/// <summary>Configs related to the <see cref="StardewValley.Tools.Hoe"/>.</summary>
public sealed class HoeConfig
{
    /// <summary>Gets the area of affected tiles at each power level for the Hoe, in units lengths x units radius.</summary>
    /// <remarks>Note that radius extends to both sides of the farmer.</remarks>
    [JsonProperty]
    public int[][] AffectedTiles { get; internal set; } =
    {
        new[] { 3, 0 },
        new[] { 5, 0 },
        new[] { 3, 1 },
        new[] { 6, 1 },
        new[] { 5, 2 },
    };

    /// <summary>
    ///     Gets a value indicating whether determines whether to apply custom tile area for the Hoe. Keep this at false if using defaults to improve
    ///     performance.
    /// </summary>
    [JsonProperty]
    public bool OverrideAffectedTiles { get; internal set; } = false;

    /// <summary>Gets a value indicating whether determines whether the Hoe can be enchanted with Master.</summary>
    [JsonProperty]
    public bool AllowMasterEnchantment { get; internal set; } = true;
}
