namespace DaLion.Stardew.Alchemy.Framework.Events.Toxicity;

#region using directives

using System;
using StardewValley;

#endregion using directives

internal class ToxicityClearedEventArgs : EventArgs, IToxicityClearedEventArgs
{
    /// <inheritdoc />
    public Farmer Player { get; }

    /// <summary>Construct an instance.</summary>
    /// <param name="player">The player who triggered the event.</param>
    internal ToxicityClearedEventArgs(Farmer player)
    {
        Player = player;
    }
}