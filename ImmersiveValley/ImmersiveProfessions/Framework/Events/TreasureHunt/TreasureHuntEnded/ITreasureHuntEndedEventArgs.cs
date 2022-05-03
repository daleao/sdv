namespace DaLion.Stardew.Professions.Framework.Events.TreasureHunt;

#region using directives

using StardewValley;

#endregion using directives

public interface ITreasureHuntEndedEventArgs
{
    /// <summary>The player who triggered the event.</summary>
    Farmer Player { get; }

    /// <summary>Whether the player successfully discovered the treasure.</summary>
    bool TreasureFound { get; }
}