namespace DaLion.Ligo.Modules.Tools.Configs;

#region using directives

using Newtonsoft.Json;

#endregion using directives

/// <summary>Configs related to the <see cref="StardewValley.Tools.Pickaxe"/>.</summary>
public sealed class PickaxeConfig
{
    /// <summary>Gets a value indicating whether enables charging the Pick.</summary>
    [JsonProperty]
    public bool EnableCharging { get; internal set; } = true;

    /// <summary>Gets pickaxe must be at least this level to charge. Must be greater than zero.</summary>
    [JsonProperty]
    public UpgradeLevel RequiredUpgradeForCharging { get; internal set; } = UpgradeLevel.Copper;

    /// <summary>Gets the radius of affected tiles at each upgrade level.</summary>
    [JsonProperty]
    public int[] RadiusAtEachPowerLevel { get; internal set; } = { 1, 2, 3, 4, 5 };

    /// <summary>Gets a value indicating whether determines whether to break boulders and meteorites.</summary>
    [JsonProperty]
    public bool BreakBouldersAndMeteorites { get; internal set; } = true;

    /// <summary>Gets a value indicating whether determines whether to harvest spawned items in the mines.</summary>
    [JsonProperty]
    public bool HarvestMineSpawns { get; internal set; } = true;

    /// <summary>Gets a value indicating whether determines whether to break containers in the mine.</summary>
    [JsonProperty]
    public bool BreakMineContainers { get; internal set; } = true;

    /// <summary>Gets a value indicating whether determines whether to clear placed objects.</summary>
    [JsonProperty]
    public bool ClearObjects { get; internal set; } = false;

    /// <summary>Gets a value indicating whether determines whether to clear placed paths and flooring.</summary>
    [JsonProperty]
    public bool ClearFlooring { get; internal set; } = false;

    /// <summary>Gets a value indicating whether determines whether to clear tilled dirt.</summary>
    [JsonProperty]
    public bool ClearDirt { get; internal set; } = true;

    /// <summary>Gets a value indicating whether determines whether to clear live crops.</summary>
    [JsonProperty]
    public bool ClearLiveCrops { get; internal set; } = false;

    /// <summary>Gets a value indicating whether determines whether to clear dead crops.</summary>
    [JsonProperty]
    public bool ClearDeadCrops { get; internal set; } = true;

    /// <summary>Gets a value indicating whether determines whether to clear debris like stones, boulders and weeds.</summary>
    [JsonProperty]
    public bool ClearDebris { get; internal set; } = true;

    /// <summary>Gets a value indicating whether determines whether to play the shockwave animation when the charged Pick is released.</summary>
    [JsonProperty]
    public bool PlayShockwaveAnimation { get; internal set; } = true;

    /// <summary>Gets a value indicating whether determines whether the Pick can be enchanted with Reaching.</summary>
    [JsonProperty]
    public bool AllowReachingEnchantment { get; internal set; } = true;

    /// <summary>Gets a value indicating whether determines whether the Pick can be enchanted with Master.</summary>
    [JsonProperty]
    public bool AllowMasterEnchantment { get; internal set; } = true;
}
