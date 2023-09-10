namespace DaLion.Overhaul.Modules.Professions.Events.TreasureHunt.TreasureHuntEnded;

#region using directives

using DaLion.Overhaul.Modules.Professions.TreasureHunts;

#endregion using directives

/// <summary>Interface for the arguments of a <see cref="TreasureHuntEndedEvent"/>.</summary>
public interface ITreasureHuntEndedEventArgs
{
    /// <summary>Gets the player who triggered the event.</summary>
    Farmer Player { get; }

    /// <summary>Gets whether this event relates to a Scavenger or Prospector hunt.</summary>
    TreasureHuntType Type { get; }

    /// <summary>Gets a value indicating whether the player successfully discovered the treasure.</summary>
    bool TreasureFound { get; }
}
