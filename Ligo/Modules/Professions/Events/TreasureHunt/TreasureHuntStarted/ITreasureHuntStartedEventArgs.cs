namespace DaLion.Ligo.Modules.Professions.Events.TreasureHunt;

#region using directives

using DaLion.Ligo.Modules.Professions.TreasureHunts;
using Microsoft.Xna.Framework;

#endregion using directives

/// <summary>Interface for the arguments of a <see cref="TreasureHuntEndedEvent"/>.</summary>
public interface ITreasureHuntStartedEventArgs
{
    /// <summary>Gets the player who triggered the event.</summary>
    Farmer Player { get; }

    /// <summary>Gets determines whether this event relates to a Scavenger or Prospector hunt.</summary>
    TreasureHuntType Type { get; }

    /// <summary>Gets the coordinates of the target tile.</summary>
    Vector2 Target { get; }
}
