namespace DaLion.Redux.Arsenal;

#region using directives

using Newtonsoft.Json;

#endregion using directives

/// <summary>The user-configurable settings for Arsenal.</summary>
public sealed class Config
{
    /// <inheritdoc cref="Arsenal.Slingshots.Config"/>
    [JsonProperty]
    public Slingshots.Config Slingshots { get; internal set; } = new();

    /// <inheritdoc cref="Arsenal.Weapons.Config"/>
    [JsonProperty]
    public Weapons.Config Weapons { get; internal set; } = new();

    /// <summary>Gets a value indicating whether face the current cursor position before swinging your arsenal.</summary>
    [JsonProperty]
    public bool FaceMouseCursor { get; internal set; } = true;

    /// <summary>Gets a value indicating whether to allow drifting in the movement direction when using weapons.</summary>
    [JsonProperty]
    public bool SlickMoves { get; internal set; } = true;

    /// <summary>Gets a value indicating whether to improve certain underwhelming gemstone enchantments.</summary>
    [JsonProperty]
    public bool RebalancedForges { get; internal set; } = true;

    /// <summary>Gets a value indicating whether to overhaul the defense stat with better scaling and other features.</summary>
    [JsonProperty]
    public bool OverhauledDefense { get; internal set; } = true;

    /// <summary>Gets a value indicating whether to overhaul the knockback stat.</summary>
    [JsonProperty]
    public bool OverhauledKnockback { get; internal set; } = true;

    /// <summary>Gets increases the health of all monsters.</summary>
    [JsonProperty]
    public float MonsterHealthMultiplier { get; internal set; } = 1.5f;

    /// <summary>Gets increases the damage dealt by all monsters.</summary>
    [JsonProperty]
    public float MonsterDamageMultiplier { get; internal set; } = 1f;

    /// <summary>Gets increases the resistance of all monsters.</summary>
    [JsonProperty]
    public float MonsterDefenseMultiplier { get; internal set; } = 1f;

    /// <summary>Gets a value indicating whether randomizes monster stats to add variability to monster encounters.</summary>
    [JsonProperty]
    public bool VariedEncounters { get; internal set; } = true;
}
