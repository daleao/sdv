﻿namespace DaLion.Professions;

#region using directives

using DaLion.Professions.Framework.Configs;
using DaLion.Professions.Framework.UI;
using DaLion.Professions.Framework.VirtualProperties;
using DaLion.Shared.Integrations.GMCM.Attributes;
using DaLion.Shared.Pathfinding;
using Newtonsoft.Json;
using StardewModdingAPI.Utilities;

#endregion using directives

/// <summary>Config schema for the Professions mod.</summary>
public sealed class ProfessionsConfig
{
    private bool _enableGoldenOstrichMayo = true;
    private bool _enableSlimeGoods = true;
    private bool _immersiveDairyYield = true;
    private float _scavengerHuntHandicap = 1f;
    private float _prospectorHuntHandicap = 1f;
    private float _anglerPriceBonusCeiling = 1f;
    private float _conservationistTaxDeductionCeiling = 1f;
    private float _trackingPointerScale = 1f;
    private float _trackingPointerBobRate = 1f;
    private bool _immersiveHeavyTapperYield = true;
    private bool _useAsyncMinionPathfinder = true;

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

    /// <summary>Gets a value indicating whether to set golden and ostrich egg machine outputs to corresponding new mayo items.</summary>
    [JsonProperty]
    [GMCMSection("prfs.artisan_breeder_producer")]
    [GMCMPriority(100)]
    public bool EnableGoldenOstrichMayo
    {
        get => this._enableGoldenOstrichMayo;
        internal set
        {
            this._enableGoldenOstrichMayo = value;
            ModHelper.GameContent.InvalidateCache("Data/Machines");
            ModHelper.GameContent.InvalidateCache("Data/Objects");
        }
    }

    /// <summary>Gets a value indicating whether large eggs and milk should yield twice the output stack instead of higher quality.</summary>
    [JsonProperty]
    [GMCMSection("prfs.artisan_breeder_producer")]
    [GMCMPriority(101)]
    public bool ImmersiveDairyYield
    {
        get => this._immersiveDairyYield;
        internal set
        {
            this._immersiveDairyYield = value;
            ModHelper.GameContent.InvalidateCache("Data/Machines");
        }
    }

    /// <summary>Gets a value indicating whether Bee House products should be affected by Producer bonuses.</summary>
    [JsonProperty]
    [GMCMSection("prfs.artisan_breeder_producer")]
    [GMCMPriority(102)]
    public bool BeesAreAnimals { get; internal set; } = true;

    /// <summary>Gets the multiplier applied to the value of friendly animals sold by Breeder. This should only be used to compensate for third-party profit balancing mods.</summary>
    [JsonProperty]
    [GMCMSection("prfs.artisan_breeder_producer")]
    [GMCMPriority(103)]
    [GMCMRange(2f, 10f, 0.5f)]
    public float BreederFriendlyAnimalMultiplier { get; internal set; } = 10f;

    /// <summary>Gets the number of items that must be foraged before foraged items become iridium-quality.</summary>
    [JsonProperty]
    [GMCMSection("prfs.ecologist_gemologist")]
    [GMCMPriority(200)]
    [GMCMRange(0, 100, 10)]
    public uint ForagesNeededForBestQuality { get; internal set; } = 30;

    /// <summary>Gets the number of minerals that must be mined before mined minerals become iridium-quality.</summary>
    [JsonProperty]
    [GMCMSection("prfs.ecologist_gemologist")]
    [GMCMPriority(201)]
    [GMCMRange(0, 100, 10)]
    public uint MineralsNeededForBestQuality { get; internal set; } = 30;

    /// <summary>Gets a multiplier applied to the base chance that a Scavenger or Prospector hunt will trigger in the right conditions.</summary>
    [JsonProperty]
    [GMCMSection("prfs.scavenger_prospector")]
    [GMCMPriority(300)]
    [GMCMRange(0.5f, 3f, 0.25f)]
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
    [GMCMRange(0.5f, 3f, 0.2f)]
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
    [GMCMRange(0.5f, 3f, 0.2f)]
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
    [GMCMRange(0.2f, 5f, 0.2f)]
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
    [GMCMRange(0.5f, 2f, 0.05f)]
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
    [GMCMPriority(310)]
    public bool UseLegacyProspectorHunt { get; internal set; } = false;

    /// <summary>
    ///     Gets the maximum multiplier that will be added to fish sold by Angler. if multiple new fish mods are installed,
    ///     you may want to adjust this to a sensible value.
    /// </summary>
    [JsonProperty]
    [GMCMSection("prfs.angler_aquarist")]
    [GMCMPriority(500)]
    [GMCMRange(0.25f, 4f, 0.25f)]
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
    [GMCMRange(10, 1000, 10)]
    public uint ConservationistTrashNeededPerTaxDeduction { get; internal set; } = 100;

    /// <summary>Gets the amount of junk items that must be collected from crab pots for every 1 point of friendship towards villagers.</summary>
    [JsonProperty]
    [GMCMSection("prfs.conservationist")]
    [GMCMPriority(601)]
    [GMCMRange(10, 1000, 10)]
    public uint ConservationistTrashNeededPerFriendshipPoint { get; internal set; } = 100;

    /// <summary>Gets the maximum income deduction allowed by the Ferngill Revenue Service.</summary>
    [JsonProperty]
    [GMCMSection("prfs.conservationist")]
    [GMCMPriority(602)]
    [GMCMRange(0.1f, 1f, 0.05f)]
    public float ConservationistTaxDeductionCeiling
    {
        get => this._conservationistTaxDeductionCeiling;
        internal set
        {
            this._conservationistTaxDeductionCeiling = Math.Abs(value);
        }
    }

    /// <summary>Gets a value indicating whether to replace the tame Pyrotechnician with the wild Pyromaniac.</summary>
    [JsonProperty]
    [GMCMSection("prfs.demolitionist")]
    [GMCMPriority(699)]
    public bool Pyromania { get; internal set; } = false;

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

    /// <summary>Gets a value indicating whether to draw the currently equipped ammo over the slingshot's tooltip.</summary>
    [JsonProperty]
    [GMCMSection("prfs.rascal")]
    [GMCMPriority(800)]
    public bool ShowEquippedAmmo { get; internal set; } = true;

    /// <summary>Gets a value indicating whether to draw the Slime minions' health.</summary>
    [JsonProperty]
    [GMCMSection("prfs.rascal")]
    [GMCMPriority(801)]
    public bool ShowMinionHealth { get; internal set; } = true;

    /// <summary>Gets a value indicating whether to use the async version of the Slime Pathfinder.</summary>
    [JsonProperty]
    [GMCMSection("prfs.rascal")]
    [GMCMPriority(802)]
    public bool UseAsyncMinionPathfinder
    {
        get => this._useAsyncMinionPathfinder;
        internal set
        {
            if (Context.IsWorldReady)
            {
                const CollisionMask collisionMask = CollisionMask.Buildings | CollisionMask.Furniture |
                                                    CollisionMask.Objects | CollisionMask.TerrainFeatures |
                                                    CollisionMask.LocationSpecific;
                if (value)
                {
                    PathfinderAsync ??= new PathfindingManagerAsync(
                        EventManager,
                        (l, t) => l.isTilePassable(t) && (!l.IsTileOccupiedBy(t, collisionMask)));
                    foreach (var slime in GreenSlime_Piped.PipedSlimes)
                    {
                        PathfinderAsync.Register(slime, slime.currentLocation)
                            .QueueRequest(slime.TilePoint, slime.TilePoint);
                    }
                }
                else
                {
                    Pathfinder ??= new PathfindingManager(
                        EventManager,
                        (l, t) => l.isTilePassable(t) && (!l.IsTileOccupiedBy(t, collisionMask)));
                    foreach (var slime in GreenSlime_Piped.PipedSlimes)
                    {
                        Pathfinder.RequestFor(slime, slime.TilePoint, slime.TilePoint);
                    }
                }
            }

            this._useAsyncMinionPathfinder = value;
        }
    }

    /// <summary>Gets a value indicating whether to add slime cheese and mayo items to the game.</summary>
    [JsonProperty]
    [GMCMSection("prfs.rascal")]
    [GMCMPriority(803)]
    public bool EnableSlimeGoods
    {
        get => this._enableSlimeGoods;
        internal set
        {
            this._enableSlimeGoods = value;
            ModHelper.GameContent.InvalidateCache("Data/Machines");
            ModHelper.GameContent.InvalidateCache("Data/Objects");
        }
    }
}
