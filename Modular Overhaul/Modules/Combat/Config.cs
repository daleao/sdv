namespace DaLion.Overhaul.Modules.Combat;

#region using directives

using Newtonsoft.Json;

#endregion using directives

/// <summary>The user-configurable settings for CMBT.</summary>
public sealed class Config : Shared.Configs.Config
{
    /// <summary>Gets a value indicating whether to enable status conditions like Bleed and Stun on enemies.</summary>
    [JsonProperty]
    public bool EnableStatusConditions { get; internal set; } = true;

    /// <summary>Gets a value indicating whether to overhaul the knockback stat adding collision damage.</summary>
    [JsonProperty]
    public bool KnockbackDamage { get; internal set; } = true;

    /// <summary>Gets a value indicating whether to replace the Shooter projectile texture.</summary>
    [JsonProperty]
    public bool ShadowyShooterProjectile { get; internal set; } = true;

    /// <summary>Gets a value indicating whether to overhaul the defense stat with better scaling and other features.</summary>
    [JsonProperty]
    public bool OverhauledDefense { get; internal set; } = true;

    /// <summary>Gets a value indicating whether back attacks gain double crit. chance.</summary>
    [JsonProperty]
    public bool CriticalBackAttacks { get; internal set; } = true;

    /// <summary>Gets a multiplier which allows scaling the health of all monsters.</summary>
    [JsonProperty]
    public float MonsterHealthMultiplier { get; internal set; } = 1f;

    /// <summary>Gets a multiplier which allows scaling the damage dealt by all monsters.</summary>
    [JsonProperty]
    public float MonsterDamageMultiplier { get; internal set; } = 1f;

    /// <summary>Gets a multiplier which allows scaling the resistance of all monsters.</summary>
    [JsonProperty]
    public float MonsterDefenseMultiplier { get; internal set; } = 1f;

    /// <summary>Gets a value indicating whether randomizes monster stats to add variability to monster encounters.</summary>
    [JsonProperty]
    public bool VariedEncounters { get; internal set; } = true;
}
