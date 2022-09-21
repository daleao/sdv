namespace DaLion.Stardew.Professions.Framework.Events.TreasureHunt;

#region using directives

using System;
using DaLion.Stardew.Professions.Framework.TreasureHunts;

#endregion using directives

/// <summary>The arguments for a <see cref="TreasureHuntEndedEvent"/>.</summary>
public sealed class TreasureHuntEndedEventArgs : EventArgs, ITreasureHuntEndedEventArgs
{
    /// <summary>Initializes a new instance of the <see cref="TreasureHuntEndedEventArgs"/> class.</summary>
    /// <param name="player">The player who triggered the event.</param>
    /// <param name="type">Whether this event relates to a Scavenger or Prospector hunt.</param>
    /// <param name="found">Whether the player successfully discovered the treasure.</param>
    internal TreasureHuntEndedEventArgs(Farmer player, TreasureHuntType type, bool found)
    {
        this.Player = player;
        this.Type = type;
        this.TreasureFound = found;
    }

    /// <inheritdoc />
    public Farmer Player { get; }

    /// <inheritdoc />
    public TreasureHuntType Type { get; }

    /// <inheritdoc />
    public bool TreasureFound { get; }
}
