namespace DaLion.Professions.Framework.TreasureHunts.Events;

#region using directives

using DaLion.Professions.Framework.TreasureHunts;
using Microsoft.Xna.Framework;

#endregion using directives

/// <summary>Interface for the arguments of a <see cref="TreasureHuntEndedEvent"/>.</summary>
public interface ITreasureHuntStartedEventArgs
{
    /// <summary>Gets the player who triggered the event.</summary>
    Farmer Player { get; }

    /// <summary>Gets whether this event relates to a Scavenger or Prospector hunt.</summary>
    TreasureHuntProfession Profession { get; }

    /// <summary>Gets the coordinates of the target tile.</summary>
    Vector2 Target { get; }

    /// <summary>Gets the time limit for the treasure hunt, in seconds.</summary>
    public uint TimeLimit { get; }
}
