namespace DaLion.Overhaul.Modules.Tweex;

#region using directives

using System.Collections.Generic;
using Newtonsoft.Json;

#endregion using directives

/// <summary>The user-configurable settings for TWX.</summary>
public sealed class Config : Shared.Configs.Config
{
    /// <summary>Gets a value indicating whether determines whether age-dependent qualities should be deterministic (true) or stochastic/random (false).</summary>
    [JsonProperty]
    public bool DeterministicAgeQuality { get; internal set; } = false;

    #region farming

    /// <summary>Gets the chance a crop may wither per day left un-watered.</summary>
    [JsonProperty]
    public float CropWitherChance { get; internal set; } = 0.1f;

    /// <summary>Gets a value indicating whether fruit trees should be prevented from growing in winter, as for regular trees.</summary>
    [JsonProperty]
    public bool PreventFruitTreeWinterGrowth { get; internal set; } = true;

    /// <summary>Gets the degree to which Bee House age improves honey quality.</summary>
    [JsonProperty]
    public float BeeHouseAgingFactor { get; internal set; } = 1f;

    /// <summary>Gets the degree to which Tea Bush age improves Tea Leaf quality.</summary>
    [JsonProperty]
    public float TeaBushAgingFactor { get; internal set; } = 1f;

    /// <summary>Gets a value indicating whether large eggs and milk should yield twice the output stack instead of higher quality.</summary>
    [JsonProperty]
    public bool ImmersiveDairyYield { get; internal set; } = true;

    /// <summary>
    ///     Gets a list of Artisan machines which should be compatible with
    ///     LargeDairyYieldsQuantityOverQuality.
    /// </summary>
    [JsonProperty]
    public HashSet<string> DairyArtisanMachines { get; internal set; } = new()
    {
        "Butter Churn", // artisan valley
        "Ice Cream Machine", // artisan valley
        "Keg", // vanilla, but artisan valley adds "kefir" item
        "Yogurt Jar", // artisan valley
    };

    /// <summary>Gets a set of maps in which to attempt to spawn crows.</summary>
    [JsonProperty]
    public HashSet<string> SpawnCrowsOnTheseMaps { get; internal set; } = new()
    {
        "IslandWest",
        "Custom_Garden",
        "Custom_GrampletonFields",
        "Custom_GrampletonFields_Small",
        "Custom_Ridgeside_SummitFarm",
        "Custom_ESMeadowFarm",
    };

    #endregion farming

    #region foraging

    /// <summary>Gets the degree to which Tree age improves sap quality.</summary>
    [JsonProperty]
    public float TreeAgingFactor { get; internal set; } = 1f;

    /// <summary>Gets the degree to which Fruit Tree age improves fruit quality.</summary>
    [JsonProperty]
    public float FruitTreeAgingFactor { get; internal set; } = 1f;

    /// <summary>Gets the amount of Foraging experience rewarded for harvesting berry bushes. Set to zero to disable.</summary>
    [JsonProperty]
    public uint BerryBushExpReward { get; internal set; } = 5;

    /// <summary>Gets the degree to which Mushroom Box age improves mushroom quality.</summary>
    [JsonProperty]
    public float MushroomBoxAgingFactor { get; internal set; } = 1f;

    /// <summary>Gets the amount of Foraging experience rewarded for harvesting Mushroom Boxes. Set to zero to disable.</summary>
    [JsonProperty]
    public uint MushroomBoxExpReward { get; internal set; } = 1;

    /// <summary>Gets the amount of Foraging experience rewarded for harvesting Tappers. Set to zero to disable.</summary>
    [JsonProperty]
    public uint TapperExpReward { get; internal set; } = 2;

    #endregion foraging

    #region fishing

    /// <summary>Gets a value indicating whether to preserve the bait when a Crab Pot produces trash.</summary>
    [JsonProperty]
    public bool TrashDoesNotConsumeBait { get; internal set; } = true;

    #endregion fishing

    #region mining

    /// <summary>Gets a value indicating whether bombs within any explosion radius are immediately triggered.</summary>
    [JsonProperty]
    public bool ChainExplosions { get; internal set; } = true;

    /// <summary>Gets a value indicating whether to impadd new mining recipes for crafting Glow and Magnet rings.</summary>
    [JsonProperty]
    public bool ImmersiveGlowstoneProgression { get; internal set; } = true;

    #endregion mining
}
