namespace DaLion.Professions.Framework.Limits.Events;

/// <summary>The arguments for a <see cref="LimitDeactivatedEvent"/>.</summary>
public sealed class LimitDeactivatedEventArgs : EventArgs, ILimitDeactivatedEventArgs
{
    /// <summary>Initializes a new instance of the <see cref="LimitDeactivatedEventArgs"/> class.</summary>
    /// <param name="player">The player who triggered the event.</param>
    internal LimitDeactivatedEventArgs(Farmer player)
    {
        this.Player = player;
    }

    /// <inheritdoc />
    public Farmer Player { get; }
}
