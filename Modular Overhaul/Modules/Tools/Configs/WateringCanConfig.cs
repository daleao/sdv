namespace DaLion.Overhaul.Modules.Tools.Configs;

#region using directives

using Newtonsoft.Json;
using StardewValley.Tools;

#endregion using directives

/// <summary>Configs related to the <see cref="StardewValley.Tools.WateringCan"/>.</summary>
public sealed class WateringCanConfig
{
    /// <summary>Gets the multiplier to base stamina consumed by the <see cref="Axe"/>.</summary>
    [JsonProperty]
    public float BaseStaminaCostMultiplier { get; internal set; } = 1f;

    /// <summary>Gets the area of affected tiles at each power level for the Can, in units lengths x units radius.</summary>
    /// <remarks>Note that radius extends to both sides of the farmer.</remarks>
    [JsonProperty]
    public (uint Length, uint Radius)[] AffectedTilesAtEachPowerLevel { get; internal set; } =
    {
        (3, 0),
        (5, 0),
        (3, 1),
        (6, 1),
        (5, 2),
        (7, 3),
        (9, 4),
    };

    /// <summary>Gets a value indicating whether the Watering Can can be enchanted with Master.</summary>
    [JsonProperty]
    public bool AllowMasterEnchantment { get; internal set; } = true;

    /// <summary>Gets a value indicating whether the Watering Can can be enchanted with Swift.</summary>
    [JsonProperty]
    public bool AllowSwiftEnchantment { get; internal set; } = true;

    /// <summary>Gets the chance to reward farming experience when watering a cropped tile.</summary>
    [JsonProperty]
    public float ExpRewardChance { get; internal set; } = 0.5f;

    /// <summary>Gets the amount of farming experience rewarded for watering a cropped tile.</summary>
    [JsonProperty]
    public int ExpRewardAmount { get; internal set; } = 1;

    /// <summary>Gets a value indicating whether to prevent refilling the can with salt or ocean water.</summary>
    [JsonProperty]
    public bool PreventRefillWithSaltWater { get; internal set; } = true;
}
