namespace DaLion.Stardew.Professions.Framework.Events.TreasureHunt;

#region using directives

using System;
using StardewValley;

using Framework.TreasureHunt;

#endregion using directives

public class TreasureHuntEndedEventArgs : EventArgs, ITreasureHuntEndedEventArgs
{
    /// <inheritdoc />
    public Farmer Player { get; }

    /// <inheritdoc />
    public TreasureHuntType Type { get; }

    /// <inheritdoc />
    public bool TreasureFound { get; }

    /// <summary>Construct an instance.</summary>
    internal TreasureHuntEndedEventArgs(Farmer player, TreasureHuntType type, bool found)
    {
        Player = player;
        Type = type;
        TreasureFound = found;
    }
}