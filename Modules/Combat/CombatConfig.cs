namespace DaLion.Overhaul.Modules.Combat;

#region using directives

using System.Collections.Generic;
using DaLion.Overhaul.Modules.Combat.Enums;
using DaLion.Overhaul.Modules.Combat.Integrations;
using DaLion.Overhaul.Modules.Combat.VirtualProperties;
using DaLion.Overhaul.Modules.Core.ConfigMenu;
using DaLion.Shared.Constants;
using DaLion.Shared.Enums;
using DaLion.Shared.Extensions.Collections;
using DaLion.Shared.Extensions.SMAPI;
using DaLion.Shared.Integrations.GMCM;
using DaLion.Shared.Integrations.GMCM.Attributes;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using Resonance;
using StardewModdingAPI.Utilities;
using StardewValley.Objects;

#endregion using directives

/// <summary>The user-configurable settings for CMBT.</summary>
public sealed class CombatConfig
{
    private bool _newResistanceFormula = true;
    private bool _enableWeaponOverhaul = true;
    private bool _enableStabbingSwords = true;
    private bool _enableMeleeComboHits = true;
    private bool _rebalancedRings = true;
    private bool _craftableGemstoneRings = true;
    private bool _enableInfinityBand = true;
    private uint _chordSoundDuration = 1000;
    private bool _colorfulResonances = true;
    private LightsourceTexture _resonanceLightsourceTexture = LightsourceTexture.Patterned;
    private bool _newPrismaticEnchantments = true;
    private bool _dwarvenLegacy = true;
    private bool _enableHeroQuest = true;
    private int _iridiumBarsPerGalaxyWeapon = 10;
    private float _ruinBladeDotMultiplier = 1f;
    private QuestDifficulty _heroQuestDifficulty = QuestDifficulty.Medium;
    private float _monsterSpawnChanceMultiplier = 1f;
    private float _monsterHealthMultiplier = 1f;
    private float _monsterDamageMultiplier = 1f;
    private float _monsterDefenseMultiplier = 1f;
    private int _monsterHealthSummand = 0;
    private int _monsterDamageSummand = 0;
    private int _monsterDefenseSummand = 1;
    private bool _enableAutoSelection = true;
    private uint _meleeAutoSelectionRange = 2;
    private uint _rangedAutoSelectionRange = 4;
    private SocketStyle _forgeSocketStyle = SocketStyle.Diamond;

    #region dropdown enums

    /// <summary>The style used to draw forged gemstones.</summary>
    public enum SocketStyle
    {
        /// <summary>None. Keep vanilla style.</summary>
        None,

        /// <summary>A diamond-shaped icon.</summary>
        Diamond,

        /// <summary>A more rounded icon.</summary>
        Round,

        /// <summary>Shaped like an iridium ore.</summary>
        Iridium,
    }

    /// <summary>The position of the forged gemstones.</summary>
    public enum SocketPosition
    {
        /// <summary>The normal position, immediately above the item's description.</summary>
        Standard,

        /// <summary>Above the horizontal separator, immediately below the item's name and level.</summary>
        AboveSeparator,
    }

    /// <summary>The style used to display stat bonuses in weapon tooltips.</summary>
    public enum TooltipStyle
    {
        /// <summary>Display the absolute value of the stat, minus it's default value for the weapon type.</summary>
        Absolute,

        /// <summary>Display the relative value of the stat, with respect to the default value for the weapon type.</summary>
        Relative,

        /// <summary>The vanilla confusing nonsense.</summary>
        Vanilla,
    }

    /// <summary>The difficulty level of the proven conditions for the virtue trials.</summary>
    public enum QuestDifficulty
    {
        /// <summary>Easy.</summary>
        Easy,

        /// <summary>Medium.</summary>
        Medium,

        /// <summary>Hard.</summary>
        Hard,
    }

    /// <summary>The texture that should be used as the resonance light source.</summary>
    public enum LightsourceTexture
    {
        /// <summary>The default, Vanilla sconce light texture.</summary>
        Sconce = 4,

        /// <summary>A more opaque sconce light texture.</summary>
        Stronger = 100,

        /// <summary>A floral-patterned light texture.</summary>
        Patterned = 101,
    }

    #endregion dropdown enums

    #region general

    /// <summary>Gets a value indicating whether to overhaul the defense stat with better scaling and other features.</summary>
    [JsonProperty]
    [GMCMSection("cmbt.general")]
    [GMCMPriority(0)]
    public bool NewResistanceFormula
    {
        get => this._newResistanceFormula;
        internal set
        {
            if (value == this._newResistanceFormula)
            {
                return;
            }

            this._newResistanceFormula = value;
            ModHelper.GameContent.InvalidateCacheAndLocalized("Data/ObjectInformation");
            if (!Context.IsWorldReady)
            {
                return;
            }

            Utility.iterateAllItems(item =>
            {
                if (item is not Ring { ParentSheetIndex: ObjectIds.TopazRing } topaz)
                {
                    return;
                }

                var key = "rings.topaz.desc" + (value ? "resist" : "defense");
                topaz.description = _I18n.Get(key);
            });
        }
    }

    /// <summary>Gets a value indicating whether to overhaul the knockback stat adding collision damage.</summary>
    [JsonProperty]
    [GMCMSection("cmbt.general")]
    [GMCMPriority(1)]
    public bool KnockbackHurts { get; internal set; } = true;

    /// <summary>Gets a value indicating whether back attacks gain double crit. chance.</summary>
    [JsonProperty]
    [GMCMSection("cmbt.general")]
    [GMCMPriority(3)]
    public bool CriticalBackAttacks { get; internal set; } = true;

    /// <summary>Gets a value indicating whether to enable status conditions like Bleed and Stun on enemies.</summary>
    [JsonProperty]
    [GMCMSection("cmbt.general")]
    [GMCMPriority(4)]
    public bool EnableStatusConditions { get; internal set; } = true;

    #endregion general

    /// <summary>Gets a value indicating whether to apply all features relating to the weapon and slingshot re-balance, including weapon tiers, shops, Mine chests and monster drops.</summary>
    [JsonProperty]
    [GMCMSection("cmbt.weapons")]
    [GMCMPriority(100)]
    public bool EnableWeaponOverhaul
    {
        get => this._enableWeaponOverhaul;
        internal set
        {
            if (value == this._enableWeaponOverhaul)
            {
                return;
            }

            if (!value)
            {
                this.EnableStabbingSwords = false;
                this.EnableMeleeComboHits = false;
            }

            this._enableWeaponOverhaul = value;
            ModHelper.GameContent.InvalidateCacheAndLocalized("Data/weapons");
            if (Context.IsWorldReady)
            {
                CombatModule.RefreshAllWeapons(value
                    ? WeaponRefreshOption.Initial
                    : WeaponRefreshOption.FromData);
            }
        }
    }

    /// <summary>Gets a value indicating whether replace the starting Rusty Sword with a Wooden Blade.</summary>
    [JsonProperty]
    [GMCMSection("cmbt.weapons")]
    [GMCMPriority(101)]
    public bool WoodyReplacesRusty { get; internal set; } = true;

    #region melee

    /// <summary>Gets a value indicating whether replace the defensive special move of some swords with an offensive lunge move.</summary>
    [JsonProperty]
    [GMCMSection("cmbt.weapons")]
    [GMCMPriority(120)]
    public bool EnableStabbingSwords
    {
        get => this._enableStabbingSwords;
        internal set
        {
            if (value == this._enableStabbingSwords)
            {
                return;
            }

            if (value && !this.EnableWeaponOverhaul)
            {
                Log.W("Stabbing Swords feature requires that Weapon Overhaul be set to true.");
                return;
            }

            this._enableStabbingSwords = value;
            ModHelper.GameContent.InvalidateCacheAndLocalized("Data/weapons");
            if (!Context.IsWorldReady)
            {
                return;
            }

            if (value)
            {
                CombatModule.ConvertAllStabbingSwords();
            }
            else
            {
                CombatModule.RevertAllStabbingSwords();
            }
        }
    }

    /// <summary>Gets a set of user-defined modded swords which should be treated as Stabby swords.</summary>
    [JsonProperty]
    [GMCMPriority(121)]
    [GMCMOverride(typeof(GenericModConfigMenu), "CombatConfigStabbingSwordsOverride")]
    public HashSet<string> StabbingSwords { get; internal set; } = new()
    {
        "Bone Sword",
        "Steel Smallsword",
        "Cutlass",
        "Rapier",
        "Steel Falchion",
        "Pirate's Sword",
        "Lava Katana",
        "Dragontooth Cutlass",
        "Blade of Ruin",
        "Sword Fish",
        "Strawblaster",
    };

    /// <summary>Gets a value indicating whether to replace vanilla weapon spam with a more strategic combo system.</summary>
    [JsonProperty]
    [GMCMSection("cmbt.weapons")]
    [GMCMPriority(122)]
    public bool EnableMeleeComboHits
    {
        get => this._enableMeleeComboHits;
        internal set
        {
            if (value && !this.EnableWeaponOverhaul)
            {
                Log.W("Melee Combo Framework requires that Weapon Overhaul be set to true.");
                return;
            }

            this._enableMeleeComboHits = value;
        }
    }

    /// <summary>Gets the number of hits in each weapon type's combo.</summary>
    [JsonProperty]
    [GMCMSection("cmbt.weapons")]
    [GMCMPriority(123)]
    [GMCMRange(0, 10)]
    public Dictionary<string, int> ComboHitsPerWeaponType { get; internal set; } = new()
    {
        { WeaponType.StabbingSword.ToString(), 4 },
        { WeaponType.DefenseSword.ToString(), 4 },
        { WeaponType.Club.ToString(), 2 },
    };

    /// <summary>Gets a value indicating whether to keep swiping while the "use tool" key is held.</summary>
    [JsonProperty]
    [GMCMSection("cmbt.weapons")]
    [GMCMPriority(124)]
    public bool SwipeHold { get; internal set; } = true;

    /// <summary>Gets a value indicating whether defense should improve parry damage.</summary>
    [JsonProperty]
    [GMCMSection("cmbt.weapons")]
    [GMCMPriority(125)]
    public bool DefenseImprovesParry { get; internal set; } = true;

    /// <summary>Gets a value indicating whether to guarantee smash crit on Duggies and guarantee miss on gliders.</summary>
    [JsonProperty]
    [GMCMSection("cmbt.weapons")]
    [GMCMPriority(126)]
    public bool GroundedClubSmash { get; internal set; } = true;

    #endregion melee

    #region ranged

    /// <summary>Gets a value indicating whether to allow slingshots to deal critical damage and be affected by critical modifiers.</summary>
    [JsonProperty]
    [GMCMSection("cmbt.weapons")]
    [GMCMPriority(140)]
    public bool EnableRangedCriticalHits { get; internal set; } = true;

    /// <summary>Gets a value indicating whether to enable the custom slingshot stun smack special move.</summary>
    [JsonProperty]
    [GMCMSection("cmbt.weapons")]
    [GMCMPriority(141)]
    public bool EnableSlingshotSpecialMove { get; internal set; } = true;

    /// <summary>Gets a value indicating whether to allow forging the Infinity Slingshot.</summary>
    [JsonProperty]
    [GMCMSection("cmbt.weapons")]
    [GMCMPriority(142)]
    public bool EnableInfinitySlingshot { get; internal set; } = true;

    /// <summary>Gets a value indicating whether projectiles should not be useless for the first 100ms.</summary>
    [JsonProperty]
    [GMCMSection("cmbt.weapons")]
    [GMCMPriority(143)]
    public bool RemoveSlingshotGracePeriod { get; internal set; } = true;

    #endregion ranged

    #region rings & enchantments

    /// <summary>Gets a value indicating whether to improve certain underwhelming rings.</summary>
    [JsonProperty]
    [GMCMSection("cmbt.rings_enchantments")]
    [GMCMPriority(200)]
    public bool RebalancedRings
    {
        get => this._rebalancedRings;
        internal set
        {
            if (value == this._rebalancedRings)
            {
                return;
            }

            this._rebalancedRings = value;
            ModHelper.GameContent.InvalidateCacheAndLocalized("Data/ObjectInformation");
        }
    }

    /// <summary>Gets a value indicating whether to add new combat recipes for crafting gemstone rings.</summary>
    [JsonProperty]
    [GMCMSection("cmbt.rings_enchantments")]
    [GMCMPriority(201)]
    public bool CraftableGemstoneRings
    {
        get => this._craftableGemstoneRings;
        internal set
        {
            if (value == this._craftableGemstoneRings)
            {
                return;
            }

            this._craftableGemstoneRings = value;
            ModHelper.GameContent.InvalidateCacheAndLocalized("Data/CraftingRecipes");
            ModHelper.GameContent.InvalidateCacheAndLocalized("Maps/springobjects");
        }
    }

    /// <summary>Gets a value indicating whether to replace the Iridium Band recipe and effect.</summary>
    [JsonProperty]
    [GMCMSection("cmbt.rings_enchantments")]
    [GMCMPriority(202)]
    public bool EnableInfinityBand
    {
        get => this._enableInfinityBand;
        internal set
        {
            if (value == this._enableInfinityBand)
            {
                return;
            }

            this._enableInfinityBand = value;
            ModHelper.GameContent.InvalidateCacheAndLocalized("Data/CraftingRecipes");
            ModHelper.GameContent.InvalidateCacheAndLocalized("Data/ObjectInformation");
            ModHelper.GameContent.InvalidateCacheAndLocalized("Maps/springobjects");
        }
    }

    /// <summary>Gets a value indicating whether to allow gemstone resonance to take place.</summary>
    [JsonProperty]
    [GMCMSection("cmbt.rings_enchantments")]
    [GMCMPriority(203)]
    public bool EnableGemstoneResonance { get; internal set; } = true;

    /// <summary>Gets a value indicating whether to allow gemstone resonance to take place.</summary>
    [JsonProperty]
    [GMCMSection("cmbt.rings_enchantments")]
    [GMCMPriority(204)]
    public bool PlayChord { get; internal set; } = true;

    /// <summary>Gets a value indicating whether to allow gemstone resonance to take place.</summary>
    [JsonProperty]
    [GMCMSection("cmbt.rings_enchantments")]
    [GMCMPriority(205)]
    [GMCMRange(500, 2500)]
    [GMCMInterval(100)]
    public uint ChordSoundDuration
    {
        get => this._chordSoundDuration;
        internal set
        {
            this._chordSoundDuration = value;
            if (Context.IsWorldReady)
            {
                HarmonicInterval.RecalculateLinSpace();
            }
        }
    }

    /// <summary>Gets a value indicating whether the resonance glow should inherit the root note's color.</summary>
    [JsonProperty]
    [GMCMSection("cmbt.rings_enchantments")]
    [GMCMPriority(206)]
    public bool ColorfulResonances
    {
        get => this._colorfulResonances;
        internal set
        {
            if (value == this._colorfulResonances)
            {
                return;
            }

            this._colorfulResonances = value;
            if (Context.IsWorldReady)
            {
                Game1.player.Get_ResonatingChords().ForEach(chord => chord.ResetLightSource());
            }
        }
    }

    /// <summary>Gets a value indicating the texture that should be used as the resonance light source.</summary>
    [JsonProperty]
    [GMCMSection("cmbt.rings_enchantments")]
    [GMCMPriority(207)]
    public LightsourceTexture ResonanceLightsourceTexture
    {
        get => this._resonanceLightsourceTexture;
        internal set
        {
            if (value == this._resonanceLightsourceTexture)
            {
                return;
            }

            this._resonanceLightsourceTexture = value;
            if (Context.IsWorldReady)
            {
                Game1.player.Get_ResonatingChords().ForEach(chord => chord.ResetLightSource());
            }
        }
    }

    /// <summary>Gets a value indicating whether to improve certain underwhelming gemstone effects.</summary>
    [JsonProperty]
    [GMCMSection("cmbt.rings_enchantments")]
    [GMCMPriority(208)]
    public bool RebalancedGemstones { get; internal set; } = true;

    /// <summary>Gets a value indicating whether to replace vanilla weapon enchantments with all-new melee and ranged enchantments.</summary>
    [JsonProperty]
    [GMCMSection("cmbt.rings_enchantments")]
    [GMCMPriority(209)]
    public bool NewPrismaticEnchantments
    {
        get => this._newPrismaticEnchantments;
        internal set
        {
            if (value == this._newPrismaticEnchantments)
            {
                return;
            }

            this._newPrismaticEnchantments = value;
            if (Context.IsWorldReady)
            {
                Reflector.GetStaticFieldSetter<List<BaseEnchantment>?>(typeof(BaseEnchantment), "_enchantments")
                    .Invoke(null);
            }
        }
    }

    #endregion rings & enchantments

    #region quests

    /// <summary>Gets a value indicating whether to enable Clint's forging mechanic for Masterwork weapons.</summary>
    [JsonProperty]
    [GMCMSection("cmbt.quests")]
    [GMCMPriority(300)]
    public bool DwarvenLegacy
    {
        get => this._dwarvenLegacy;
        internal set
        {
            if (value == this._dwarvenLegacy)
            {
                return;
            }

            this._dwarvenLegacy = value;
            ModHelper.GameContent.InvalidateCacheAndLocalized("Data/Events/Blacksmith");
            ModHelper.GameContent.InvalidateCacheAndLocalized("Data/Quests");
            ModHelper.GameContent.InvalidateCacheAndLocalized("Data/Monsters");
            ModHelper.GameContent.InvalidateCacheAndLocalized("Data/weapons");
        }
    }

    /// <summary>Gets a value indicating whether replace lame Galaxy and Infinity weapons with something truly legendary.</summary>
    [JsonProperty]
    [GMCMSection("cmbt.quests")]
    [GMCMPriority(301)]
    public bool EnableHeroQuest
    {
        get => this._enableHeroQuest;
        internal set
        {
            if (value == this._enableHeroQuest)
            {
                return;
            }

            this._enableHeroQuest = value;
            ModHelper.GameContent.InvalidateCacheAndLocalized("Data/Events/WizardHouse");
            ModHelper.GameContent.InvalidateCacheAndLocalized("Data/ObjectInformation");
            ModHelper.GameContent.InvalidateCacheAndLocalized("Data/weapons");
            ModHelper.GameContent.InvalidateCacheAndLocalized("Strings/Locations");
            ModHelper.GameContent.InvalidateCacheAndLocalized("Strings/StringsFromCSFiles");
            ModHelper.GameContent.InvalidateCache("TileSheets/Projectiles");
            if ((VanillaTweaksIntegration.IsValueCreated && VanillaTweaksIntegration.Instance.IsRegistered) ||
                (SimpleWeaponsIntegration.IsValueCreated && SimpleWeaponsIntegration.Instance.IsRegistered))
            {
                ModHelper.GameContent.InvalidateCache("TileSheets/weapons");
            }
        }
    }

    /// <summary>Gets a value indicating the number of Iridium Bars required to receive a Galaxy weapon.</summary>
    [JsonProperty]
    [GMCMSection("cmbt.quests")]
    [GMCMPriority(302)]
    [GMCMRange(0, 50)]
    public int IridiumBarsPerGalaxyWeapon
    {
        get => this._iridiumBarsPerGalaxyWeapon;
        internal set
        {
            this._iridiumBarsPerGalaxyWeapon = Math.Max(value, 0);
        }
    }

    /// <summary>Gets a factor that can be used to reduce the Ruined Blade's damage-over-time effect.</summary>
    [JsonProperty]
    [GMCMSection("cmbt.quests")]
    [GMCMPriority(303)]
    [GMCMRange(0, 4)]
    public float RuinBladeDotMultiplier
    {
        get => this._ruinBladeDotMultiplier;
        internal set
        {
            this._ruinBladeDotMultiplier = Math.Max(value, 0f);
        }
    }

    /// <summary>Gets a value indicating whether the Blade of Ruin can be deposited in chests.</summary>
    [JsonProperty]
    [GMCMSection("cmbt.quests")]
    [GMCMPriority(304)]
    public bool CanStoreRuinBlade { get; internal set; } = false;

    /// <summary>Gets a value indicating the difficulty of the proven conditions for each virtue trial.</summary>
    [JsonProperty]
    [GMCMSection("cmbt.quests")]
    [GMCMPriority(305)]
    public QuestDifficulty HeroQuestDifficulty
    {
        get => this._heroQuestDifficulty;
        internal set
        {
            if (value == this._heroQuestDifficulty)
            {
                return;
            }

            this._heroQuestDifficulty = value;
            if (Context.IsWorldReady && CombatModule.State.HeroQuest is { } quest)
            {
                Virtue.List.ForEach(virtue => quest.UpdateTrialProgress(virtue));
            }
        }
    }

    #endregion quests

    #region enemies

    /// <summary>Gets a value indicating whether randomizes monster stats to add variability to monster encounters.</summary>
    [JsonProperty]
    [GMCMSection("cmbt.enemies")]
    [GMCMPriority(400)]
    public bool VariedEncounters { get; internal set; } = true;

    /// <summary>Gets a multiplier which allows increasing the spawn chance of monsters in dungeons.</summary>
    [JsonProperty]
    [GMCMSection("cmbt.enemies")]
    [GMCMPriority(401)]
    [GMCMRange(0.1f, 10f)]
    public float MonsterSpawnChanceMultiplier
    {
        get => this._monsterSpawnChanceMultiplier;
        internal set
        {
            this._monsterSpawnChanceMultiplier = Math.Max(value, 0.1f);
        }
    }

    /// <summary>Gets a multiplier which allows scaling the health of all monsters.</summary>
    [JsonProperty]
    [GMCMSection("cmbt.enemies")]
    [GMCMPriority(402)]
    [GMCMRange(0.1f, 10f)]
    public float MonsterHealthMultiplier
    {
        get => this._monsterHealthMultiplier;
        internal set
        {
            this._monsterHealthMultiplier = Math.Max(value, 0.1f);
        }
    }

    /// <summary>Gets a multiplier which allows scaling the damage dealt by all monsters.</summary>
    [JsonProperty]
    [GMCMSection("cmbt.enemies")]
    [GMCMPriority(403)]
    [GMCMRange(0.1f, 10f)]
    public float MonsterDamageMultiplier
    {
        get => this._monsterDamageMultiplier;
        internal set
        {
            this._monsterDamageMultiplier = Math.Max(value, 0.1f);
        }
    }

    /// <summary>Gets a multiplier which allows scaling the resistance of all monsters.</summary>
    [JsonProperty]
    [GMCMSection("cmbt.enemies")]
    [GMCMPriority(404)]
    [GMCMRange(0.1f, 10f)]
    public float MonsterDefenseMultiplier
    {
        get => this._monsterDefenseMultiplier;
        internal set
        {
            this._monsterDefenseMultiplier = Math.Max(value, 0.1f);
        }
    }

    /// <summary>Gets a summand which is added to the resistance of all monsters (before the multiplier).</summary>
    [JsonProperty]
    [GMCMSection("cmbt.enemies")]
    [GMCMPriority(405)]
    [GMCMRange(-100, 100)]
    [GMCMInterval(10)]
    public int MonsterHealthSummand
    {
        get => this._monsterHealthSummand;
        internal set
        {
            this._monsterHealthSummand = Math.Max(value, -100);
        }
    }

    /// <summary>Gets a summand which is added to the resistance of all monsters (before the multiplier).</summary>
    [JsonProperty]
    [GMCMSection("cmbt.enemies")]
    [GMCMPriority(406)]
    [GMCMRange(-50, 50)]
    public int MonsterDamageSummand
    {
        get => this._monsterDamageSummand;
        internal set
        {
            this._monsterDamageSummand = Math.Max(value, -50);
        }
    }

    /// <summary>Gets a summand which is added to the resistance of all monsters (before the multiplier).</summary>
    [JsonProperty]
    [GMCMSection("cmbt.enemies")]
    [GMCMPriority(407)]
    [GMCMRange(-10, 10)]
    public int MonsterDefenseSummand
    {
        get => this._monsterDefenseSummand;
        internal set
        {
            this._monsterDefenseSummand = Math.Max(value, -10);
        }
    }

    #endregion enemies

    #region controls & ui

    /// <summary>Gets a value indicating whether to allow drifting in the movement direction when swinging weapons.</summary>
    [JsonProperty]
    [GMCMSection("controls_ui")]
    [GMCMPriority(500)]
    public bool SlickMoves { get; internal set; } = true;

    /// <summary>Gets a value indicating whether face the current cursor position before swinging your weapon.</summary>
    [JsonProperty]
    [GMCMSection("controls_ui")]
    [GMCMPriority(501)]
    public bool FaceMouseCursor { get; internal set; } = true;

    /// <summary>Gets a value indicating whether to allow auto-selecting a weapon or slingshot.</summary>
    [JsonProperty]
    [GMCMSection("controls_ui")]
    [GMCMPriority(502)]
    public bool EnableAutoSelection
    {
        get => this._enableAutoSelection;
        internal set
        {
            this._enableAutoSelection = value;
            if (value || !Context.IsWorldReady)
            {
                return;
            }

            CombatModule.State.AutoSelectableMelee = null;
            CombatModule.State.AutoSelectableRanged = null;
        }
    }

    /// <summary>Gets a value indicating how close an enemy must be to auto-select a weapon, in tiles.</summary>
    [JsonProperty]
    [GMCMSection("controls_ui")]
    [GMCMPriority(503)]
    [GMCMRange(1, 5)]
    public uint MeleeAutoSelectionRange
    {
        get => this._meleeAutoSelectionRange;
        internal set
        {
            this._meleeAutoSelectionRange = Math.Max(value, 1);
        }
    }

    /// <summary>Gets a value indicating how close an enemy must be to auto-select a slingshot, in tiles.</summary>
    [JsonProperty]
    [GMCMSection("controls_ui")]
    [GMCMPriority(504)]
    [GMCMRange(1, 10)]
    public uint RangedAutoSelectionRange
    {
        get => this._rangedAutoSelectionRange;
        internal set
        {
            this._rangedAutoSelectionRange = Math.Max(value, 1);
        }
    }

    /// <summary>Gets the chosen key(s) for toggling auto-selection.</summary>
    [JsonProperty]
    [GMCMSection("controls_ui")]
    [GMCMPriority(505)]
    public KeybindList SelectionKey { get; internal set; } = KeybindList.Parse("LeftShift, LeftShoulder");

    /// <summary>Gets the <see cref="Color"/> used to indicate tools enabled or auto-selection.</summary>
    [JsonProperty]
    [GMCMSection("controls_ui")]
    [GMCMPriority(506)]
    [GMCMColorPicker(false, (uint)IGenericModConfigMenuOptionsApi.ColorPickerStyle.RGBSliders)]
    [GMCMDefaultColor(0, 255, 255)]
    public Color SelectionBorderColor { get; internal set; } = Color.Aqua;

    /// <summary>Gets a value indicating whether to color-code tool names, <see href="https://tvtropes.org/pmwiki/pmwiki.php/Main/ColourCodedForYourConvenience"> for your convenience</see>.</summary>
    [JsonProperty]
    [GMCMSection("controls_ui")]
    [GMCMPriority(507)]
    public bool ColorCodedForYourConvenience { get; internal set; } = true;

    /// <summary>Gets the <see cref="Color"/> used by common-tier weapons.</summary>
    [JsonProperty]
    [GMCMSection("controls_ui")]
    [GMCMPriority(508)]
    [GMCMOverride(typeof(GenericModConfigMenu), "CombatConfigColorByTierOverride")]
    public Color[] ColorByTier { get; internal set; } =
    {
        new(34, 17, 34),
        Color.Green,
        Color.Blue,
        Color.Purple,
        Color.Red,
        Color.MonoGameOrange,
        Color.MonoGameOrange,
    };

    /// <summary>Gets the style of the sprite used to represent gemstone forges in tooltips.</summary>
    [JsonProperty]
    [GMCMSection("controls_ui")]
    [GMCMPriority(509)]
    public SocketStyle ForgeSocketStyle
    {
        get => this._forgeSocketStyle;
        internal set
        {
            if (value == this._forgeSocketStyle)
            {
                return;
            }

            this._forgeSocketStyle = value;
            ModHelper.GameContent.InvalidateCache($"{Manifest.UniqueID}/GemstoneSockets");
        }
    }

    /// <summary>Gets the relative position where forge gemstones should be drawn.</summary>
    [JsonProperty]
    [GMCMSection("controls_ui")]
    [GMCMPriority(510)]
    public SocketPosition ForgeSocketPosition { get; internal set; } = SocketPosition.AboveSeparator;

    /// <summary>Gets the style of the tooltips for displaying stat bonuses for weapons.</summary>
    [JsonProperty]
    [GMCMSection("controls_ui")]
    [GMCMPriority(511)]
    public TooltipStyle WeaponTooltipStyle { get; internal set; }

    /// <summary>Gets a value indicating whether to override the draw method to include the currently-equipped ammo.</summary>
    [JsonProperty]
    [GMCMSection("controls_ui")]
    [GMCMPriority(512)]
    public bool DrawCurrentAmmo { get; internal set; } = true;

    /// <summary>Gets a value indicating whether to replace the mouse cursor with a bulls-eye while firing.</summary>
    [JsonProperty]
    [GMCMSection("controls_ui")]
    [GMCMPriority(513)]
    public bool BullseyeReplacesCursor { get; internal set; } = true;

    #endregion controls & ui
}
