namespace DaLion.Overhaul.Modules.Professions;

#region using directives

using System.Collections.Generic;
using DaLion.Overhaul.Modules.Core.ConfigMenu;
using DaLion.Overhaul.Modules.Professions.Configs;
using DaLion.Shared.Integrations.GMCM.Attributes;
using Newtonsoft.Json;

#endregion using directives

/// <summary>The user-configurable settings for PRFS.</summary>
public sealed class ProfessionConfig
{
    private float _scavengerHuntHandicap = 1f;
    private float _prospectorHuntHandicap = 1f;
    private float _anglerPriceBonusCeiling = 1f;
    private float _conservationistTaxDeductionCeiling = 1f;

    /// <inheritdoc cref="LimitBreakConfig"/>
    [JsonProperty]
    [GMCMInnerConfig("DaLion.Overhaul.Modules.Professions/Limit", "prfs.limit", true)]
    public LimitBreakConfig Limit { get; internal set; } = new();

    /// <inheritdoc cref="PrestigeConfig"/>
    [JsonProperty]
    [GMCMInnerConfig("DaLion.Overhaul.Modules.Professions/Prestige", "prfs.prestige", true)]
    public PrestigeConfig Prestige { get; internal set; } = new();

    /// <inheritdoc cref="ExperienceConfig"/>
    [JsonProperty]
    [GMCMInnerConfig("DaLion.Overhaul.Modules.Professions/Experience", "prfs.experience", true)]
    public ExperienceConfig Experience { get; internal set; } = new();

    /// <inheritdoc cref="ControlsUiConfig"/>
    [JsonProperty]
    [GMCMInnerConfig("DaLion.Overhaul.Modules.Professions/ControlsUi", "controls_ui", true)]
    public ControlsUiConfig ControlsUi { get; internal set; } = new();

    /// <summary>Gets a value indicating whether determines whether Harvester and Agriculturist perks should apply to crops harvested by Junimos.</summary>
    [JsonProperty]
    [GMCMSection("prfs.general")]
    [GMCMPriority(0)]
    public bool ShouldJunimosInheritProfessions { get; internal set; } = false;

    /// <summary>
    ///     Gets a value indicating whether if enabled, machine and building ownership will be ignored when determining whether to apply profession
    ///     bonuses.
    /// </summary>
    [JsonProperty]
    [GMCMSection("prfs.general")]
    [GMCMPriority(1)]
    public bool LaxOwnershipRequirements { get; internal set; } = false;

    /// <summary>Gets a value indicating whether the quality of produced artisan goods should be always the same as the quality of the input material. If set to false, then the quality will be less than or equal to that of the input.</summary>
    [JsonProperty]
    [GMCMSection("prfs.general")]
    [GMCMPriority(2)]
    public bool ArtisanGoodsAlwaysInputQuality { get; internal set; } = false;

    /// <summary>Gets a set of machines used to create artisan goods. Add to this list the artisan machines from third-party mods you are using to make them compatible with the Artisan profession.</summary>
    [JsonProperty]
    [GMCMSection("prfs.general")]
    [GMCMPriority(3)]
    [GMCMOverride(typeof(GenericModConfigMenu), "ProfessionConfigArtisanMachinesOverride")]
    public HashSet<string> ArtisanMachines { get; internal set; } = new()
    {
        "Cheese Press", // vanilla
        "Keg", // vanilla
        "Loom", // vanilla
        "Mayonnaise Machine", // vanilla
        "Oil Maker", // vanilla
        "Preserves Jar", // vanilla
        "Alembic", // artisan valley
        "Butter Churn", // artisan valley
        "Canning Machine", // fresh meat
        "DNA Synthesizer", // fresh meat
        "Dehydrator", // artisan valley
        "Drying Rack", // artisan valley
        "Espresso Machine", // artisan valley
        "Extruder", // artisan valley
        "Foreign Cask", // artisan valley
        "Glass Jar", // artisan valley
        "Grinder", // artisan valley
        "Ice Cream Machine", // artisan valley
        "Infuser", // artisan valley
        "Juicer", // artisan valley
        "Marble Soda Machine", // fizzy drinks
        "Meat Press", // fresh meat
        "Pepper Blender", // artisan valley
        "Smoker", // artisan valley
        "Soap Press", // artisan valley
        "Sorbet Machine", // artisan valley
        "Still", // artisan valley
        "Vinegar Cask", // artisan valley
        "Wax Barrel", // artisan valley
        "Yogurt Jar", // artisan valley
        "Artisanal Soda Maker", // artisanal soda makers
        "Carbonator", // artisanal soda makers
        "Cola Maker", // artisanal soda makers
        "Cream Soda Maker", // artisanal soda makers
        "Syrup Maker", // artisanal soda makers
        "Shaved Ice Machine", // shaved ice & frozen treats
    };

    /// <summary>Gets a list of artisan goods derived from animal produce. Add to this list the animal-derived goods from third-party mods you are using to make them compatible with the Producer profession.</summary>
    [JsonProperty]
    [GMCMSection("prfs.general")]
    [GMCMPriority(4)]
    [GMCMOverride(typeof(GenericModConfigMenu), "ProfessionConfigAnimalDerivedGoodsOverride")]
    public HashSet<string> AnimalDerivedGoods { get; internal set; } = new()
    {
        "Mayonnaise", // vanilla
        "Duck Mayonnaise", // vanilla
        "Void Mayonnaise", // vanilla
        "Dinosaur Mayonnaise", // vanilla
        "Cheese", // vanilla
        "Goat Cheese", // vanilla
        "Cloth", // vanilla
        "Butter", // ppja
        "Goat Butter", // ppja
        "Yogurt", // ppja
        "Goat Yogurt", // ppja
        "Fruit Yogurt", // ppja
        "Kefir", // ppja
        "Kumis", // ppja
        "Lassi", // ppja
        "Avocado Mayonnaise", // ppja
        "Black Pepper Mayonnaise", // ppja
        "Garlic Mayonnaise", // ppja
        "Lucky Purple Mayonnaise", // ppja
        "Olive Oil Mayonnaise", // ppja
        "Thunder Mayonnaise", // ppja
        "Wasabi Mayonnaise", // ppja
        "Delight Mayonnaise", // ostrich mayonnaise
        "Shiny Mayonnaise", // gold mayonnaise
        "Slime Butter", // ppja
        "Slime Cheese", // garden village
        "Slime Mayonnaise", // garden village
    };

    /// <summary>Gets a value indicating whether Bee House products should be affected by Producer bonuses.</summary>
    [JsonProperty]
    [GMCMSection("prfs.general")]
    [GMCMPriority(5)]
    public bool BeesAreAnimals { get; internal set; } = true;

    /// <summary>Gets the number of items that must be foraged before foraged items become iridium-quality.</summary>
    [JsonProperty]
    [GMCMSection("prfs.general")]
    [GMCMPriority(6)]
    [GMCMRange(0, 1000)]
    [GMCMInterval(10)]
    public uint ForagesNeededForBestQuality { get; internal set; } = 100;

    /// <summary>Gets the number of minerals that must be mined before mined minerals become iridium-quality.</summary>
    [JsonProperty]
    [GMCMSection("prfs.general")]
    [GMCMPriority(7)]
    [GMCMRange(0, 1000)]
    [GMCMInterval(10)]
    public uint MineralsNeededForBestQuality { get; internal set; } = 100;

    /// <summary>Gets the chance that a scavenger or prospector hunt will trigger in the right conditions.</summary>
    [JsonProperty]
    [GMCMSection("prfs.general")]
    [GMCMPriority(8)]
    [GMCMRange(0f, 1f)]
    [GMCMInterval(0.05f)]
    public double ChanceToStartTreasureHunt { get; internal set; } = 0.1;

    /// <summary>Gets a value indicating whether determines whether a Scavenger Hunt can trigger while entering a farm map.</summary>
    [JsonProperty]
    [GMCMSection("prfs.general")]
    [GMCMPriority(9)]
    public bool AllowScavengerHuntsOnFarm { get; internal set; } = false;

    /// <summary>Gets the minimum distance to the scavenger hunt target before the indicator appears.</summary>
    [JsonProperty]
    [GMCMSection("prfs.general")]
    [GMCMPriority(10)]
    [GMCMRange(1, 12)]
    public uint ScavengerDetectionDistance { get; internal set; } = 3;

    /// <summary>Gets a multiplier which is used to extend the duration of Scavenger hunts, in case you feel they end too quickly.</summary>
    [JsonProperty]
    [GMCMSection("prfs.general")]
    [GMCMPriority(11)]
    [GMCMRange(0.5f, 3f)]
    [GMCMInterval(0.2f)]
    public float ScavengerHuntHandicap
    {
        get => this._scavengerHuntHandicap;
        internal set
        {
            this._scavengerHuntHandicap = Math.Max(value, 0.5f);
        }
    }

    /// <summary>Gets the minimum distance to the prospector hunt target before the indicator is heard.</summary>
    [JsonProperty]
    [GMCMSection("prfs.general")]
    [GMCMPriority(12)]
    [GMCMRange(10, 30)]
    public uint ProspectorDetectionDistance { get; internal set; } = 20;

    /// <summary>Gets a multiplier which is used to extend the duration of Prospector hunts, in case you feel they end too quickly.</summary>
    [JsonProperty]
    [GMCMSection("prfs.general")]
    [GMCMPriority(13)]
    [GMCMRange(0.5f, 3f)]
    [GMCMInterval(0.2f)]
    public float ProspectorHuntHandicap
    {
        get => this._prospectorHuntHandicap;
        internal set
        {
            this._prospectorHuntHandicap = Math.Max(value, 0.5f);
        }
    }

    /// <summary>Gets the maximum speed bonus a Spelunker can reach.</summary>
    [JsonProperty]
    [GMCMSection("prfs.general")]
    [GMCMPriority(14)]
    [GMCMRange(0, 10)]
    public uint SpelunkerSpeedCeiling { get; internal set; } = 10;

    /// <summary>Gets a value indicating whether toggles the Get Excited buff when a Demolitionist is hit by an explosion.</summary>
    [JsonProperty]
    [GMCMSection("prfs.general")]
    [GMCMPriority(15)]
    public bool DemolitionistGetExcited { get; internal set; } = true;

    /// <summary>Gets a value indicating whether to increase the quality of all active Crystalarium minerals when the Gemologist owner gains a quality level-up.</summary>
    [JsonProperty]
    [GMCMSection("prfs.general")]
    [GMCMPriority(16)]
    public bool CrystalariumUpgradesWithGemologist { get; internal set; } = true;

    /// <summary>
    ///     Gets the maximum multiplier that will be added to fish sold by Angler. if multiple new fish mods are installed,
    ///     you may want to adjust this to a sensible value.
    /// </summary>
    [JsonProperty]
    [GMCMSection("prfs.general")]
    [GMCMPriority(17)]
    [GMCMRange(0.25f, 4f)]
    [GMCMInterval(0.25f)]
    public float AnglerPriceBonusCeiling
    {
        get => this._anglerPriceBonusCeiling;
        internal set
        {
            this._anglerPriceBonusCeiling = Math.Abs(value);
        }
    }

    /// <summary>
    ///     Gets the maximum number of Fish Ponds that will be counted for catching bar loss compensation.
    /// </summary>
    [JsonProperty]
    [GMCMSection("prfs.general")]
    [GMCMPriority(18)]
    [GMCMRange(0, 24f)]
    public uint AquaristFishPondCeiling { get; internal set; } = 12;

    /// <summary>Gets the amount of junk items that must be collected from crab pots for every 1% of tax deduction the following season.</summary>
    [JsonProperty]
    [GMCMSection("prfs.general")]
    [GMCMPriority(19)]
    [GMCMRange(10, 1000)]
    [GMCMInterval(10)]
    public uint TrashNeededPerTaxDeduction { get; internal set; } = 100;

    /// <summary>Gets the amount of junk items that must be collected from crab pots for every 1 point of friendship towards villagers.</summary>
    [JsonProperty]
    [GMCMSection("prfs.general")]
    [GMCMPriority(20)]
    [GMCMRange(10, 1000)]
    [GMCMInterval(10)]
    public uint TrashNeededPerFriendshipPoint { get; internal set; } = 100;

    /// <summary>Gets the maximum income deduction allowed by the Ferngill Revenue Service.</summary>
    [JsonProperty]
    [GMCMSection("prfs.general")]
    [GMCMPriority(21)]
    [GMCMRange(0.1f, 1f)]
    [GMCMInterval(0.05f)]
    public float ConservationistTaxDeductionCeiling
    {
        get => this._conservationistTaxDeductionCeiling;
        internal set
        {
            this._conservationistTaxDeductionCeiling = Math.Abs(value);
        }
    }
}
