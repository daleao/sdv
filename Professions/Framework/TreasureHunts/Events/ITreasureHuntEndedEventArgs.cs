namespace DaLion.Professions.Framework.TreasureHunts.Events;

#region using directives

using DaLion.Professions.Framework.TreasureHunts;

#endregion using directives

/// <summary>Interface for the arguments of a <see cref="TreasureHuntEndedEvent"/>.</summary>
public interface ITreasureHuntEndedEventArgs
{
    /// <summary>Gets the player who triggered the event.</summary>
    Farmer Player { get; }

    /// <summary>Gets whether this event is related to a <see cref="Profession.Scavenger"/>> or <see cref="Profession.Prospector"/>> hunt.</summary>
    TreasureHuntProfession Profession { get; }

    /// <summary>Gets a value indicating whether the player successfully discovered the treasure.</summary>
    bool TreasureFound { get; }
}
