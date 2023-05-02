namespace DaLion.Alchemy.Events.Toxicity.Cleared;

#region using directives

using System;
using StardewValley;

#endregion using directives

/// <summary>The arguments for a <see cref="ToxicityClearedEvent"/>.</summary>
internal class ToxicityClearedEventArgs : EventArgs, IToxicityClearedEventArgs
{
    /// <summary>Initializes a new instance of the <see cref="ToxicityClearedEventArgs"/> class.</summary>
    /// <param name="player">The player who triggered the event.</param>
    internal ToxicityClearedEventArgs(Farmer player)
    {
        this.Player = player;
    }

    /// <inheritdoc />
    public Farmer Player { get; }
}
