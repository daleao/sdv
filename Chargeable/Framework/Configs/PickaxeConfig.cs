namespace DaLion.Chargeable.Framework.Configs;

#region using directives

using DaLion.Shared.Integrations.GMCM.Attributes;
using Newtonsoft.Json;
using StardewValley.Tools;

#endregion using directives

/// <summary>Configs related to the <see cref="StardewValley.Tools.Pickaxe"/>.</summary>
public sealed class PickaxeConfig
{
    /// <summary>Gets a value indicating whether enables charging the Pick.</summary>
    [JsonProperty]
    [GMCMPriority(0)]
    public bool EnableCharging { get; internal set; } = true;

    /// <summary>Gets the radius of affected tiles at each upgrade level.</summary>
    [JsonProperty]
    [GMCMPriority(1)]
    [GMCMOverride(typeof(ChargeableConfigMenu), "PickaxeRadiusAtEachLevelOverride")]
    public uint[] RadiusAtEachPowerLevel { get; internal set; } = [1, 2, 3, 4, 5, 6];

    /// <summary>Gets a value indicating whether to break boulders and meteorites.</summary>
    [JsonProperty]
    [GMCMPriority(100)]
    public bool BreakBouldersAndMeteorites { get; internal set; } = true;

    /// <summary>Gets a value indicating whether to harvest spawned items in the mines.</summary>
    [JsonProperty]
    [GMCMPriority(101)]
    public bool HarvestMineSpawns { get; internal set; } = true;

    /// <summary>Gets a value indicating whether to break containers in the mine.</summary>
    [JsonProperty]
    [GMCMPriority(102)]
    public bool BreakMineContainers { get; internal set; } = true;

    /// <summary>Gets a value indicating whether to clear placed objects.</summary>
    [JsonProperty]
    [GMCMPriority(103)]
    public bool ClearObjects { get; internal set; } = false;

    /// <summary>Gets a value indicating whether to clear placed paths and flooring.</summary>
    [JsonProperty]
    [GMCMPriority(104)]
    public bool ClearFlooring { get; internal set; } = false;

    /// <summary>Gets a value indicating whether to clear tilled dirt.</summary>
    [JsonProperty]
    [GMCMPriority(105)]
    public bool ClearDirt { get; internal set; } = true;

    /// <summary>Gets a value indicating whether to clear live crops.</summary>
    [JsonProperty]
    [GMCMPriority(106)]
    public bool ClearLiveCrops { get; internal set; } = false;

    /// <summary>Gets a value indicating whether to clear dead crops.</summary>
    [JsonProperty]
    [GMCMPriority(107)]
    public bool ClearDeadCrops { get; internal set; } = true;

    /// <summary>Gets a value indicating whether to clear debris like stones, boulders and weeds.</summary>
    [JsonProperty]
    [GMCMPriority(108)]
    public bool ClearDebris { get; internal set; } = true;

    /// <summary>Gets a value indicating whether the Pick can be enchanted with Reaching.</summary>
    [JsonProperty]
    [GMCMPriority(200)]
    public bool AllowReachingEnchantment { get; internal set; } = true;

    /// <summary>Gets the multiplier to base stamina consumed by the <see cref="Axe"/>.</summary>
    [JsonProperty]
    [GMCMPriority(201)]
    [GMCMRange(0f, 2f, 0.05f)]
    public float StaminaCostMultiplier { get; internal set; } = 1f;
}
