namespace DaLion.Combat;

#region using directives

using DaLion.Shared.Enums;
using DaLion.Shared.Integrations.GMCM.Attributes;
using Newtonsoft.Json;
using StardewValley.Tools;

#endregion using directives

/// <summary>Config schema for the Combat mod.</summary>
public sealed class CombatConfig
{
    private float _monsterSpawnChanceMultiplier = 1f;
    private float _monsterHealthMultiplier = 1f;
    private float _monsterDamageMultiplier = 1f;
    private float _monsterDefenseMultiplier = 1f;
    private int _monsterHealthSummand = 0;
    private int _monsterDamageSummand = 0;
    private int _monsterDefenseSummand = 1;

    #region dropdown enums

    /// <summary>The texture that should be used as the resonance light source.</summary>
    public enum FaceCursorCondition
    {
        /// <summary>Never face the mouse cursor when using a tool.</summary>
        Never = -1,

        /// <summary>Face the mouse cursor when the current tool is a <see cref="MeleeWeapon"/> or <see cref="Slingshot"/>.</summary>
        WeaponOnly = 0,

        /// <summary>Face the mouse cursor when using any tool.</summary>
        Always = 1,
    }

    #endregion dropdown enums

    /// <summary>Gets a value indicating whether to replace vanilla weapon spam with a more strategic combo system.</summary>
    [JsonProperty]
    [GMCMSection("cmbt.combo")]
    [GMCMPriority(1)]
    public bool EnableComboHits { get; internal set; } = true;

    /// <summary>Gets the number of hits in each weapon type's combo.</summary>
    [JsonProperty]
    [GMCMSection("cmbt.combo")]
    [GMCMPriority(2)]
    [GMCMRange(0, 10)]
    public Dictionary<string, int> ComboHitsPerWeaponType { get; internal set; } = new()
    {
        { WeaponType.DefenseSword.ToString(), 4 },
        { WeaponType.Club.ToString(), 2 },
    };

    /// <summary>Gets a value indicating whether to keep swiping while the "use tool" key is held.</summary>
    [JsonProperty]
    [GMCMSection("cmbt.combo")]
    [GMCMPriority(3)]
    public bool SwipeHold { get; internal set; } = true;

    /// <summary>Gets a value indicating whether to replace the linear damage mitigation formula with a rational formula that gives more impactful but diminishing returns from the defense stat.</summary>
    [JsonProperty]
    [GMCMSection("cmbt.stats")]
    [GMCMPriority(100)]
    public bool HyperbolicMitigationFormula { get; internal set; } = true;

    /// <summary>Gets a value indicating whether defense should improve parry damage.</summary>
    [JsonProperty]
    [GMCMSection("cmbt.stats")]
    [GMCMPriority(101)]
    public bool DefenseImprovesParry { get; internal set; } = true;

    /// <summary>Gets a value indicating whether critical strikes should ignore the target's defense.</summary>
    [JsonProperty]
    [GMCMSection("cmbt.stats")]
    [GMCMPriority(102)]
    public bool CritsIgnoreDefense { get; internal set; } = true;

    /// <summary>Gets a value indicating whether back attacks gain double crit. chance.</summary>
    [JsonProperty]
    [GMCMSection("cmbt.stats")]
    [GMCMPriority(103)]
    public bool CriticalBackAttacks { get; internal set; } = true;

    /// <summary>Gets a value indicating whether to add collision damage when knocked-back enemies hit a wall or obstacle.</summary>
    [JsonProperty]
    [GMCMSection("cmbt.stats")]
    [GMCMPriority(104)]
    public bool KnockbackHurts { get; internal set; } = true;

    /// <summary>Gets a value indicating whether to allow drifting in the movement direction when swinging weapons.</summary>
    [JsonProperty]
    [GMCMSection("cmbt.controls")]
    [GMCMPriority(500)]
    public bool SlickMoves { get; internal set; } = true;

    /// <summary>Gets a value indicating whether face the current cursor position before using a tool.</summary>
    [JsonProperty]
    [GMCMSection("cmbt.controls")]
    [GMCMPriority(501)]
    public FaceCursorCondition FaceMouseCursor { get; internal set; } = FaceCursorCondition.WeaponOnly;

    /// <summary>Gets a value indicating whether randomizes monster stats to add variability to monster encounters.</summary>
    [JsonProperty]
    [GMCMSection("cmbt.enemies")]
    [GMCMPriority(1000)]
    public bool VariedEncounters { get; internal set; } = false;

    /// <summary>Gets a multiplier which allows increasing the spawn chance of monsters in dungeons.</summary>
    [JsonProperty]
    [GMCMSection("cmbt.enemies")]
    [GMCMPriority(1001)]
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
    [GMCMPriority(1002)]
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
    [GMCMPriority(1003)]
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
    [GMCMPriority(1004)]
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
    [GMCMPriority(1005)]
    [GMCMRange(-100, 100, 10)]
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
    [GMCMPriority(1006)]
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
    [GMCMPriority(1007)]
    [GMCMRange(-10, 10)]
    public int MonsterDefenseSummand
    {
        get => this._monsterDefenseSummand;
        internal set
        {
            this._monsterDefenseSummand = Math.Max(value, -10);
        }
    }
}
