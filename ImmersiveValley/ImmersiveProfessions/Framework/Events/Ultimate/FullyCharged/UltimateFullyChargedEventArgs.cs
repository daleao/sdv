namespace DaLion.Stardew.Professions.Framework.Events.Ultimate;

#region using directives

using System;
using StardewValley;

#endregion using directives

public sealed class UltimateFullyChargedEventArgs : EventArgs, IUltimateFullyChargedEventArgs
{
    /// <inheritdoc />
    public Farmer Player { get; }

    /// <summary>Construct an instance.</summary>
    internal UltimateFullyChargedEventArgs(Farmer player)
    {
        Player = player;
    }
}