namespace DaLion.Professions;

#region using directives

using System.Collections.Generic;
using DaLion.Professions.Framework.Configs;
using DaLion.Professions.Framework.UI;
using DaLion.Shared.Integrations.GMCM.Attributes;
using Newtonsoft.Json;
using StardewModdingAPI.Utilities;

#endregion using directives

/// <summary>Config schema for the Professions mod.</summary>
public sealed class ProfessionsConfig
{
    private HashSet<string> _artisanMachines = ["(BC)ExampleMod.ExampleMachine"];
    private HashSet<string> _animalDerivedGoods = ["(O)ExampleMod.ExampleProduce"];
    private float _scavengerHuntHandicap = 1f;
    private float _prospectorHuntHandicap = 1f;
    private float _anglerPriceBonusCeiling = 1f;
    private float _conservationistTaxDeductionCeiling = 1f;
    private float _trackingPointerScale = 1.2f;
    private float _trackingPointerBobRate = 1f;
    private bool _immersiveHeavyTapperYield = true;

    /// <inheritdoc cref="SkillsConfig"/>
    [JsonProperty]
    [GMCMInnerConfig("DaLion.Professions/Skills", "prfs.skills", true)]
    public SkillsConfig Skills { get; internal set; } = new();

    /// <inheritdoc cref="MasteriesConfig"/>
    [JsonProperty]
    [GMCMInnerConfig("DaLion.Professions/Masteries", "prfs.masteries", true)]
    public MasteriesConfig Masteries { get; internal set; } = new();

    /// <summary>Gets mod key used by Prospector and Scavenger professions (also Demolitionist).</summary>
    [JsonProperty]
    [GMCMSection("prfs.general")]
    [GMCMPriority(0)]
    public KeybindList ModKey { get; internal set; } = KeybindList.Parse("LeftShift, LeftShoulder");

    /// <summary>
    ///     Gets a value indicating whether if enabled, machine and building ownership will be ignored when determining whether to apply profession
    ///     bonuses.
    /// </summary>
    [JsonProperty]
    [GMCMSection("prfs.general")]
    [GMCMPriority(1)]
    public bool LaxOwnershipRequirements { get; internal set; } = false;

    /// <summary>Gets a value indicating whether determines whether Harvester and Agriculturist perks should apply to crops harvested by Junimos.</summary>
    [JsonProperty]
    [GMCMSection("prfs.general")]
    [GMCMPriority(2)]
    public bool ShouldJunimosInheritProfessions { get; internal set; } = false;

    /// <summary>Gets a list of (qualified) IDs of machines used to create artisan goods. Add to this list the artisan machines from third-party mods you are using to make them compatible with the Artisan profession.</summary>
    [JsonProperty]
    [GMCMSection("prfs.artisan_producer")]
    [GMCMPriority(101)]
    [GMCMOverride(typeof(ProfessionsConfigMenu), "ArtisanMachinesOverride")]
    public HashSet<string> ArtisanMachines
    {
        get => this._artisanMachines;
        internal set
        {
            foreach (var machine in this._artisanMachines)
            {
                Lookups.ArtisanMachines.Remove(machine);
            }

            this._artisanMachines = value;
            foreach (var machine in this._artisanMachines)
            {
                Lookups.ArtisanMachines.Add(machine);
            }
        }
    }

    /// <summary>Gets a value indicating whether to set golden and ostrich egg machine outputs to corresponding new mayo items.</summary>
    [JsonProperty]
    [GMCMSection("prfs.artisan_producer")]
    [GMCMPriority(103)]
    public bool EnableGoldenOstrichMayo { get; internal set; } = true;

    /// <summary>Gets a value indicating whether large eggs and milk should yield twice the output stack instead of higher quality.</summary>
    [JsonProperty]
    [GMCMSection("prfs.artisan_producer")]
    [GMCMPriority(104)]
    public bool ImmersiveDairyYield { get; internal set; } = true;

    /// <summary>Gets a list of (qualified) IDs of artisan goods derived from animal produce. Add to this list the animal-derived goods from third-party mods you are using to make them compatible with the Producer profession.</summary>
    [JsonProperty]
    [GMCMSection("prfs.artisan_producer")]
    [GMCMPriority(102)]
    [GMCMOverride(typeof(ProfessionsConfigMenu), "AnimalDerivedGoodsOverride")]
    public HashSet<string> AnimalDerivedGoods
    {
        get => this._animalDerivedGoods;
        internal set
        {
            foreach (var good in this._animalDerivedGoods)
            {
                Lookups.AnimalDerivedGoods.Remove(good);
            }

            this._animalDerivedGoods = value;
            foreach (var good in this._animalDerivedGoods)
            {
                Lookups.AnimalDerivedGoods.Add(good);
            }
        }
    }

    /// <summary>Gets a value indicating whether Bee House products should be affected by Producer bonuses.</summary>
    [JsonProperty]
    [GMCMSection("prfs.artisan_producer")]
    [GMCMPriority(105)]
    public bool BeesAreAnimals { get; internal set; } = true;

    /// <summary>Gets the number of items that must be foraged before foraged items become iridium-quality.</summary>
    [JsonProperty]
    [GMCMSection("prfs.ecologist_gemologist")]
    [GMCMPriority(200)]
    [GMCMRange(0, 100)]
    [GMCMStep(10)]
    public uint ForagesNeededForBestQuality { get; internal set; } = 30;

    /// <summary>Gets the number of minerals that must be mined before mined minerals become iridium-quality.</summary>
    [JsonProperty]
    [GMCMSection("prfs.ecologist_gemologist")]
    [GMCMPriority(201)]
    [GMCMRange(0, 100)]
    [GMCMStep(10)]
    public uint MineralsNeededForBestQuality { get; internal set; } = 30;

    /// <summary>Gets a multiplier applied to the base chance that a Scavenger or Prospector hunt will trigger in the right conditions.</summary>
    [JsonProperty]
    [GMCMSection("prfs.scavenger_prospector")]
    [GMCMPriority(300)]
    [GMCMRange(0.5f, 3f)]
    [GMCMStep(0.25f)]
    public double TreasureHuntStartChanceMultiplier { get; internal set; } = 1f;

    /// <summary>Gets a value indicating whether determines whether a Scavenger Hunt can trigger while entering a farm map.</summary>
    [JsonProperty]
    [GMCMSection("prfs.scavenger_prospector")]
    [GMCMPriority(301)]
    public bool AllowScavengerHuntsOnFarm { get; internal set; } = false;

    /// <summary>Gets the minimum distance to the scavenger hunt target before the indicator appears.</summary>
    [JsonProperty]
    [GMCMSection("prfs.scavenger_prospector")]
    [GMCMPriority(302)]
    [GMCMRange(1, 12)]
    public uint ScavengerDetectionDistance { get; internal set; } = 3;

    /// <summary>Gets a multiplier which is used to extend the duration of Scavenger hunts, in case you feel they end too quickly.</summary>
    [JsonProperty]
    [GMCMSection("prfs.scavenger_prospector")]
    [GMCMPriority(303)]
    [GMCMRange(0.5f, 3f)]
    [GMCMStep(0.2f)]
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
    [GMCMSection("prfs.scavenger_prospector")]
    [GMCMPriority(304)]
    [GMCMRange(10, 30)]
    public uint ProspectorDetectionDistance { get; internal set; } = 20;

    /// <summary>Gets a multiplier which is used to extend the duration of Prospector hunts, in case you feel they end too quickly.</summary>
    [JsonProperty]
    [GMCMSection("prfs.scavenger_prospector")]
    [GMCMPriority(305)]
    [GMCMRange(0.5f, 3f)]
    [GMCMStep(0.2f)]
    public float ProspectorHuntHandicap
    {
        get => this._prospectorHuntHandicap;
        internal set
        {
            this._prospectorHuntHandicap = Math.Max(value, 0.5f);
        }
    }

    /// <summary>Gets the size of the pointer used to track objects by Prospector and Scavenger professions.</summary>
    [JsonProperty]
    [GMCMSection("prfs.scavenger_prospector")]
    [GMCMPriority(306)]
    [GMCMRange(0.2f, 5f)]
    [GMCMStep(0.2f)]
    public float TrackingPointerScale
    {
        get => this._trackingPointerScale;
        internal set
        {
            this._trackingPointerScale = value;
            if (HudPointer.IsCreated)
            {
                HudPointer.Instance.Scale = value;
            }
        }
    }

    /// <summary>Gets the speed at which the tracking pointer bounces up and down (higher is faster).</summary>
    [JsonProperty]
    [GMCMSection("prfs.scavenger_prospector")]
    [GMCMPriority(307)]
    [GMCMRange(0.5f, 2f)]
    [GMCMStep(0.05f)]
    public float TrackingPointerBobRate
    {
        get => this._trackingPointerBobRate;
        internal set
        {
            this._trackingPointerBobRate = value;
            if (HudPointer.IsCreated)
            {
                HudPointer.Instance.BobRate = value;
            }
        }
    }

    /// <summary>Gets a value indicating whether Prospector and Scavenger will only track off-screen object while <see cref="ModKey"/> is held.</summary>
    [JsonProperty]
    [GMCMSection("prfs.scavenger_prospector")]
    [GMCMPriority(308)]
    public bool DisableAlwaysTrack { get; internal set; } = false;

    /// <summary>Gets a value indicating whether to restore the legacy purple arrow for Prospector Hunts, instead of the new audio cues.</summary>
    [JsonProperty]
    [GMCMSection("prfs.scavenger_prospector")]
    [GMCMPriority(309)]
    public bool UseLegacyProspectorHunt { get; internal set; } = false;

    /// <summary>
    ///     Gets the maximum multiplier that will be added to fish sold by Angler. if multiple new fish mods are installed,
    ///     you may want to adjust this to a sensible value.
    /// </summary>
    [JsonProperty]
    [GMCMSection("prfs.angler_aquarist")]
    [GMCMPriority(500)]
    [GMCMRange(0.25f, 4f)]
    [GMCMStep(0.25f)]
    public float AnglerPriceBonusCeiling
    {
        get => this._anglerPriceBonusCeiling;
        internal set
        {
            this._anglerPriceBonusCeiling = Math.Abs(value);
        }
    }

    /// <summary>
    ///     Gets a value indicating whether to display the MAX icon below fish in the Collections Menu which have been caught at the
    ///     maximum size.
    /// </summary>
    [JsonProperty]
    [GMCMSection("prfs.angler_aquarist")]
    [GMCMPriority(501)]
    public bool ShowFishCollectionMaxIcon { get; internal set; } = true;

    /// <summary>
    ///     Gets the maximum number of Fish Ponds that will be counted for catching bar loss compensation.
    /// </summary>
    [JsonProperty]
    [GMCMSection("prfs.angler_aquarist")]
    [GMCMPriority(502)]
    [GMCMRange(0, 24f)]
    public uint AquaristFishPondCeiling { get; internal set; } = 12;

    /// <summary>Gets the amount of junk items that must be collected from crab pots for every 1% of tax deduction the following season.</summary>
    [JsonProperty]
    [GMCMSection("prfs.conservationist")]
    [GMCMPriority(600)]
    [GMCMRange(10, 1000)]
    [GMCMStep(10)]
    public uint ConservationistTrashNeededPerTaxDeduction { get; internal set; } = 100;

    /// <summary>Gets the amount of junk items that must be collected from crab pots for every 1 point of friendship towards villagers.</summary>
    [JsonProperty]
    [GMCMSection("prfs.conservationist")]
    [GMCMPriority(601)]
    [GMCMRange(10, 1000)]
    [GMCMStep(10)]
    public uint ConservationistTrashNeededPerFriendshipPoint { get; internal set; } = 100;

    /// <summary>Gets the maximum income deduction allowed by the Ferngill Revenue Service.</summary>
    [JsonProperty]
    [GMCMSection("prfs.conservationist")]
    [GMCMPriority(602)]
    [GMCMRange(0.1f, 1f)]
    [GMCMStep(0.05f)]
    public float ConservationistTaxDeductionCeiling
    {
        get => this._conservationistTaxDeductionCeiling;
        internal set
        {
            this._conservationistTaxDeductionCeiling = Math.Abs(value);
        }
    }

    /// <summary>Gets a value indicating whether heavy tappers should yield twice the output stack instead of produce faster. This makes it more consistent with the new Heavy Furnace and less redudant with the Tapper profession.</summary>
    [JsonProperty]
    [GMCMSection("prfs.tapper")]
    [GMCMPriority(700)]
    public bool ImmersiveHeavyTapperYield
    {
        get => this._immersiveHeavyTapperYield;
        internal set
        {
            if (value != this._immersiveHeavyTapperYield)
            {
                ModHelper.GameContent.InvalidateCache("Data/BigCraftables");
            }

            this._immersiveHeavyTapperYield = value;
        }
    }

    /// <summary>Gets a value indicating whether regular trees should age like fruit trees, producing higher-quality syrups when tapped.</summary>
    [JsonProperty]
    [GMCMSection("prfs.tapper")]
    [GMCMPriority(701)]
    public bool AgingTreesQualitySyrups { get; internal set; } = true;
}
