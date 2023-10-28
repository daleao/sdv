namespace DaLion.Overhaul.Modules.Tweex;

#region using directives

using System.Collections.Generic;
using DaLion.Overhaul.Modules.Core.ConfigMenu;
using DaLion.Shared.Extensions.SMAPI;
using DaLion.Shared.Integrations.GMCM.Attributes;
using Newtonsoft.Json;

#endregion using directives

/// <summary>The user-configurable settings for TWX.</summary>
public sealed class TweexConfig
{
    private float _cropWitherChance = 0.1f;
    private float _beeHouseAgingFactor = 1f;
    private float _teaBushAgingFactor = 1f;
    private float _fruitTreeAgingFactor = 1f;
    private float _treeAgingFactor = 1f;
    private float _mushroomBoxAgingFactor = 1f;
    private bool _immersiveGlowstoneProgression = true;

    #region farming

    /// <summary>Gets the chance a crop may wither per day left un-watered.</summary>
    [JsonProperty]
    [GMCMSection("twx.farming")]
    [GMCMPriority(0)]
    [GMCMRange(0f, 1f)]
    [GMCMInterval(0.05f)]
    public float CropWitherChance
    {
        get => this._cropWitherChance;
        internal set
        {
            this._cropWitherChance = Math.Clamp(value, 0f, 1f);
        }
    }

    /// <summary>Gets the degree to which Bee House age improves honey quality.</summary>
    [JsonProperty]
    [GMCMSection("twx.farming")]
    [GMCMPriority(2)]
    [GMCMRange(0f, 2f)]
    public float BeeHouseAgingFactor
    {
        get => this._beeHouseAgingFactor;
        internal set
        {
            this._beeHouseAgingFactor = Math.Abs(value);
        }
    }

    /// <summary>Gets the degree to which Tea Bush age improves Tea Leaf quality.</summary>
    [JsonProperty]
    [GMCMSection("twx.farming")]
    [GMCMPriority(3)]
    [GMCMRange(0f, 2f)]
    public float TeaBushAgingFactor
    {
        get => this._teaBushAgingFactor;
        internal set
        {
            this._teaBushAgingFactor = Math.Abs(value);
        }
    }

    /// <summary>Gets the degree to which Fruit Tree age improves fruit quality.</summary>
    [JsonProperty]
    [GMCMSection("twx.farming")]
    [GMCMPriority(4)]
    [GMCMRange(0f, 2f)]
    public float FruitTreeAgingFactor
    {
        get => this._fruitTreeAgingFactor;
        internal set
        {
            this._fruitTreeAgingFactor = Math.Abs(value);
        }
    }

    /// <summary>Gets a value indicating whether fruit trees should be prevented from growing in winter, as for regular trees.</summary>
    [JsonProperty]
    [GMCMSection("twx.farming")]
    [GMCMPriority(1)]
    public bool PreventFruitTreeWinterGrowth { get; internal set; } = true;

    /// <summary>Gets a value indicating whether large eggs and milk should yield twice the output stack instead of higher quality.</summary>
    [JsonProperty]
    [GMCMSection("twx.farming")]
    [GMCMPriority(5)]
    public bool ImmersiveDairyYield { get; internal set; } = true;

    /// <summary>
    ///     Gets a list of Artisan machines which should be compatible with
    ///     LargeDairyYieldsQuantityOverQuality.
    /// </summary>
    [JsonProperty]
    [GMCMIgnore]
    public HashSet<string> DairyArtisanMachines { get; internal set; } = new()
    {
        "Butter Churn", // artisan valley
        "Ice Cream Machine", // artisan valley
        "Keg", // vanilla, but artisan valley adds "kefir" item
        "Yogurt Jar", // artisan valley
    };

    /// <summary>Gets a set of maps in which to attempt to spawn crows.</summary>
    [JsonProperty]
    [GMCMSection("twx.farming")]
    [GMCMPriority(6)]
    [GMCMOverride(typeof(GenericModConfigMenu), "TweexConfigSpawnCrowsOnTheseMapsOverride")]
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
    [GMCMSection("twx.foraging")]
    [GMCMPriority(50)]
    [GMCMRange(0f, 2f)]
    public float TreeAgingFactor
    {
        get => this._treeAgingFactor;
        internal set
        {
            this._treeAgingFactor = Math.Abs(value);
        }
    }

    /// <summary>Gets the degree to which Mushroom Box age improves mushroom quality.</summary>
    [JsonProperty]
    [GMCMSection("twx.foraging")]
    [GMCMPriority(51)]
    [GMCMRange(0f, 2f)]
    public float MushroomBoxAgingFactor
    {
        get => this._mushroomBoxAgingFactor;
        internal set
        {
            this._mushroomBoxAgingFactor = Math.Abs(value);
        }
    }

    /// <summary>Gets the amount of Foraging experience rewarded for harvesting Mushroom Boxes. Set to zero to disable.</summary>
    [JsonProperty]
    [GMCMSection("twx.foraging")]
    [GMCMPriority(52)]
    [GMCMRange(0, 10)]
    public uint MushroomBoxExpReward { get; internal set; } = 1;

    /// <summary>Gets the amount of Foraging experience rewarded for harvesting berry bushes. Set to zero to disable.</summary>
    [JsonProperty]
    [GMCMSection("twx.foraging")]
    [GMCMPriority(53)]
    [GMCMRange(0, 10)]
    public uint BerryBushExpReward { get; internal set; } = 5;

    /// <summary>Gets the amount of Foraging experience rewarded for harvesting Tappers. Set to zero to disable.</summary>
    [JsonProperty]
    [GMCMSection("twx.foraging")]
    [GMCMPriority(54)]
    [GMCMRange(0, 10)]
    public uint TapperExpReward { get; internal set; } = 2;

    #endregion foraging

    #region fishing

    /// <summary>Gets a value indicating whether to preserve the bait when a Crab Pot produces trash.</summary>
    [JsonProperty]
    [GMCMSection("twx.fishing")]
    [GMCMPriority(100)]
    public bool TrashDoesNotConsumeBait { get; internal set; } = true;

    #endregion fishing

    #region mining

    /// <summary>Gets a value indicating whether bombs within any explosion radius are immediately triggered.</summary>
    [JsonProperty]
    [GMCMSection("twx.mining")]
    [GMCMPriority(150)]
    public bool ChainExplosions { get; internal set; } = true;

    /// <summary>Gets a value indicating whether to impadd new mining recipes for crafting Glow and Magnet rings.</summary>
    [JsonProperty]
    [GMCMSection("twx.mining")]
    [GMCMPriority(151)]
    public bool ImmersiveGlowstoneProgression
    {
        get => this._immersiveGlowstoneProgression;
        internal set
        {
            this._immersiveGlowstoneProgression = value;
            ModHelper.GameContent.InvalidateCacheAndLocalized("Data/CraftingRecipes");
        }
    }

    #endregion mining

    #region other

    /// <summary>Gets a value indicating whether determines whether age-dependent qualities should be deterministic (true) or stochastic/random (false).</summary>
    [JsonProperty]
    [GMCMSection("twx.other")]
    [GMCMPriority(200)]
    public bool DeterministicAgeQuality { get; internal set; } = false;

    #endregion other
}
