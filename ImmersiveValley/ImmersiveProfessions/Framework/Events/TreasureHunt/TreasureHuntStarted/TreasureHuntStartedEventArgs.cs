namespace DaLion.Stardew.Professions.Framework.Events.TreasureHunt;

#region using directives

using System;
using Microsoft.Xna.Framework;
using StardewValley;

using Framework.TreasureHunt;

#endregion using directives

public class TreasureHuntStartedEventArgs : EventArgs, ITreasureHuntStartedEventArgs
{
    /// <inheritdoc />
    public Farmer Player { get; }

    /// <inheritdoc />
    public TreasureHuntType Type { get; }

    /// <inheritdoc />
    public Vector2 Target { get; }

    /// <summary>Construct an instance.</summary>
    internal TreasureHuntStartedEventArgs(Farmer player, TreasureHuntType type, Vector2 target)
    {
        Player = player;
        Type = type;
        Target = target;
    }
}