namespace DaLion.Ligo.Modules.Arsenal.Configs;

#region using directives

using System.Collections.Generic;
using Newtonsoft.Json;

#endregion using directives

/// <summary>Configs related to <see cref="StardewValley.Tools.MeleeWeapon"/>s.</summary>
public sealed class WeaponConfig
{
    /// <summary>Gets a value indicating whether to replace vanilla weapon spam with a more strategic combo system.</summary>
    [JsonProperty]
    public bool AllowComboHits { get; internal set; } = true;

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
    public bool BringBackStabbySwords { get; internal set; } = true;

    /// <summary>Gets a value indicating whether replace the defensive special move of some swords with an offensive lunge move.</summary>
    [JsonProperty]
    public WeaponType GalaxySwordType { get; internal set; } = WeaponType.StabbingSword;

    /// <summary>Gets a value indicating whether replace the defensive special move of some swords with an offensive lunge move.</summary>
    [JsonProperty]
    public WeaponType InfinityBladeType { get; internal set; } = WeaponType.StabbingSword;

    /// <summary>Gets a value indicating whether to apply the corresponding weapon rebalance.</summary>
    [JsonProperty]
    public bool RebalancedWeapons { get; internal set; } = true;

    /// <summary>Gets a value indicating whether to apply the corresponding weapon retexture.</summary>
    [JsonProperty]
    public bool RetexturedWeapons { get; internal set; } = true;

    /// <summary>Gets a value indicating whether enable new overhauled enchantments for melee weapons, and rebalance some old ones.</summary>
    [JsonProperty]
    public bool UseLigoEnchants { get; internal set; } = true;
}
