namespace DaLion.Professions.Framework.Limits.Events;

/// <summary>The arguments for a <see cref="LimitChargeChangedEvent"/>.</summary>
public sealed class LimitChargeChangedEventArgs : EventArgs, ILimitChargeChangedEventArgs
{
    /// <summary>Initializes a new instance of the <see cref="LimitChargeChangedEventArgs"/> class.</summary>
    /// <param name="player">The player who triggered the event.</param>
    /// <param name="oldValue">The old charge value.</param>
    /// <param name="newValue">The new charge value.</param>
    internal LimitChargeChangedEventArgs(Farmer player, double oldValue, double newValue)
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
