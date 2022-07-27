namespace DaLion.Stardew.Professions.Framework.Events.TreasureHunt;

#region using directives

using Microsoft.Xna.Framework;
using StardewValley;
using System;
using TreasureHunts;

#endregion using directives

/// <summary>The arguments for a <see cref="TreasureHuntStartedEvent"/>.</summary>
public sealed class TreasureHuntStartedEventArgs : EventArgs, ITreasureHuntStartedEventArgs
{
    /// <inheritdoc />
    public Farmer Player { get; }

    /// <inheritdoc />
    public TreasureHuntType Type { get; }

    /// <inheritdoc />
    public Vector2 Target { get; }

    /// <summary>Construct an instance.</summary>
    /// <param name="player">The player who triggered the event.</param>
    /// <param name="type">Whether this event relates to a Scavenger or Prospector hunt.</param>
    /// <param name="target">The coordinates of the target tile.</param>
    internal TreasureHuntStartedEventArgs(Farmer player, TreasureHuntType type, Vector2 target)
    {
        Player = player;
        Type = type;
        Target = target;
    }
}