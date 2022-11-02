namespace DaLion.Redux.Framework.Rings;

#region using directives

using System.Linq;

#endregion using directives

/// <summary>A buffer for aggregating stat bonuses.</summary>
public sealed class StatBuffer
{
    private readonly float[] _stats = new float[10];

    /// <summary>
    ///     Initializes a new instance of the <see cref="StatBuffer"/> class.Constructs an instance, initializing all
    ///     stat bonuses to zero.
    /// </summary>
    public StatBuffer()
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="StatBuffer"/> class.Constructs an instance, initializing all
    ///     stat bonuses to the specified values.
    /// </summary>
    /// <param name="damageModifier">The damage modifier.</param>
    /// <param name="critChanceModifier">The critical chance modifier.</param>
    /// <param name="critPowerModifier">The critical power modifier.</param>
    /// <param name="knockbackModifier">The knockback modifier.</param>
    /// <param name="precisionModifier">The precision modifier.</param>
    /// <param name="swingSpeedModifier">The swing speed modifier.</param>
    /// <param name="cooldownReduction">The cooldown reduction.</param>
    /// <param name="defenseModifier">The added defense.</param>
    /// <param name="addedMagneticRadius">The added magnetic radius.</param>
    internal StatBuffer(
        float damageModifier,
        float critChanceModifier,
        float critPowerModifier,
        float precisionModifier,
        float knockbackModifier,
        float swingSpeedModifier,
        float cooldownReduction,
        float defenseModifier,
        int addedMagneticRadius)
    {
        this._stats[0] = damageModifier;
        this._stats[1] = critChanceModifier;
        this._stats[2] = critPowerModifier;
        this._stats[3] = swingSpeedModifier;
        this._stats[4] = knockbackModifier;
        this._stats[5] = precisionModifier;
        this._stats[6] = cooldownReduction;
        this._stats[7] = defenseModifier;
        this._stats[8] = addedMagneticRadius;
    }

    /// <summary>Gets or sets the added magnetic radius.</summary>
    public int MagneticRadius { get => (int)this._stats[8]; set => this._stats[8] = value; }

    /// <summary>Gets or sets the cooldown reduction.</summary>
    public float CooldownReduction { get => this._stats[6]; set => this._stats[6] = value; }

    /// <summary>Gets or sets the critical chance modifier.</summary>
    public float CritChanceModifier { get => this._stats[1]; set => this._stats[1] = value; }

    /// <summary>Gets or sets the critical power modifier.</summary>
    public float CritPowerModifier { get => this._stats[2]; set => this._stats[2] = value; }

    /// <summary>Gets or sets the damage modifier.</summary>
    public float DamageModifier { get => this._stats[0]; set => this._stats[0] = value; }

    /// <summary>Gets or sets the added defense.</summary>
    public float DefenseModifier { get => (int)this._stats[7]; set => this._stats[7] = value; }

    /// <summary>Gets or sets the knockback modifier.</summary>
    public float KnockbackModifier { get => this._stats[4]; set => this._stats[4] = value; }

    /// <summary>Gets or sets the precision modifier.</summary>
    public float PrecisionModifier { get => this._stats[5]; set => this._stats[5] = value; }

    /// <summary>Gets or sets the swing speed modifier.</summary>
    public float SwingSpeedModifier { get => this._stats[3]; set => this._stats[3] = value; }

    /// <summary>Determines whether any of the buffered stats is non-zero.</summary>
    /// <returns><see langword="true"/> if at least one of the buffered stats is greater than zero, otherwise <see langword="false"/>.</returns>
    public bool Any()
    {
        return this._stats.Any(s => s > 0f);
    }

    /// <summary>Gets the number of non-zero buffered stats.</summary>
    /// <returns>The number of non-zero buffered stats.</returns>
    public int Count()
    {
        return this._stats.Count(s => s > 0f);
    }
}
