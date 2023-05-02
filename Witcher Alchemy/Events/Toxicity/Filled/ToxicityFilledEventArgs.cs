namespace DaLion.Alchemy.Events.Toxicity.Filled;

#region using directives

using System;
using StardewValley;

#endregion using directives

/// <summary>The arguments for a <see cref="ToxicityFilledEvent"/>.</summary>
internal class ToxicityFilledEventArgs : EventArgs, IToxicityFilledEventArgs
{
    /// <summary>Initializes a new instance of the <see cref="ToxicityFilledEventArgs"/> class.</summary>
    /// <param name="player">The player who triggered the event.</param>
    internal ToxicityFilledEventArgs(Farmer player)
    {
        this.Player = player;
    }

    /// <inheritdoc />
    public Farmer Player { get; }
}
