namespace DaLion.Overhaul.Modules.Combat;

#region using directives

using System.Collections.Generic;
using DaLion.Shared.Enums;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using StardewModdingAPI.Utilities;

#endregion using directives

/// <summary>The user-configurable settings for CMBT.</summary>
public sealed class Config : Shared.Configs.Config
{
    #region dropdown enums

    /// <summary>The style used to draw forged gemstones.</summary>
    public enum ForgeSocketStyle
    {
        /// <summary>A diamond-shaped icon.</summary>
        Diamond,

        /// <summary>A more rounded icon.</summary>
        Round,

        /// <summary>Shaped like an iridium ore.</summary>
        Iridium,
    }

    /// <summary>The position of the forged gemstones.</summary>
    public enum ForgeSocketPosition
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
    public enum Difficulty
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

    #region general combat

    /// <summary>Gets a value indicating whether to enable status conditions like Bleed and Stun on enemies.</summary>
    [JsonProperty]
    public bool EnableStatusConditions { get; internal set; } = true;

    /// <summary>Gets a value indicating whether to overhaul the knockback stat adding collision damage.</summary>
    [JsonProperty]
    public bool EnableKnockbackDamage { get; internal set; } = true;

    /// <summary>Gets a value indicating whether back attacks gain double crit. chance.</summary>
    [JsonProperty]
    public bool CriticalBackAttacks { get; internal set; } = true;

    /// <summary>Gets a value indicating whether to overhaul the defense stat with better scaling and other features.</summary>
    [JsonProperty]
    public bool NewResistanceFormula { get; internal set; } = true;

    #endregion general combat

    #region items

    /// <summary>Gets a value indicating whether to apply all features relating to the weapon and slingshot re-balance, including weapon tiers, shops, Mine chests and monster drops.</summary>
    [JsonProperty]
    public bool EnableWeaponOverhaul { get; internal set; } = true;

    #region melee

    /// <summary>Gets a value indicating whether to replace vanilla weapon spam with a more strategic combo system.</summary>
    [JsonProperty]
    public bool EnableComboHits { get; internal set; } = true;

    /// <summary>Gets the number of hits in each weapon type's combo.</summary>
    [JsonProperty]
    public Dictionary<WeaponType, int> ComboHitsPerWeapon { get; internal set; } = new()
    {
        { WeaponType.StabbingSword, 4 }, { WeaponType.DefenseSword, 4 }, { WeaponType.Club, 2 },
    };

    /// <summary>Gets a value indicating whether to keep swiping while the "use tool" key is held.</summary>
    [JsonProperty]
    public bool SwipeHold { get; internal set; } = true;

    /// <summary>Gets a value indicating whether replace the defensive special move of some swords with an offensive lunge move.</summary>
    [JsonProperty]
    public bool EnableStabbingSwords { get; internal set; } = true;

    /// <summary>Gets a set of user-defined modded swords which should be treated as Stabby swords.</summary>
    [JsonProperty]
    public string[] StabbingSwords { get; internal set; } =
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
        "Galaxy Sword",
        "Infinity Blade",
        "Strawblaster",
    };

    /// <summary>Gets a value indicating whether defense should improve parry damage.</summary>
    [JsonProperty]
    public bool DefenseImprovesParry { get; internal set; } = true;

    /// <summary>Gets a value indicating whether to guarantee smash crit on Duggies and guarantee miss on gliders.</summary>
    [JsonProperty]
    public bool GroundedClubSmash { get; internal set; } = true;

    #endregion melee

    #region ranged

    /// <summary>Gets a value indicating whether to allow slingshots to deal critical damage and be affected by critical modifiers.</summary>
    [JsonProperty]
    public bool EnableRangedCriticalHits { get; internal set; } = true;

    /// <summary>Gets a value indicating whether to enable the custom slingshot stun smack special move.</summary>
    [JsonProperty]
    public bool EnableSlingshotSpecialMove { get; internal set; } = true;

    /// <summary>Gets a value indicating whether to allow forging the Infinity Slingshot.</summary>
    [JsonProperty]
    public bool EnableInfinitySlingshot { get; internal set; } = true;

    /// <summary>Gets a value indicating whether projectiles should not be useless for the first 100ms.</summary>
    [JsonProperty]
    public bool RemoveSlingshotGracePeriod { get; internal set; } = true;

    #endregion ranged

    #region rings

    /// <summary>Gets a value indicating whether to improve certain underwhelming rings.</summary>
    [JsonProperty]
    public bool RebalancedRings { get; internal set; } = true;

    /// <summary>Gets a value indicating whether to add new combat recipes for crafting gemstone rings.</summary>
    [JsonProperty]
    public bool CraftableGemstoneRings { get; internal set; } = true;

    /// <summary>Gets a value indicating whether to replace the Iridium Band recipe and effect.</summary>
    [JsonProperty]
    public bool EnableInfinityBand { get; internal set; } = true;

    /// <summary>Gets a value indicating whether to allow gemstone resonance to take place.</summary>
    [JsonProperty]
    public bool EnableResonances { get; internal set; } = true;

    /// <summary>Gets a value indicating whether the resonance glow should inherit the root note's color.</summary>
    [JsonProperty]
    public bool ColorfulResonances { get; internal set; } = true;

    /// <summary>Gets a value indicating the texture that should be used as the resonance light source.</summary>
    [JsonProperty]
    public LightsourceTexture ResonanceLightsourceTexture { get; internal set; } = LightsourceTexture.Sconce;

    #endregion rings

    /// <summary>Gets a value indicating whether to improve certain underwhelming gemstone effects.</summary>
    [JsonProperty]
    public bool RebalancedGemstones { get; internal set; } = true;

    /// <summary>Gets a value indicating whether to replace vanilla weapon enchantments with all-new melee and ranged enchantments.</summary>
    [JsonProperty]
    public bool NewPrismaticEnchantments { get; internal set; } = true;

    #endregion items

    #region quests

    /// <summary>Gets a value indicating whether replace the starting Rusty Sword with a Wooden Blade.</summary>
    [JsonProperty]
    public bool WoodyReplacesRusty { get; internal set; } = true;

    /// <summary>Gets a value indicating whether to enable Clint's forging mechanic for Masterwork weapons.</summary>
    [JsonProperty]
    public bool DwarvenLegacy { get; internal set; } = true;

    /// <summary>Gets a value indicating whether replace lame Galaxy and Infinity weapons with something truly legendary.</summary>
    [JsonProperty]
    public bool EnableHeroQuest { get; internal set; } = true;

    /// <summary>Gets a value indicating the number of Iridium Bars required to receive a Galaxy weapon.</summary>
    [JsonProperty]
    public int IridiumBarsPerGalaxyWeapon { get; internal set; } = 10;

    /// <summary>Gets a factor that can be used to reduce the Ruined Blade's damage-over-time effect.</summary>
    [JsonProperty]
    public float RuinBladeDotMultiplier { get; internal set; } = 1f;

    /// <summary>Gets a value indicating whether the Blade of Ruin can be deposited in chests.</summary>
    [JsonProperty]
    public bool CanStoreRuinBlade { get; internal set; } = false;

    /// <summary>Gets a value indicating the difficulty of the proven conditions for each virtue trial.</summary>
    [JsonProperty]
    public Difficulty HeroQuestDifficulty { get; internal set; } = Difficulty.Medium;

    #endregion quests

    #region enemies

    /// <summary>Gets a summand which is added to the resistance of all monsters (before the multiplier).</summary>
    [JsonProperty]
    public int MonsterHealthSummand { get; internal set; } = 0;

    /// <summary>Gets a summand which is added to the resistance of all monsters (before the multiplier).</summary>
    [JsonProperty]
    public int MonsterDamageSummand { get; internal set; } = 0;

    /// <summary>Gets a summand which is added to the resistance of all monsters (before the multiplier).</summary>
    [JsonProperty]
    public int MonsterDefenseSummand { get; internal set; } = 1;

    /// <summary>Gets a multiplier which allows scaling the health of all monsters.</summary>
    [JsonProperty]
    public float MonsterHealthMultiplier { get; internal set; } = 1.5f;

    /// <summary>Gets a multiplier which allows scaling the damage dealt by all monsters.</summary>
    [JsonProperty]
    public float MonsterDamageMultiplier { get; internal set; } = 1.5f;

    /// <summary>Gets a multiplier which allows scaling the resistance of all monsters.</summary>
    [JsonProperty]
    public float MonsterDefenseMultiplier { get; internal set; } = 1.5f;

    /// <summary>Gets a value indicating whether randomizes monster stats to add variability to monster encounters.</summary>
    [JsonProperty]
    public bool VariedEncounters { get; internal set; } = true;

    /// <summary>Gets a multiplier which allows increasing the spawn chance of monsters in dungeons.</summary>
    [JsonProperty]
    public float MonsterSpawnChanceMultiplier { get; internal set; } = 1f;

    #endregion enemies

    #region controls

    /// <summary>Gets a value indicating whether face the current cursor position before swinging your weapon.</summary>
    [JsonProperty]
    public bool FaceMouseCursor { get; internal set; } = true;

    /// <summary>Gets a value indicating whether to allow drifting in the movement direction when swinging weapons.</summary>
    [JsonProperty]
    public bool SlickMoves { get; internal set; } = true;

    /// <summary>Gets a value indicating whether to allow auto-selecting a weapon or slingshot.</summary>
    [JsonProperty]
    public bool EnableAutoSelection { get; internal set; } = true;

    /// <summary>Gets a value indicating how close an enemy must be to auto-select a weapon, in tiles.</summary>
    [JsonProperty]
    public uint MeleeAutoSelectionRange { get; internal set; } = 2;

    /// <summary>Gets a value indicating how close an enemy must be to auto-select a slingshot, in tiles.</summary>
    [JsonProperty]
    public uint RangedAutoSelectionRange { get; internal set; } = 4;

    /// <summary>Gets the chosen key(s) for toggling auto-selection.</summary>
    [JsonProperty]
    public KeybindList SelectionKey { get; internal set; } = KeybindList.Parse("LeftShift, LeftShoulder");

    /// <summary>Gets the <see cref="Color"/> used to indicate tools enabled or auto-selection.</summary>
    [JsonProperty]
    public Color SelectionBorderColor { get; internal set; } = Color.Aqua;

    #endregion controls

    #region interface

    /// <summary>Gets a value indicating whether to color-code tool names, <see href="https://tvtropes.org/pmwiki/pmwiki.php/Main/ColourCodedForYourConvenience"> for your convenience</see>.</summary>
    [JsonProperty]
    public bool ColorCodedForYourConvenience { get; internal set; } = true;

    /// <summary>Gets the <see cref="Color"/> used by common-tier weapons.</summary>
    [JsonProperty]
    public Color CommonTierColor { get; internal set; } = Game1.textColor;

    /// <summary>Gets the <see cref="Color"/> used by uncommon-tier weapons.</summary>
    [JsonProperty]
    public Color UncommonTierColor { get; internal set; } = Color.Green;

    /// <summary>Gets the <see cref="Color"/> used by rare-tier weapons.</summary>
    [JsonProperty]
    public Color RareTierColor { get; internal set; } = Color.Blue;

    /// <summary>Gets the <see cref="Color"/> used by common-tier weapons.</summary>
    [JsonProperty]
    public Color EpicTierColor { get; internal set; } = Color.Purple;

    /// <summary>Gets the <see cref="Color"/> used by common-tier weapons.</summary>
    [JsonProperty]
    public Color MythicTierColor { get; internal set; } = Color.Red;

    /// <summary>Gets the <see cref="Color"/> used by common-tier weapons.</summary>
    [JsonProperty]
    public Color MasterworkTierColor { get; internal set; } = Color.MonoGameOrange;

    /// <summary>Gets the <see cref="Color"/> used by common-tier weapons.</summary>
    [JsonProperty]
    public Color LegendaryTierColor { get; internal set; } = Color.MonoGameOrange;

    /// <summary>Gets a value indicating whether to replace generic Forge text with specific gemstone icons and empty sockets.</summary>
    [JsonProperty]
    public bool DrawForgeSockets { get; internal set; } = true;

    /// <summary>Gets the style of the sprite used to represent gemstone forges in tooltips.</summary>
    [JsonProperty]
    public ForgeSocketStyle SocketStyle { get; internal set; } = ForgeSocketStyle.Diamond;

    /// <summary>Gets the relative position where forge gemstones should be drawn.</summary>
    [JsonProperty]
    public ForgeSocketPosition SocketPosition { get; internal set; } = ForgeSocketPosition.AboveSeparator;

    /// <summary>Gets the style of the tooltips for displaying stat bonuses for weapons.</summary>
    [JsonProperty]
    public TooltipStyle WeaponTooltipStyle { get; internal set; } = TooltipStyle.Relative;

    /// <summary>Gets a value indicating whether to override the draw method to include the currently-equipped ammo.</summary>
    [JsonProperty]
    public bool DrawCurrentAmmo { get; internal set; } = true;

    /// <summary>Gets a value indicating whether to replace the mouse cursor with a bulls-eye while firing.</summary>
    [JsonProperty]
    public bool BullseyeReplacesCursor { get; internal set; } = true;

    #endregion interface
}
