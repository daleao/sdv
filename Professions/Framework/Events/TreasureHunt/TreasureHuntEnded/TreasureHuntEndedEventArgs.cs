namespace DaLion.Professions.Framework.Events.TreasureHunt.TreasureHuntEnded;

#region using directives

using DaLion.Professions.Framework.TreasureHunts;

#endregion using directives

/// <summary>The arguments for a <see cref="TreasureHuntEndedEvent"/>.</summary>
public sealed class TreasureHuntEndedEventArgs : EventArgs, ITreasureHuntEndedEventArgs
{
    /// <summary>Initializes a new instance of the <see cref="TreasureHuntEndedEventArgs"/> class.</summary>
    /// <param name="player">The player who triggered the event.</param>
    /// <param name="profession">Whether this event relates to a Scavenger or Prospector hunt.</param>
    /// <param name="found">Whether the player successfully discovered the treasure.</param>
    internal TreasureHuntEndedEventArgs(Farmer player, TreasureHuntProfession profession, bool found)
    {
        this.Player = player;
        this.Profession = profession;
        this.TreasureFound = found;
    }

    /// <inheritdoc />
    public Farmer Player { get; }

    /// <inheritdoc />
    public TreasureHuntProfession Profession { get; }

    /// <inheritdoc />
    public bool TreasureFound { get; }
}
