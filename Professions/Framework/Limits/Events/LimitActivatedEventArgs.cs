namespace DaLion.Professions.Framework.Limits.Events;

/// <summary>The arguments for a <see cref="LimitActivatedEvent"/>.</summary>
public sealed class LimitActivatedEventArgs : EventArgs, ILimitActivatedEventArgs
{
    /// <summary>Initializes a new instance of the <see cref="LimitActivatedEventArgs"/> class.</summary>
    /// <param name="player">The player who triggered the event.</param>
    internal LimitActivatedEventArgs(Farmer player)
    {
        this.Player = player;
    }

    /// <inheritdoc />
    public Farmer Player { get; }
}
