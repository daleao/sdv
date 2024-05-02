namespace DaLion.Professions.Framework.Events.Limit.ChargeInitiated;

/// <summary>The arguments for an <see cref="LimitChargeInitiatedEvent"/>.</summary>
internal sealed class LimitChargeInitiatedEventArgs : EventArgs, ILimitChargeInitiatedEventArgs
{
    /// <summary>Initializes a new instance of the <see cref="LimitChargeInitiatedEventArgs"/> class.</summary>
    /// <param name="player">The player who triggered the event.</param>
    /// <param name="newValue">The new charge value.</param>
    internal LimitChargeInitiatedEventArgs(Farmer player, double newValue)
    {
        this.Player = player;
        this.NewValue = newValue;
    }

    /// <inheritdoc />
    public Farmer Player { get; }

    /// <inheritdoc />
    public double NewValue { get; }
}
