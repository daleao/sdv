namespace DaLion.Redux.Arsenal.Weapons;

#region using directives

using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

#endregion using directives

/// <summary>Configs related to <see cref="StardewValley.Tools.MeleeWeapon"/>s.</summary>
public sealed class Config
{
    /// <summary>Gets a value indicating whether replace the starting Rusty Sword with a Wooden Blade.</summary>
    [JsonProperty]
    public bool WoodyReplacesRusty { get; internal set; } = true;

    /// <summary>Gets a value indicating whether replace the defensive special move of some swords with an offensive lunge move.</summary>
    [JsonProperty]
    public bool BringBackStabbySwords { get; internal set; } = true;

    /// <summary>Gets the swords that should be converted to stabby swords.</summary>
    [JsonProperty]
    public HashSet<string> StabbySwords { get; internal set; } = new[]
    {
        "Steel Smallsword",
        "Cutlass",
        "Rapier",
        "Steel Falchion",
        "Pirate's Sword",
        "Forest Sword",
        "Insect Head",
        "Obsidian Edge",
        "Lava Katana",
        "Galaxy Sword",
        "Tempered Galaxy Sword",
        "Dragontooth Cutlass",
        "Infinity Blade",
    }.ToHashSet();

    /// <summary>Gets a value indicating whether to replace vanilla weapon spam with a more strategic combo system.</summary>
    [JsonProperty]
    public bool ComboHits { get; internal set; } = true;

    /// <summary>Gets a value indicating whether guaranteed smash crit on Duggies. Guaranteed smash miss on flying enemies.</summary>
    [JsonProperty]
    public bool ImmersiveClubSmash { get; internal set; } = true;

    /// <summary>Gets a value indicating whether replace lame Galaxy and Infinity weapons with something truly legendary.</summary>
    [JsonProperty]
    public bool InfinityPlusOneWeapons { get; internal set; } = true;

    /// <summary>Gets a value indicating whether enable new overhauled enchantments for melee weapons, and rebalance some old ones.</summary>
    [JsonProperty]
    public bool OverhauledEnchants { get; internal set; } = true;
}
