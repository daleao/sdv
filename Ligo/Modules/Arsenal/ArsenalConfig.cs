namespace DaLion.Ligo.Modules.Arsenal;

#region using directives

using DaLion.Ligo.Modules.Arsenal.Configs;
using DaLion.Shared.Configs;
using Newtonsoft.Json;

#endregion using directives

/// <summary>The user-configurable settings for Arsenal.</summary>
public sealed class ArsenalConfig : Config
{
    /// <inheritdoc cref="SlingshotConfig"/>
    [JsonProperty]
    public SlingshotConfig Slingshots { get; internal set; } = new();

    /// <inheritdoc cref="WeaponConfig"/>
    [JsonProperty]
    public WeaponConfig Weapons { get; internal set; } = new();

    /// <summary>Gets a value indicating whether face the current cursor position before swinging your arsenal.</summary>
    [JsonProperty]
    public bool FaceMouseCursor { get; internal set; } = true;

    /// <summary>Gets a value indicating whether to allow drifting in the movement direction when using weapons.</summary>
    [JsonProperty]
    public bool SlickMoves { get; internal set; } = true;

    /// <summary>Gets a value indicating whether to color-code weapon and slingshot names, <see href="https://tvtropes.org/pmwiki/pmwiki.php/Main/ColourCodedForYourConvenience">for your convenience</see>.</summary>
    [JsonProperty]
    public bool ColorCodedForYourConvenience { get; internal set; } = true;

    /// <summary>Gets a value indicating whether to improve certain underwhelming gemstone enchantments.</summary>
    [JsonProperty]
    public bool RebalancedForges { get; internal set; } = true;

    /// <summary>Gets a value indicating whether to overhaul the knockback stat adding collision damage.</summary>
    [JsonProperty]
    public bool KnockbackDamage { get; internal set; } = true;

    /// <summary>Gets a value indicating whether to overhaul the defense stat with better scaling and other features.</summary>
    [JsonProperty]
    public bool OverhauledDefense { get; internal set; } = true;

    /// <summary>Gets increases the health of all monsters.</summary>
    [JsonProperty]
    public float MonsterHealthMultiplier { get; internal set; } = 2f;

    /// <summary>Gets increases the damage dealt by all monsters.</summary>
    [JsonProperty]
    public float MonsterDamageMultiplier { get; internal set; } = 1f;

    /// <summary>Gets increases the resistance of all monsters.</summary>
    [JsonProperty]
    public float MonsterDefenseMultiplier { get; internal set; } = 1.5f;

    /// <summary>Gets a value indicating whether randomizes monster stats to add variability to monster encounters.</summary>
    [JsonProperty]
    public bool VariedEncounters { get; internal set; } = true;

    /// <summary>Gets a value indicating whether replace the starting Rusty Sword with a Wooden Blade.</summary>
    [JsonProperty]
    public bool WoodyReplacesRusty { get; internal set; } = true;

    /// <summary>Gets a value indicating whether replace the starting Rusty Sword with a Wooden Blade.</summary>
    [JsonProperty]
    public bool DwarvishCrafting { get; internal set; } = true;

    /// <summary>Gets a value indicating whether replace lame Galaxy and Infinity weapons with something truly legendary.</summary>
    [JsonProperty]
    public bool InfinityPlusOne { get; internal set; } = true;

    /// <summary>Gets a value indicating the number of Iridium Bars required to obtain a Galaxy weapon.</summary>
    [JsonProperty]
    public int IridiumBarsRequiredForGalaxyArsenal { get; internal set; } = 10;

    /// <inheritdoc />
    internal override bool Validate()
    {
        var isValid = true;

        if (this.Weapons.GalaxySwordType == WeaponType.StabbingSword)
        {
            Collections.StabbySwords.Add(Constants.GalaxySwordIndex);
        }
        else if (this.Weapons.GalaxySwordType != WeaponType.DefenseSword)
        {
            Log.W(
                $"Invalid type {this.Weapons.GalaxySwordType} for Galaxy Sword. Should be either 'StabbingSword' or 'DefenseSword'. The value will default to 'DefenseSword'.");
            this.Weapons.GalaxySwordType = WeaponType.DefenseSword;
        }

        if (this.Weapons.InfinityBladeType == WeaponType.StabbingSword)
        {
            Collections.StabbySwords.Add(Constants.InfinityBladeIndex);
        }
        else if (this.Weapons.InfinityBladeType != WeaponType.DefenseSword)
        {
            Log.W(
                $"Invalid type {this.Weapons.InfinityBladeType} for Infinity Blade. Should be either 'StabbingSword' or 'DefenseSword'. The value will default to 'DefenseSword'.");
            this.Weapons.GalaxySwordType = WeaponType.DefenseSword;
        }

        return isValid;
    }
}
