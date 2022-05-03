namespace DaLion.Stardew.Professions.Framework.Events.TreasureHunt;

#region using directives

using System;
using Microsoft.Xna.Framework;
using StardewValley;

#endregion using directives

public class TreasureHuntStartedEventArgs : EventArgs, ITreasureHuntStartedEventArgs
{
    /// <inheritdoc />
    public Farmer Player { get; }

    /// <inheritdoc />
    public Vector2 Target { get; }

    /// <summary>Construct an instance.</summary>
    internal TreasureHuntStartedEventArgs(Farmer player, Vector2 target)
    {
        Player = player;
        Target = target;
    }
}