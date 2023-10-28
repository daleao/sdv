namespace DaLion.Overhaul.Modules.Professions;

#region using directives

using System.Collections.Generic;
using DaLion.Overhaul.Modules.Core.ConfigMenu;
using DaLion.Overhaul.Modules.Core.UI;
using DaLion.Overhaul.Modules.Professions.Events.Display.RenderingHud;
using DaLion.Overhaul.Modules.Professions.Events.Player.Warped;
using DaLion.Overhaul.Modules.Professions.Extensions;
using DaLion.Overhaul.Modules.Professions.Ultimates;
using DaLion.Overhaul.Modules.Professions.VirtualProperties;
using DaLion.Shared.Integrations.GMCM.Attributes;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using StardewModdingAPI.Utilities;

#endregion using directives

/// <summary>The user-configurable settings for PRFS.</summary>
public sealed class ProfessionConfig
{
    private readonly Dictionary<string, float> _skillExpMultipliers = new()
    {
        { "Farming", 1f },
        { "Fishing", 1f },
        { "Foraging", 1f },
        { "Mining", 1f },
        { "Combat", 1f },
        { "Luck", 1f },
        { "blueberry.LoveOfCooking.CookingSkill", 1f },
        { "spacechase0.Cooking", 1f },
        { "spacechase0.Magic", 1f },
        { "drbirbdev.Binning", 1f },
        { "drbirbdev.Socializing", 1f },
        { "moonslime.Excavation", 1f },
    };

    private float _scavengerHuntHandicap = 1f;
    private float _prospectorHuntHandicap = 1f;
    private float _anglerPriceBonusCeiling = 1f;
    private float _conservationistTaxDeductionCeiling = 1f;
    private bool _enableLimitBreaks = true;
    private double _limitGainFactor = 1f;
    private double _limitDrainFactor = 1f;
    private float _skillResetCostMultiplier = 1f;
    private float _expBonusPerSkillReset = 0.1f;
    private ProgressionStyle _progressionStyle = ProgressionStyle.StackedStars;
    private float _trackingPointerScale = 1.2f;
    private float _trackingPointerBobRate = 1f;

    #region dropdown enums

    /// <summary>The style used to indicate Skill Reset progression.</summary>
    public enum ProgressionStyle
    {
        /// <summary>Use stacked quality star icons, one per reset level.</summary>
        StackedStars,

        /// <summary>Use Generation 3 Pokemon contest ribbons.</summary>
        Gen3Ribbons,

        /// <summary>Use Generation 4 Pokemon contest ribbons.</summary>
        Gen4Ribbons,
    }

    #endregion dropdown enums

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

    /// <summary>Gets custom mod Artisan machines. Add to this list to make them compatible with the profession.</summary>
    [JsonProperty]
    [GMCMIgnore]
    public HashSet<string> CustomArtisanMachines { get; internal set; } = new()
    {
        "Alembic", // artisan valley
        "Artisanal Soda Maker", // artisanal soda makers
        "Butter Churn", // artisan valley
        "Canning Machine", // fresh meat
        "Carbonator", // artisanal soda makers
        "Cola Maker", // artisanal soda makers
        "Cream Soda Maker", // artisanal soda makers
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
        "Shaved Ice Machine", // shaved ice & frozen treats
        "Smoker", // artisan valley
        "Soap Press", // artisan valley
        "Sorbet Machine", // artisan valley
        "Still", // artisan valley
        "Syrup Maker", // artisanal soda makers
        "Vinegar Cask", // artisan valley
        "Wax Barrel", // artisan valley
        "Yogurt Jar", // artisan valley
    };

    /// <summary>Gets a value indicating whether Bee House products should be affected by Producer bonuses.</summary>
    [JsonProperty]
    [GMCMSection("prfs.general")]
    [GMCMPriority(3)]
    public bool BeesAreAnimals { get; internal set; } = true;

    /// <summary>Gets the number of items that must be foraged before foraged items become iridium-quality.</summary>
    [JsonProperty]
    [GMCMSection("prfs.general")]
    [GMCMPriority(4)]
    [GMCMRange(0, 1000)]
    [GMCMInterval(10)]
    public uint ForagesNeededForBestQuality { get; internal set; } = 100;

    /// <summary>Gets the number of minerals that must be mined before mined minerals become iridium-quality.</summary>
    [JsonProperty]
    [GMCMSection("prfs.general")]
    [GMCMPriority(5)]
    [GMCMRange(0, 1000)]
    [GMCMInterval(10)]
    public uint MineralsNeededForBestQuality { get; internal set; } = 100;

    /// <summary>Gets the chance that a scavenger or prospector hunt will trigger in the right conditions.</summary>
    [JsonProperty]
    [GMCMSection("prfs.general")]
    [GMCMPriority(6)]
    [GMCMRange(0f, 1f)]
    [GMCMInterval(0.05f)]
    public double ChanceToStartTreasureHunt { get; internal set; } = 0.1;

    /// <summary>Gets a value indicating whether determines whether a Scavenger Hunt can trigger while entering a farm map.</summary>
    [JsonProperty]
    [GMCMSection("prfs.general")]
    [GMCMPriority(7)]
    public bool AllowScavengerHuntsOnFarm { get; internal set; } = false;

    /// <summary>Gets a multiplier which is used to extend the duration of Scavenger hunts, in case you feel they end too quickly.</summary>
    [JsonProperty]
    [GMCMSection("prfs.general")]
    [GMCMPriority(8)]
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

    /// <summary>Gets a multiplier which is used to extend the duration of Prospector hunts, in case you feel they end too quickly.</summary>
    [JsonProperty]
    [GMCMSection("prfs.general")]
    [GMCMPriority(9)]
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

    /// <summary>Gets the minimum distance to the treasure hunt target before the indicator appears.</summary>
    [JsonProperty]
    [GMCMSection("prfs.general")]
    [GMCMPriority(10)]
    [GMCMRange(1, 10)]
    public uint TreasureDetectionDistance { get; internal set; } = 3;

    /// <summary>Gets the maximum speed bonus a Spelunker can reach.</summary>
    [JsonProperty]
    [GMCMSection("prfs.general")]
    [GMCMPriority(11)]
    [GMCMRange(0, 10)]
    public uint SpelunkerSpeedCeiling { get; internal set; } = 10;

    /// <summary>Gets a value indicating whether toggles the Get Excited buff when a Demolitionist is hit by an explosion.</summary>
    [JsonProperty]
    [GMCMSection("prfs.general")]
    [GMCMPriority(12)]
    public bool DemolitionistGetExcited { get; internal set; } = true;

    /// <summary>Gets a value indicating whether to increase the quality of all active Crystalarium minerals when the Gemologist owner gains a quality level-up.</summary>
    [JsonProperty]
    [GMCMSection("prfs.general")]
    [GMCMPriority(13)]
    public bool CrystalariumUpgradesWithGemologist { get; internal set; } = true;

    /// <summary>
    ///     Gets the maximum multiplier that will be added to fish sold by Angler. if multiple new fish mods are installed,
    ///     you may want to adjust this to a sensible value.
    /// </summary>
    [JsonProperty]
    [GMCMSection("prfs.general")]
    [GMCMPriority(14)]
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
    [GMCMPriority(15)]
    [GMCMRange(0, 24f)]
    public uint AquaristFishPondCeiling { get; internal set; } = 12;

    /// <summary>Gets the amount of junk items that must be collected from crab pots for every 1% of tax deduction the following season.</summary>
    [JsonProperty]
    [GMCMSection("prfs.general")]
    [GMCMPriority(16)]
    [GMCMRange(10, 1000)]
    [GMCMInterval(10)]
    public uint TrashNeededPerTaxDeduction { get; internal set; } = 100;

    /// <summary>Gets the amount of junk items that must be collected from crab pots for every 1 point of friendship towards villagers.</summary>
    [JsonProperty]
    [GMCMSection("prfs.general")]
    [GMCMPriority(17)]
    [GMCMRange(10, 1000)]
    [GMCMInterval(10)]
    public uint TrashNeededPerFriendshipPoint { get; internal set; } = 100;

    /// <summary>Gets the maximum income deduction allowed by the Ferngill Revenue Service.</summary>
    [JsonProperty]
    [GMCMSection("prfs.general")]
    [GMCMPriority(18)]
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

    #region limit break

    /// <summary>Gets a value indicating whether to allow Limit Breaks to be used in-game.</summary>
    [JsonProperty]
    [GMCMSection("prfs.limit_break")]
    [GMCMPriority(100)]
    public bool EnableLimitBreaks
    {
        get => this._enableLimitBreaks;
        internal set
        {
            this._enableLimitBreaks = value;
            if (!Context.IsWorldReady || Game1.player.Get_Ultimate() is not { } ultimate)
            {
                return;
            }

            switch (value)
            {
                case false:
                    ultimate.ChargeValue = 0d;
                    EventManager.DisableWithAttribute<UltimateEventAttribute>();
                    break;
                case true:
                {
                    Game1.player.RevalidateUltimate();
                    EventManager.Enable<UltimateWarpedEvent>();
                    if (Game1.currentLocation.IsDungeon())
                    {
                        EventManager.Enable<UltimateMeterRenderingHudEvent>();
                    }

                    break;
                }
            }
        }
    }

    /// <summary>Gets the mod key used to activate the Limit Break.</summary>
    [JsonProperty]
    [GMCMSection("prfs.limit_break")]
    [GMCMPriority(101)]
    public KeybindList LimitBreakKey { get; internal set; } = KeybindList.Parse("LeftShift, LeftShoulder");

    /// <summary>Gets a value indicating whether the Limit Break is activated by holding the <see cref="LimitBreakKey"/>, as opposed to simply pressing.</summary>
    [JsonProperty]
    [GMCMSection("prfs.limit_break")]
    [GMCMPriority(102)]
    public bool HoldKeyToLimitBreak { get; internal set; } = true;

    /// <summary>Gets how long the <see cref="LimitBreakKey"/> should be held to activate the Limit Break, in milliseconds.</summary>
    [JsonProperty]
    [GMCMSection("prfs.limit_break")]
    [GMCMPriority(103)]
    [GMCMRange(250, 4000)]
    [GMCMInterval(50)]
    public uint LimitBreakHoldDelayMilliseconds { get; internal set; }

    /// <summary>
    ///     Gets the rate at which one builds the Limit gauge. Increase this if you feel the gauge raises too
    ///     slowly.
    /// </summary>
    [JsonProperty]
    [GMCMSection("prfs.limit_break")]
    [GMCMPriority(104)]
    [GMCMRange(0.25f, 4f)]
    public double LimitGainFactor
    {
        get => this._limitGainFactor;
        internal set
        {
            this._limitGainFactor = Math.Abs(value);
        }
    }

    /// <summary>
    ///     Gets the rate at which the Limit gauge depletes during Ultimate. Decrease this to make the Limit Break last
    ///     longer.
    /// </summary>
    [JsonProperty]
    [GMCMSection("prfs.limit_break")]
    [GMCMPriority(105)]
    [GMCMRange(0.25f, 4f)]
    public double LimitDrainFactor
    {
        get => this._limitDrainFactor;
        internal set
        {
            this._limitDrainFactor = Math.Abs(value);
        }
    }

    /// <summary>Gets monetary cost of changing the chosen Limit Break. Set to 0 to change for free.</summary>
    [JsonProperty]
    [GMCMSection("prfs.limit_break")]
    [GMCMPriority(106)]
    [GMCMRange(0, 100000)]
    [GMCMInterval(1000)]
    public uint LimitRespecCost { get; internal set; } = 0;

    /// <summary>Gets the offset that should be applied to the Limit Gauge's position.</summary>
    [JsonProperty]
    [GMCMSection("prfs.limit_break")]
    [GMCMPriority(107)]
    [GMCMDefaultVector2(0f, 0f)]
    public Vector2 LimitGaugeOffset { get; internal set; } = Vector2.Zero;

    #endregion limit break

    #region prestige

    /// <summary>Gets a value indicating whether to apply Prestige changes.</summary>
    [JsonProperty]
    [GMCMSection("prfs.prestige")]
    [GMCMPriority(200)]
    public bool EnablePrestige { get; internal set; } = true;

    /// <summary>Gets the base skill reset cost multiplier. Set to 0 to reset for free.</summary>
    [JsonProperty]
    [GMCMSection("prfs.prestige")]
    [GMCMPriority(201)]
    [GMCMRange(0f, 3f)]
    public float SkillResetCostMultiplier
    {
        get => this._skillResetCostMultiplier;
        internal set
        {
            this._skillResetCostMultiplier = Math.Abs(value);
        }
    }

    /// <summary>Gets a value indicating whether resetting a skill also clears all corresponding recipes.</summary>
    [JsonProperty]
    [GMCMSection("prfs.prestige")]
    [GMCMPriority(202)]
    public bool ForgetRecipesOnSkillReset { get; internal set; } = true;

    /// <summary>Gets a value indicating whether the player can use the Statue of Prestige more than once per day.</summary>
    [JsonProperty]
    [GMCMSection("prfs.prestige")]
    [GMCMPriority(203)]
    public bool AllowMultipleResets { get; internal set; } = false;

    /// <summary>Gets a percentage bonus applied to a skill's experience gain after a respective skill prestige. Negative values mean it becomes harder to regain those levels.</summary>
    [JsonProperty]
    [GMCMSection("prfs.prestige")]
    [GMCMPriority(204)]
    [GMCMRange(-0.5f, 1f)]
    public float ExpBonusPerSkillReset
    {
        get => this._expBonusPerSkillReset;
        internal set
        {
            this._expBonusPerSkillReset = Math.Max(value, -0.5f);
        }
    }

    /// <summary>Gets monetary cost of respecing prestige profession choices for a skill. Set to 0 to respec for free.</summary>
    [JsonProperty]
    [GMCMSection("prfs.prestige")]
    [GMCMPriority(205)]
    [GMCMRange(0, 100000)]
    [GMCMInterval(1000)]
    public uint PrestigeRespecCost { get; internal set; } = 20000;

    /// <summary>
    ///     Gets the style of the sprite that appears next to skill bars. Accepted values: "StackedStars", "Gen3Ribbons",
    ///     "Gen4Ribbons".
    /// </summary>
    [JsonProperty]
    [GMCMSection("prfs.prestige")]
    [GMCMPriority(206)]
    public ProgressionStyle PrestigeProgressionStyle
    {
        get => this._progressionStyle;
        internal set
        {
            this._progressionStyle = value;
            ModHelper.GameContent.InvalidateCache($"{Manifest.UniqueID}/PrestigeProgression");
        }
    }

    /// <summary>Gets a value indicating whether to allow extended progression up to level 20.</summary>
    [JsonProperty]
    [GMCMSection("prfs.prestige")]
    [GMCMPriority(207)]
    public bool EnableExtendedProgression { get; internal set; } = true;

    /// <summary>Gets how much skill experience is required for each level up beyond 10.</summary>
    [JsonProperty]
    [GMCMSection("prfs.prestige")]
    [GMCMPriority(208)]
    [GMCMRange(1000, 10000)]
    [GMCMInterval(500)]
    public uint RequiredExpPerExtendedLevel { get; internal set; } = 5000;

    #endregion prestige

    #region experience

    /// <summary>Gets a multiplier that will be applied to all skill experience gained from the start of the game.</summary>
    /// <remarks>The order is Farming, Fishing, Foraging, Mining, Combat and Luck (if installed).</remarks>
    [JsonProperty]
    [GMCMSection("prfs.experience")]
    [GMCMPriority(300)]
    [GMCMRange(0.2f, 2f)]
    [GMCMOverride(typeof(GenericModConfigMenu), "ProfessionConfigSkillExpMulitpliersOverride")]
    public Dictionary<string, float> SkillExpMultipliers
    {
        get => this._skillExpMultipliers;
        internal set
        {
            foreach (var pair in value)
            {
                this._skillExpMultipliers[pair.Key] = Math.Abs(pair.Value);
            }
        }
    }

    #endregion experience

    #region controls & ui

    /// <summary>Gets mod key used by Prospector and Scavenger professions.</summary>
    [JsonProperty]
    [GMCMSection("controls_ui")]
    [GMCMPriority(401)]
    public KeybindList ModKey { get; internal set; } = KeybindList.Parse("LeftShift, LeftShoulder");

    /// <summary>Gets the size of the pointer used to track objects by Prospector and Scavenger professions.</summary>
    [JsonProperty]
    [GMCMSection("controls_ui")]
    [GMCMPriority(402)]
    [GMCMRange(0.2f, 5f)]
    [GMCMInterval(0.2f)]
    public float TrackingPointerScale
    {
        get => this._trackingPointerScale;
        internal set
        {
            this._trackingPointerScale = value;
            if (HudPointer.Instance.IsValueCreated)
            {
                HudPointer.Instance.Value.Scale = value;
            }
        }
    }

    /// <summary>Gets the speed at which the tracking pointer bounces up and down (higher is faster).</summary>
    [JsonProperty]
    [GMCMSection("controls_ui")]
    [GMCMPriority(403)]
    [GMCMRange(0.5f, 2f)]
    [GMCMInterval(0.05f)]
    public float TrackingPointerBobRate
    {
        get => this._trackingPointerBobRate;
        internal set
        {
            this._trackingPointerBobRate = value;
            if (HudPointer.Instance.IsValueCreated)
            {
                HudPointer.Instance.Value.BobRate = value;
            }
        }
    }

    /// <summary>Gets a value indicating whether if enabled, Prospector and Scavenger will only track off-screen object while <see cref="ModKey"/> is held.</summary>
    [JsonProperty]
    [GMCMSection("controls_ui")]
    [GMCMPriority(404)]
    public bool DisableAlwaysTrack { get; internal set; } = false;

    /// <summary>
    ///     Gets a value indicating whether to display the MAX icon below fish in the Collections Menu which have been caught at the
    ///     maximum size.
    /// </summary>
    [JsonProperty]
    [GMCMSection("controls_ui")]
    [GMCMPriority(405)]
    public bool ShowFishCollectionMaxIcon { get; internal set; } = true;

    #endregion controls & ui
}
