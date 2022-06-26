namespace DaLion.Stardew.Alchemy.Framework.Events.Toxicity;

#region using directives

using StardewValley;
using System;

#endregion using directives

/// <summary>The arguments for a <see cref="PlayerOverdosedEvent"/>.</summary>
internal class PlayerOverdosedEventArgs : EventArgs, IPlayerOverdosedEventArgs
{
    /// <inheritdoc />
    public Farmer Player { get; }

    /// <summary>Construct an instance.</summary>
    /// <param name="player">The player who triggered the event.</param>
    internal PlayerOverdosedEventArgs(Farmer player)
    {
        Player = player;
    }
}