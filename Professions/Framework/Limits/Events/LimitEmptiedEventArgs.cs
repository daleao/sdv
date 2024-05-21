namespace DaLion.Professions.Framework.Limits.Events;

/// <summary>The arguments for a <see cref="LimitEmptiedEvent"/>.</summary>
public sealed class LimitEmptiedEventArgs : EventArgs, ILimitEmptiedEventArgs
{
    /// <summary>Initializes a new instance of the <see cref="LimitEmptiedEventArgs"/> class.</summary>
    /// <param name="player">The player who triggered the event.</param>
    internal LimitEmptiedEventArgs(Farmer player)
    {
        this.Player = player;
    }

    /// <inheritdoc />
    public Farmer Player { get; }
}
