namespace DaLion.Professions.Framework.Events.Limit.FullyCharged;

/// <summary>The arguments for an <see cref="LimitFullyChargedEvent"/>.</summary>
public sealed class LimitFullyChargedEventArgs : EventArgs, ILimitFullyChargedEventArgs
{
    /// <summary>Initializes a new instance of the <see cref="LimitFullyChargedEventArgs"/> class.</summary>
    /// <param name="player">The player who triggered the event.</param>
    internal LimitFullyChargedEventArgs(Farmer player)
    {
        this.Player = player;
    }

    /// <inheritdoc />
    public Farmer Player { get; }
}
