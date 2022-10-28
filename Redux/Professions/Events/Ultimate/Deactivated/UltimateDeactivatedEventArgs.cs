namespace DaLion.Redux.Professions.Events.Ultimate;

/// <summary>The arguments for an <see cref="UltimateDeactivatedEvent"/>.</summary>
public sealed class UltimateDeactivatedEventArgs : EventArgs, IUltimateDeactivatedEventArgs
{
    /// <summary>Initializes a new instance of the <see cref="UltimateDeactivatedEventArgs"/> class.</summary>
    /// <param name="player">The player who triggered the event.</param>
    internal UltimateDeactivatedEventArgs(Farmer player)
    {
        this.Player = player;
    }

    /// <inheritdoc />
    public Farmer Player { get; }
}
