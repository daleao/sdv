namespace DaLion.Overhaul.Modules.Combat.Configs;

#region using directives

using System.Collections.Generic;
using DaLion.Overhaul.Modules.Combat.Enums;
using DaLion.Overhaul.Modules.Core.ConfigMenu;
using DaLion.Shared.Enums;
using DaLion.Shared.Extensions.SMAPI;
using DaLion.Shared.Integrations.GMCM.Attributes;
using Newtonsoft.Json;

#endregion using directives

/// <summary>The user-configurable settings for CMBT.</summary>
public sealed class WeaponsSlingshotsConfig
{
    private bool _enableOverhaul = true;
    private bool _enableStabbingSwords = true;
    private bool _enableComboHits = true;

    /// <summary>Gets a value indicating whether to apply all features relating to the weapon and slingshot re-balance, including weapon tiers, shops, Mine chests and monster drops.</summary>
    [JsonProperty]
    [GMCMPriority(100)]
    public bool EnableOverhaul
    {
        get => this._enableOverhaul;
        internal set
        {
            if (value == this._enableOverhaul)
            {
                return;
            }

            if (!value)
            {
                this.EnableStabbingSwords = false;
                this.EnableComboHits = false;
            }

            this._enableOverhaul = value;
            ModHelper.GameContent.InvalidateCacheAndLocalized("Data/weapons");
            if (Context.IsWorldReady)
            {
                CombatModule.RefreshAllWeapons(value
                    ? WeaponRefreshOption.Initial
                    : WeaponRefreshOption.FromData);
            }
        }
    }

    /// <summary>Gets a value indicating whether replace the defensive special move of some swords with an offensive lunge move.</summary>
    [JsonProperty]
    [GMCMSection("cmbt.melee")]
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

            if (value && !this.EnableOverhaul)
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
    [GMCMSection("cmbt.melee")]
    [GMCMPriority(122)]
    public bool EnableComboHits
    {
        get => this._enableComboHits;
        internal set
        {
            if (value && !this.EnableOverhaul)
            {
                Log.W("Melee Combo Framework requires that Weapon Overhaul be set to true.");
                return;
            }

            this._enableComboHits = value;
        }
    }

    /// <summary>Gets the number of hits in each weapon type's combo.</summary>
    [JsonProperty]
    [GMCMSection("cmbt.melee")]
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
    [GMCMSection("cmbt.melee")]
    [GMCMPriority(124)]
    public bool SwipeHold { get; internal set; } = true;

    /// <summary>Gets a value indicating whether defense should improve parry damage.</summary>
    [JsonProperty]
    [GMCMSection("cmbt.melee")]
    [GMCMPriority(125)]
    public bool DefenseImprovesParry { get; internal set; } = true;

    /// <summary>Gets a value indicating whether to guarantee smash crit on Duggies and guarantee miss on gliders.</summary>
    [JsonProperty]
    [GMCMSection("cmbt.melee")]
    [GMCMPriority(126)]
    public bool GroundedClubSmash { get; internal set; } = true;

    /// <summary>Gets a value indicating whether to allow slingshots to deal critical damage and be affected by critical modifiers.</summary>
    [JsonProperty]
    [GMCMSection("cmbt.ranged")]
    [GMCMPriority(140)]
    public bool EnableRangedCriticalHits { get; internal set; } = true;

    /// <summary>Gets a value indicating whether to enable the custom slingshot stun smack special move.</summary>
    [JsonProperty]
    [GMCMSection("cmbt.ranged")]
    [GMCMPriority(141)]
    public bool EnableSlingshotSpecialMove { get; internal set; } = true;

    /// <summary>Gets a value indicating whether to allow forging the Infinity Slingshot.</summary>
    [JsonProperty]
    [GMCMSection("cmbt.ranged")]
    [GMCMPriority(142)]
    public bool EnableInfinitySlingshot { get; internal set; } = true;

    /// <summary>Gets a value indicating whether projectiles should not be useless for the first 100ms.</summary>
    [JsonProperty]
    [GMCMSection("cmbt.ranged")]
    [GMCMPriority(143)]
    public bool RemoveSlingshotGracePeriod { get; internal set; } = true;
}
