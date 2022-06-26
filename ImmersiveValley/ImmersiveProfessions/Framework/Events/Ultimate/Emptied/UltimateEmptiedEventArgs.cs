namespace DaLion.Stardew.Professions.Framework.Events.Ultimate;

#region using directives

using System;
using StardewValley;

#endregion using directives

/// <summary>The arguments for an <see cref="UltimateEmptiedEvent"/>.</summary>
public sealed class UltimateEmptiedEventArgs : EventArgs, IUltimateEmptiedEventArgs
{
    /// <inheritdoc />
    public Farmer Player { get; }

    /// <summary>Construct an instance.</summary>
    /// <param name="player">The player who triggered the event.</param>
    internal UltimateEmptiedEventArgs(Farmer player)
    {
        Player = player;
    }
}