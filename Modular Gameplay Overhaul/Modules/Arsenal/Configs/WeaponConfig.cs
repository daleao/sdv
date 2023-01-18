namespace DaLion.Overhaul.Modules.Arsenal.Configs;

#region using directives

using System.Collections.Generic;
using Newtonsoft.Json;

#endregion using directives

/// <summary>Configs related to <see cref="StardewValley.Tools.MeleeWeapon"/>s.</summary>
public sealed class WeaponConfig
{
    #region dropdown enums

    /// <summary>The style used to display stat bonuses in weapon tooltips.</summary>
    public enum TooltipStyle
    {
        /// <summary>Display the absolute value of the stat, minus it's default value for the weapon type.</summary>
        Absolute,

        /// <summary>Display the relative value of the stat, with respect to the default value for the weapon type.</summary>
        Relative,
    }

    #endregion dropdown enums

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

    /// <summary>Gets a value indicating whether to guarantee smash crit on Duggies and guarantee miss on gliders.</summary>
    [JsonProperty]
    public bool GroundedClubSmash { get; internal set; } = true;

    /// <summary>Gets a value indicating whether defense should improve parry damage.</summary>
    [JsonProperty]
    public bool DefenseImprovesParry { get; internal set; } = true;

    /// <summary>Gets a value indicating whether replace the defensive special move of some swords with an offensive lunge move.</summary>
    [JsonProperty]
    public bool EnableStabbySwords { get; internal set; } = true;

    /// <summary>Gets a value indicating the weapon type of the Galaxy Sword.</summary>
    [JsonProperty]
    public WeaponType GalaxySwordType { get; internal set; } = WeaponType.StabbingSword;

    /// <summary>Gets a value indicating the weapon type of the Infinity Blade.</summary>
    [JsonProperty]
    public WeaponType InfinityBladeType { get; internal set; } = WeaponType.StabbingSword;

    /// <summary>Gets a set of user-defined modded swords which should be treated as Stabby swords.</summary>
    [JsonProperty]
    public string[] CustomStabbingSwords { get; internal set; } =
    {
        "Strawblaster",
    };

    /// <summary>Gets a value indicating whether to apply the corresponding weapon rebalance.</summary>
    [JsonProperty]
    public bool EnableRebalance { get; internal set; } = true;

    /// <summary>Gets a value indicating whether to apply the corresponding weapon retexture.</summary>
    [JsonProperty]
    public bool EnableRetexture { get; internal set; } = true;

    /// <summary>Gets a value indicating whether enable new overhauled enchantments for melee weapons, and rebalance some old ones.</summary>
    [JsonProperty]
    public bool EnableEnchants { get; internal set; } = true;

    /// <summary>Gets the style of the tooltips for displaying stat bonuses for weapons.</summary>
    [JsonProperty]
    public TooltipStyle WeaponTooltipStyle { get; internal set; } = TooltipStyle.Absolute;

    /// <summary>Gets a value indicating how close an enemy must be to auto-select a weapon, in tiles.</summary>
    [JsonProperty]
    public uint AutoSelectionRange { get; internal set; } = 2;
}
