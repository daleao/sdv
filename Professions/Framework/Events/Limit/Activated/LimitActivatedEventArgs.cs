namespace DaLion.Professions.Framework.Events.Limit.Activated;

/// <summary>The arguments for an <see cref="LimitActivatedEvent"/>.</summary>
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
