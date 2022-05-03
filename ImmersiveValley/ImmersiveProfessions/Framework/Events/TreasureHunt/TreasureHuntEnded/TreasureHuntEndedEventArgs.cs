namespace DaLion.Stardew.Professions.Framework.Events.TreasureHunt;

#region using directives

using System;
using StardewValley;

#endregion using directives

public class TreasureHuntEndedEventArgs : EventArgs, ITreasureHuntEndedEventArgs
{
    /// <inheritdoc />
    public Farmer Player { get; }

    /// <inheritdoc />
    public bool TreasureFound { get; }

    /// <summary>Construct an instance.</summary>
    internal TreasureHuntEndedEventArgs(Farmer player, bool found)
    {
        Player = player;
        TreasureFound = found;
    }
}