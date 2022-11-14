namespace DaLion.Ligo.Modules.Professions.Events.Ultimate;

/// <summary>The arguments for an <see cref="UltimateChargeInitiatedEvent"/>.</summary>
internal sealed class UltimateChargeInitiatedEventArgs : EventArgs, IUltimateChargeInitiatedEventArgs
{
    /// <summary>Initializes a new instance of the <see cref="UltimateChargeInitiatedEventArgs"/> class.</summary>
    /// <param name="player">The player who triggered the event.</param>
    /// <param name="newValue">The new charge value.</param>
    internal UltimateChargeInitiatedEventArgs(Farmer player, double newValue)
    {
        this.Player = player;
        this.NewValue = newValue;
    }

    /// <inheritdoc />
    public Farmer Player { get; }

    /// <inheritdoc />
    public double NewValue { get; }
}
