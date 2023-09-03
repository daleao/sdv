namespace DaLion.Overhaul.Modules.Professions.Events.TreasureHunt.TreasureHuntStarted;

#region using directives

using DaLion.Overhaul.Modules.Professions.TreasureHunts;
using Microsoft.Xna.Framework;

#endregion using directives

/// <summary>The arguments for a <see cref="TreasureHuntStartedEvent"/>.</summary>
public sealed class TreasureHuntStartedEventArgs : EventArgs, ITreasureHuntStartedEventArgs
{
    /// <summary>Initializes a new instance of the <see cref="TreasureHuntStartedEventArgs"/> class.</summary>
    /// <param name="player">The player who triggered the event.</param>
    /// <param name="type">Whether this event relates to a Scavenger or Prospector hunt.</param>
    /// <param name="target">The coordinates of the target tile.</param>
    internal TreasureHuntStartedEventArgs(Farmer player, TreasureHuntType type, Vector2 target)
    {
        this.Player = player;
        this.Type = type;
        this.Target = target;
    }

    /// <inheritdoc />
    public Farmer Player { get; }

    /// <inheritdoc />
    public TreasureHuntType Type { get; }

    /// <inheritdoc />
    public Vector2 Target { get; }
}
