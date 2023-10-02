namespace DaLion.Overhaul.Modules.Tools.Configs;

#region using directives

using Newtonsoft.Json;
using StardewValley.Tools;

#endregion using directives

/// <summary>Configs related to the <see cref="StardewValley.Tools.Hoe"/>.</summary>
public sealed class HoeConfig
{
    /// <summary>Gets the multiplier to base stamina consumed by the <see cref="Axe"/>.</summary>
    [JsonProperty]
    public float BaseStaminaCostMultiplier { get; internal set; } = 1f;

    /// <summary>Gets the area of affected tiles at each power level for the Hoe, in units lengths x units radius.</summary>
    /// <remarks>Note that radius extends to both sides of the farmer.</remarks>
    [JsonProperty]
    public (uint Length, uint Radius)[] AffectedTilesAtEachPowerLevel { get; internal set; } =
    {
        (3, 0),
        (5, 0),
        (3, 1),
        (6, 1),
        (7, 2),
        (8, 3),
        (9, 4),
    };

    /// <summary>Gets a value indicating whether the Hoe can be enchanted with Master.</summary>
    [JsonProperty]
    public bool AllowMasterEnchantment { get; internal set; } = true;
}
