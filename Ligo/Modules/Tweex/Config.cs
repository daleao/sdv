namespace DaLion.Ligo.Modules.Tweex;

#region using directives

using System.Collections.Generic;

#endregion using directives

/// <summary>The user-configurable settings for Tweex.</summary>
public sealed class Config
{
    /// <summary>Gets or sets the degree to which Bee House age improves honey quality.</summary>
    public float BeeHouseAgingFactor { get; set; } = 1f;

    /// <summary>Gets or sets the degree to which Mushroom Box age improves mushroom quality.</summary>
    public float MushroomBoxAgingFactor { get; set; } = 1f;

    /// <summary>Gets or sets the degree to which Tree age improves sap quality.</summary>
    public float TreeAgingFactor { get; set; } = 1f;

    /// <summary>Gets or sets the degree to which Fruit Tree age improves fruit quality.</summary>
    public float FruitTreeAgingFactor { get; set; } = 1f;

    /// <summary>Gets or sets a value indicating whether determines whether age-dependent qualities should be deterministic (true) or stochastic/random (false).</summary>
    public bool DeterministicAgeQuality { get; set; } = true;

    /// <summary>Gets or sets a value indicating whether if wild forage rewards experience, berry bushes should qualify.</summary>
    public bool BerryBushesRewardExp { get; set; } = true;

    /// <summary>Gets or sets a value indicating whether if fruit bat cave rewards experience, so should mushroom cave.</summary>
    public bool MushroomBoxesRewardExp { get; set; } = true;

    /// <summary>Gets or sets a value indicating whether if crab pots reward experience, so should tappers.</summary>
    public bool TappersRewardExp { get; set; } = true;

    /// <summary>Gets or sets a value indicating whether if regular trees can't grow in winter, neither should fruit trees.</summary>
    public bool PreventFruitTreeGrowthInWinter { get; set; } = true;

    /// <summary>Gets or sets a value indicating whether large input products should yield more processed output instead of higher quality.</summary>
    public bool LargeProducsYieldQuantityOverQuality { get; set; } = true;

    /// <summary>
    ///     Gets or sets add custom mod Artisan machines to this list to make them compatible with
    ///     LargeProducsYieldQuantityOverQuality.
    /// </summary>
    public HashSet<string> DairyArtisanMachines { get; set; } = new()
    {
        "Butter Churn", // artisan valley
        "Ice Cream Machine", // artisan valley
        "Keg", // vanilla
        "Yogurt Jar", // artisan valley
    };

    /// <summary>Gets or sets a value indicating whether the Mill's output should consider the quality of the ingredient.</summary>
    public bool MillsPreserveQuality { get; set; } = true;

    /// <summary>Gets or sets a value indicating whether bombs within any explosion radius are immediately triggered.</summary>
    public bool ExplosionTriggeredBombs { get; set; } = true;

    /// <summary>Gets or sets a value indicating whether to set the quality of legendary fish at best.</summary>
    public bool LegendaryFishAlwaysBestQuality { get; set; } = true;
}
