namespace DaLion.Professions.Framework.Events.Limit.ChargeIncreased;

/// <summary>The arguments for an <see cref="LimitChargeIncreasedEvent"/>.</summary>
public sealed class LimitChargeIncreasedEventArgs : EventArgs, ILimitChargeIncreasedEventArgs
{
    /// <summary>Initializes a new instance of the <see cref="LimitChargeIncreasedEventArgs"/> class.</summary>
    /// <param name="player">The player who triggered the event.</param>
    /// <param name="oldValue">The old charge value.</param>
    /// <param name="newValue">The new charge value.</param>
    internal LimitChargeIncreasedEventArgs(Farmer player, double oldValue, double newValue)
    {
        this.Player = player;
        this.OldValue = oldValue;
        this.NewValue = newValue;
    }

    /// <inheritdoc />
    public Farmer Player { get; }

    /// <inheritdoc />
    public double OldValue { get; }

    /// <inheritdoc />
    public double NewValue { get; }
}
