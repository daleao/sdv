namespace DaLion.Stardew.Professions.Framework.Events.Ultimate;

#region using directives

using System;
using StardewValley;

#endregion using directives

public sealed class UltimateActivatedEventArgs : EventArgs, IUltimateActivatedEventArgs
{
    /// <inheritdoc />
    public Farmer Player { get; }

    /// <summary>Construct an instance.</summary>
    /// <param name="player">The player who triggered the event.</param>
    internal UltimateActivatedEventArgs(Farmer player)
    {
        Player = player;
    }
}