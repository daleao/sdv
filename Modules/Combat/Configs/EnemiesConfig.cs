namespace DaLion.Overhaul.Modules.Combat.Configs;

#region using directives

using DaLion.Shared.Integrations.GMCM.Attributes;
using Newtonsoft.Json;

#endregion using directives

/// <summary>The user-configurable settings for CMBT.</summary>
public sealed class EnemiesConfig
{
    private float _monsterSpawnChanceMultiplier = 1f;
    private float _monsterHealthMultiplier = 1f;
    private float _monsterDamageMultiplier = 1f;
    private float _monsterDefenseMultiplier = 1f;
    private int _monsterHealthSummand = 0;
    private int _monsterDamageSummand = 0;
    private int _monsterDefenseSummand = 1;

    /// <summary>Gets a value indicating whether randomizes monster stats to add variability to monster encounters.</summary>
    [JsonProperty]
    [GMCMPriority(400)]
    public bool VariedEncounters { get; internal set; } = true;

    /// <summary>Gets a multiplier which allows increasing the spawn chance of monsters in dungeons.</summary>
    [JsonProperty]
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
}
