namespace DaLion.Stardew.Professions.Framework.Events.TreasureHunt;

#region using directives

using System;
using TreasureHunts;

#endregion using directives

/// <summary>The arguments for a <see cref="TreasureHuntEndedEvent"/>.</summary>
public sealed class TreasureHuntEndedEventArgs : EventArgs, ITreasureHuntEndedEventArgs
{
    /// <inheritdoc />
    public Farmer Player { get; }

    /// <inheritdoc />
    public TreasureHuntType Type { get; }

    /// <inheritdoc />
    public bool TreasureFound { get; }

    /// <summary>Construct an instance.</summary>
    /// <param name="player">The player who triggered the event.</param>
    /// <param name="type">Whether this event relates to a Scavenger or Prospector hunt.</param>
    /// <param name="found">Whether the player successfully discovered the treasure.</param>
    internal TreasureHuntEndedEventArgs(Farmer player, TreasureHuntType type, bool found)
    {
        Player = player;
        Type = type;
        TreasureFound = found;
    }
}