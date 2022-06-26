namespace DaLion.Stardew.Professions.Framework.Events.Ultimate;

#region using directives

using StardewValley;
using System;

#endregion using directives

/// <summary>The arguments for an <see cref="UltimateFullyChargedEvent"/>.</summary>
public sealed class UltimateFullyChargedEventArgs : EventArgs, IUltimateFullyChargedEventArgs
{
    /// <inheritdoc />
    public Farmer Player { get; }

    /// <summary>Construct an instance.</summary>
    /// <param name="player">The player who triggered the event.</param>
    internal UltimateFullyChargedEventArgs(Farmer player)
    {
        Player = player;
    }
}