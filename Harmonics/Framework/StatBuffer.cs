namespace DaLion.Harmonics.Framework;

#region using directives

using System.Linq;
using DaLion.Shared.Extensions;

#endregion using directives

/// <summary>A buffer for aggregating stat bonuses.</summary>
public sealed class StatBuffer
{
    private readonly float[] _stats = new float[8];

    /// <summary>Initializes a new instance of the <see cref="StatBuffer"/> class with all stats set to zero.</summary>
    public StatBuffer()
    {
    }

    /// <summary>Initializes a new instance of the <see cref="StatBuffer"/> class with all stats set to the specified array values.</summary>
    /// <param name="stats">An array with exactly 9 float values corresponding to each stat bonus.</param>
    public StatBuffer(float[] stats)
    {
        if (stats.Length != 8)
        {
            ThrowHelper.ThrowInvalidOperationException();
        }

        this._stats = stats;
    }

    /// <summary>Initializes a new instance of the <see cref="StatBuffer"/> class with all stats set to the specified individual values.</summary>
    /// <param name="attackMultiplier">The attack multiplier.</param>
    /// <param name="cooldownReduction">The cooldown reduction.</param>
    /// <param name="critChanceMultiplier">The critical chance multiplier.</param>
    /// <param name="critPowerMultiplier">The critical power multiplier.</param>
    /// <param name="defense">The added defense.</param>
    /// <param name="knockbackMultiplier">The knockback multiplier.</param>
    /// <param name="magneticRadius">The added magnetic radius.</param>
    /// <param name="weaponSpeedMultiplier">The weapon speed multiplier.</param>
    internal StatBuffer(
        float attackMultiplier,
        float cooldownReduction,
        float critChanceMultiplier,
        float critPowerMultiplier,
        float defense,
        float knockbackMultiplier,
        float magneticRadius,
        float weaponSpeedMultiplier)
    {
        this._stats[0] = attackMultiplier;
        this._stats[1] = critChanceMultiplier;
        this._stats[2] = critPowerMultiplier;
        this._stats[3] = cooldownReduction;
        this._stats[4] = defense;
        this._stats[5] = knockbackMultiplier;
        this._stats[6] = magneticRadius;
        this._stats[7] = weaponSpeedMultiplier;
    }

    /// <summary>Gets or sets the attack multiplier.</summary>
    public float AttackMultiplier { get => this._stats[0]; set => this._stats[0] = value; }

    /// <summary>Gets or sets the cooldown reduction.</summary>
    public float CooldownReduction { get => this._stats[1]; set => this._stats[1] = value; }

    /// <summary>Gets or sets the critical chance multiplier.</summary>
    public float CriticalChanceMultiplier { get => this._stats[2]; set => this._stats[2] = value; }

    /// <summary>Gets or sets the critical power multiplier.</summary>
    public float CriticalPowerMultiplier { get => this._stats[3]; set => this._stats[3] = value; }

    /// <summary>Gets or sets the added defense.</summary>
    public float Defense { get => this._stats[4]; set => this._stats[4] = value; }

    /// <summary>Gets or sets the knockback multiplier.</summary>
    public float KnockbackMultiplier { get => this._stats[5]; set => this._stats[5] = value; }

    /// <summary>Gets or sets the added magnetic radius.</summary>
    public float MagneticRadius { get => (int)this._stats[6]; set => this._stats[6] = value; }

    /// <summary>Gets or sets the weapon speed multiplier.</summary>
    public float WeaponSpeedMultiplier { get => this._stats[7]; set => this._stats[7] = value; }

    /// <summary>Adds two <see cref="StatBuffer"/>s.</summary>
    /// <param name="left">Source <see cref="StatBuffer" /> on the left of the add sign.</param>
    /// <param name="right">Source <see cref="StatBuffer" /> on the right of the add sign.</param>
    /// <returns>Sum of the vectors.</returns>
    public static StatBuffer operator +(StatBuffer left, StatBuffer right)
    {
        return new StatBuffer(left._stats.ElementwiseAdd(right._stats));
    }

    /// <summary>Compares two <see cref="StatBuffer"/>s.</summary>
    /// <param name="left">Source <see cref="StatBuffer" /> on the left of the add sign.</param>
    /// <param name="right">Source <see cref="StatBuffer" /> on the right of the add sign.</param>
    /// <returns>Sum of the vectors.</returns>
    public static bool operator ==(StatBuffer left, StatBuffer right)
    {
        return left._stats == right._stats;
    }

    /// <summary>Compares two <see cref="StatBuffer"/>s.</summary>
    /// <param name="left">Source <see cref="StatBuffer" /> on the left of the add sign.</param>
    /// <param name="right">Source <see cref="StatBuffer" /> on the right of the add sign.</param>
    /// <returns>Sum of the vectors.</returns>
    public static bool operator !=(StatBuffer left, StatBuffer right)
    {
        return !(left == right);
    }

    /// <inheritdoc />
    public override bool Equals(object? @object)
    {
        return @object is StatBuffer buffer && this == buffer;
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return this._stats.GetHashCode();
    }

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
