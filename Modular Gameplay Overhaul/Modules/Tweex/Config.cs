namespace DaLion.Overhaul.Modules.Tweex;

#region using directives

using System.Collections.Generic;
using Newtonsoft.Json;

#endregion using directives

/// <summary>The user-configurable settings for Tweex.</summary>
public sealed class Config : Shared.Configs.Config
{
    #region quality settings

    /// <summary>Gets the degree to which Bee House age improves honey quality.</summary>
    [JsonProperty]
    public float BeeHouseAgingFactor { get; internal set; } = 1f;

    /// <summary>Gets the degree to which Mushroom Box age improves mushroom quality.</summary>
    [JsonProperty]
    public float MushroomBoxAgingFactor { get; internal set; } = 1f;

    /// <summary>Gets the degree to which Tree age improves sap quality.</summary>
    [JsonProperty]
    public float TreeAgingFactor { get; internal set; } = 1f;

    /// <summary>Gets the degree to which Fruit Tree age improves fruit quality.</summary>
    [JsonProperty]
    public float FruitTreeAgingFactor { get; internal set; } = 1f;

    /// <summary>Gets the degree to which Tea Bush age improves Tea Leaf quality.</summary>
    [JsonProperty]
    public float TeaBushAgingFactor { get; internal set; } = 1f;

    /// <summary>Gets a value indicating whether determines whether age-dependent qualities should be deterministic (true) or stochastic/random (false).</summary>
    [JsonProperty]
    public bool DeterministicAgeQuality { get; internal set; } = true;

    #endregion quality settings

    #region exp settings

    /// <summary>Gets the amount of Foraging experience rewarded for harvesting berry bushes. Set to zero to disable.</summary>
    [JsonProperty]
    public uint BerryBushExpReward { get; internal set; } = 5;

    /// <summary>Gets the amount of Foraging experience rewarded for harvesting Mushroom Boxes. Set to zero to disable.</summary>
    [JsonProperty]
    public uint MushroomBoxExpReward { get; internal set; } = 1;

    /// <summary>Gets the amount of Foraging experience rewarded for harvesting Tappers. Set to zero to disable.</summary>
    [JsonProperty]
    public uint TapperExpReward { get; internal set; } = 5;

    #endregion exp settings

    /// <summary>Gets a value indicating whether if regular trees can't grow in winter, neither should fruit trees.</summary>
    [JsonProperty]
    public bool PreventFruitTreeGrowthInWinter { get; internal set; } = true;

    /// <summary>Gets a value indicating whether large input products should yield more processed output instead of higher quality.</summary>
    [JsonProperty]
    public bool LargeProducsYieldQuantityOverQuality { get; internal set; } = true;

    /// <summary>
    ///     Gets add custom mod Artisan machines to this list to make them compatible with
    ///     LargeProductsYieldQuantityOverQuality.
    /// </summary>
    [JsonProperty]
    public HashSet<string> DairyArtisanMachines { get; internal set; } = new()
    {
        "Butter Churn", // artisan valley
        "Ice Cream Machine", // artisan valley
        "Keg", // vanilla
        "Yogurt Jar", // artisan valley
    };

    /// <summary>Gets a value indicating whether the Mill's output should consider the quality of the ingredient.</summary>
    [JsonProperty]
    public bool MillsPreserveQuality { get; internal set; } = true;

    /// <summary>Gets a value indicating whether bombs within any explosion radius are immediately triggered.</summary>
    [JsonProperty]
    public bool ExplosionTriggeredBombs { get; internal set; } = true;

    /// <summary>Gets a value indicating whether to set the quality of legendary fish at best.</summary>
    [JsonProperty]
    public bool LegendaryFishAlwaysBestQuality { get; internal set; } = true;
}
