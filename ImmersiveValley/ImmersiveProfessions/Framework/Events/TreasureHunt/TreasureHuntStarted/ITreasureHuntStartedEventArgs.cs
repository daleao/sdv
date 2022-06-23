namespace DaLion.Stardew.Professions.Framework.Events.TreasureHunt;

#region using directives

using Microsoft.Xna.Framework;
using StardewValley;

using TreasureHunts;

#endregion using directives

public interface ITreasureHuntStartedEventArgs
{
    /// <summary>The player who triggered the event.</summary>
    Farmer Player { get; }

    /// <summary>Whether this event relates to a Scavenger or Prospector hunt.</summary>
    TreasureHuntType Type { get; }

    /// <summary>The coordinates of the target tile.</summary>
    Vector2 Target { get; }
}