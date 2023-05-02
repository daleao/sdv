namespace DaLion.Alchemy.Events.Toxicity.Overdosed;

#region using directives

using System;
using StardewValley;

#endregion using directives

/// <summary>The arguments for a <see cref="PlayerOverdosedEvent"/>.</summary>
internal class PlayerOverdosedEventArgs : EventArgs, IPlayerOverdosedEventArgs
{
    /// <summary>Initializes a new instance of the <see cref="PlayerOverdosedEventArgs"/> class.</summary>
    /// <param name="player">The player who triggered the event.</param>
    internal PlayerOverdosedEventArgs(Farmer player)
    {
        this.Player = player;
    }

    /// <inheritdoc />
    public Farmer Player { get; }
}
