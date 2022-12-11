namespace DaLion.Ligo.Modules.Tweex;

#region using directives

using System.Collections.Generic;
using DaLion.Shared.Configs;
using Newtonsoft.Json;

#endregion using directives

/// <summary>The user-configurable settings for Tweex.</summary>
public sealed class TweexConfig : Config
{
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

    /// <summary>Gets a value indicating whether determines whether age-dependent qualities should be deterministic (true) or stochastic/random (false).</summary>
    [JsonProperty]
    public bool DeterministicAgeQuality { get; internal set; } = true;

    /// <summary>Gets a value indicating whether if wild forage rewards experience, berry bushes should qualify.</summary>
    [JsonProperty]
    public bool BerryBushesRewardExp { get; internal set; } = true;

    /// <summary>Gets a value indicating whether if fruit bat cave rewards experience, so should mushroom cave.</summary>
    [JsonProperty]
    public bool MushroomBoxesRewardExp { get; internal set; } = true;

    /// <summary>Gets a value indicating whether if crab pots reward experience, so should tappers.</summary>
    [JsonProperty]
    public bool TappersRewardExp { get; internal set; } = true;

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
