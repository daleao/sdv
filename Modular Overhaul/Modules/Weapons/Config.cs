namespace DaLion.Overhaul.Modules.Weapons;

#region using directives

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using StardewModdingAPI.Utilities;

#endregion using directives

/// <summary>The user-configurable settings for WPNZ.</summary>
public sealed class Config : Shared.Configs.Config
{
    #region dropdown enums

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
    public enum TrialDifficulty
    {
        /// <summary>Easy.</summary>
        Easy,

        /// <summary>Medium.</summary>
        Medium,

        /// <summary>Hard.</summary>
        Hard,
    }

    #endregion dropdown enums

    /// <summary>Gets a value indicating whether to apply all features relating to the weapon re-balance, including weapon tiers, shops, Mine chests and drops.</summary>
    [JsonProperty]
    public bool EnableRebalance { get; internal set; } = true;

    /// <summary>Gets the style of the tooltips for displaying stat bonuses for weapons.</summary>
    [JsonProperty]
    public TooltipStyle WeaponTooltipStyle { get; internal set; } = TooltipStyle.Relative;

    /// <summary>Gets a value indicating whether to color-code tool names, <see href="https://tvtropes.org/pmwiki/pmwiki.php/Main/ColourCodedForYourConvenience"> for your convenience</see>.</summary>
    [JsonProperty]
    public bool ColorCodedForYourConvenience { get; internal set; } = true;

    /// <summary>Gets a value indicating whether to replace vanilla weapon spam with a more strategic combo system.</summary>
    [JsonProperty]
    public bool EnableComboHits { get; internal set; } = true;

    /// <summary>Gets a value indicating whether to keep swiping while the "use tool" key is held.</summary>
    [JsonProperty]
    public bool SwipeHold { get; internal set; } = true;

    /// <summary>Gets the number of hits in each weapon type's combo.</summary>
    [JsonProperty]
    public Dictionary<WeaponType, int> ComboHitsPerWeapon { get; internal set; } = new()
    {
        { WeaponType.StabbingSword, 4 }, { WeaponType.DefenseSword, 4 }, { WeaponType.Club, 2 },
    };

    /// <summary>Gets a value indicating whether replace the defensive special move of some swords with an offensive lunge move.</summary>
    [JsonProperty]
    public bool EnableStabbySwords { get; internal set; } = true;

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

    /// <summary>Gets a value indicating whether to apply the corresponding weapon retexture.</summary>
    [JsonProperty]
    public bool EnableRetexture { get; internal set; } = true;

    /// <summary>Gets a value indicating whether to enable Clint's forging mechanic for Masterwork weapons.</summary>
    [JsonProperty]
    public bool DwarvenLegacy { get; internal set; } = true;

    /// <summary>Gets a value indicating whether replace lame Galaxy and Infinity weapons with something truly legendary.</summary>
    [JsonProperty]
    public bool InfinityPlusOne { get; internal set; } = true;

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
    public TrialDifficulty VirtueTrialDifficulty { get; internal set; } = TrialDifficulty.Medium;

    /// <summary>Gets a value indicating whether defense should improve parry damage.</summary>
    [JsonProperty]
    public bool DefenseImprovesParry { get; internal set; } = true;

    /// <summary>Gets a value indicating whether to guarantee smash crit on Duggies and guarantee miss on gliders.</summary>
    [JsonProperty]
    public bool GroundedClubSmash { get; internal set; } = true;

    /// <summary>Gets a value indicating whether replace the starting Rusty Sword with a Wooden Blade.</summary>
    [JsonProperty]
    public bool WoodyReplacesRusty { get; internal set; } = true;

    /// <summary>Gets a value indicating whether face the current cursor position before swinging your weapon.</summary>
    [JsonProperty]
    public bool FaceMouseCursor { get; internal set; } = true;

    /// <summary>Gets a value indicating whether to allow drifting in the movement direction when swinging weapons.</summary>
    [JsonProperty]
    public bool SlickMoves { get; internal set; } = true;

    /// <summary>Gets a value indicating whether to allow auto-selecting a weapon or slingshot.</summary>
    [JsonProperty]
    public bool EnableAutoSelection { get; internal set; } = true;

    /// <summary>Gets the chosen mod key(s).</summary>
    [JsonProperty]
    public KeybindList SelectionKey { get; internal set; } = KeybindList.Parse("LeftShift, LeftShoulder");

    /// <summary>Gets the <see cref="Color"/> used to indicate tools enabled or auto-selection.</summary>
    [JsonProperty]
    public Color SelectionBorderColor { get; internal set; } = Color.Magenta;

    /// <summary>Gets a value indicating how close an enemy must be to auto-select a weapon, in tiles.</summary>
    [JsonProperty]
    public uint AutoSelectionRange { get; internal set; } = 2;
}
