namespace DaLion.Overhaul.Modules.Professions.Events.Ultimate;

/// <summary>The arguments for an <see cref="UltimateFullyChargedEvent"/>.</summary>
public sealed class UltimateFullyChargedEventArgs : EventArgs, IUltimateFullyChargedEventArgs
{
    /// <summary>Initializes a new instance of the <see cref="UltimateFullyChargedEventArgs"/> class.</summary>
    /// <param name="player">The player who triggered the event.</param>
    internal UltimateFullyChargedEventArgs(Farmer player)
    {
        this.Player = player;
    }

    /// <inheritdoc />
    public Farmer Player { get; }
}
