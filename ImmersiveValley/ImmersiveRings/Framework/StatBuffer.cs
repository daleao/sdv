namespace DaLion.Stardew.Rings.Framework;

#region using directives

using System.Linq;

#endregion using directives

/// <summary>A buffer for aggregating stat bonuses.</summary>
public sealed class StatBuffer
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="StatBuffer"/> class.Constructs an instance, initializing all
    ///     stat bonuses to zero.
    /// </summary>
    public StatBuffer()
    {
        this.DamageModifier = 0f;
        this.CritChanceModifier = 0f;
        this.CritPowerModifier = 0f;
        this.SwingSpeedModifier = 0f;
        this.KnockbackModifier = 0f;
        this.PrecisionModifier = 0f;
        this.CooldownReduction = 0f;
        this.AddedDefense = 0;
        this.AddedImmunity = 0;
        this.AddedMagneticRadius = 0;
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="StatBuffer"/> class.Constructs an instance, initializing all
    ///     stat bonuses to the specified values.
    /// </summary>
    /// <param name="damageModifier">The damage modifier.</param>
    /// <param name="critChanceModifier">The critical chance modifier.</param>
    /// <param name="critPowerModifier">The critical power modifier.</param>
    /// <param name="swingSpeedModifier">The swing speed modifier.</param>
    /// <param name="knockbackModifier">The knockback modifier.</param>
    /// <param name="precisionModifier">The precision modifier.</param>
    /// <param name="cooldownReduction">The cooldown reduction.</param>
    /// <param name="addedDefense">The added defense.</param>
    /// <param name="addedImmunity">The added immunity.</param>
    /// <param name="addedMagneticRadius">The added magnetic radius.</param>
    internal StatBuffer(
        float damageModifier,
        float critChanceModifier,
        float critPowerModifier,
        float swingSpeedModifier,
        float knockbackModifier,
        float precisionModifier,
        float cooldownReduction,
        int addedDefense,
        int addedImmunity,
        int addedMagneticRadius)
    {
        this.DamageModifier = damageModifier;
        this.CritChanceModifier = critChanceModifier;
        this.CritPowerModifier = critPowerModifier;
        this.SwingSpeedModifier = swingSpeedModifier;
        this.KnockbackModifier = knockbackModifier;
        this.PrecisionModifier = precisionModifier;
        this.CooldownReduction = cooldownReduction;
        this.AddedDefense = addedDefense;
        this.AddedImmunity = addedImmunity;
        this.AddedMagneticRadius = addedMagneticRadius;
    }

    /// <summary>Gets or sets the added defense.</summary>
    public int AddedDefense { get; set; }

    /// <summary>Gets or sets the added immunity.</summary>
    public int AddedImmunity { get; set; }

    /// <summary>Gets or sets the added magnetic radius.</summary>
    public int AddedMagneticRadius { get; set; }

    /// <summary>Gets or sets the cooldown reduction.</summary>
    public float CooldownReduction { get; set; }

    /// <summary>Gets or sets the critical chance modifier.</summary>
    public float CritChanceModifier { get; set; }

    /// <summary>Gets or sets the critical power modifier.</summary>
    public float CritPowerModifier { get; set; }

    /// <summary>Gets or sets the damage modifier.</summary>
    public float DamageModifier { get; set; }

    /// <summary>Gets or sets the knockback modifier.</summary>
    public float KnockbackModifier { get; set; }

    /// <summary>Gets or sets the precision modifier.</summary>
    public float PrecisionModifier { get; set; }

    /// <summary>Gets or sets the swing speed modifier.</summary>
    public float SwingSpeedModifier { get; set; }

    /// <summary>Determines whether any of the buffered stats is non-zero.</summary>
    /// <returns><see langword="true"/> if at least one of the buffered stats is greater than zero, otherwise <see langword="false"/>.</returns>
    public bool Any()
    {
        return this.DamageModifier != 0f || this.CritChanceModifier != 0f || this.CritPowerModifier != 0f ||
               this.SwingSpeedModifier != 0f || this.KnockbackModifier != 0f || this.PrecisionModifier != 0f ||
               this.CooldownReduction != 0f || this.AddedDefense != 0 || this.AddedImmunity != 0 ||
               this.AddedMagneticRadius != 0;
    }

    /// <summary>Gets the number of non-zero buffered stats.</summary>
    /// <returns>The number of non-zero buffered stats.</returns>
    public int Count()
    {
        return this.GetType().GetFields().Count(fi => (float)fi.GetValue(this)! > 0f);
    }
}
