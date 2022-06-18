namespace DaLion.Stardew.Professions.Framework.Events.Ultimate;

#region using directives

using System;
using StardewValley;

#endregion using directives

public sealed class UltimateEmptiedEventArgs : EventArgs, IUltimateEmptiedEventArgs
{
    /// <inheritdoc />
    public Farmer Player { get; }

    /// <summary>Construct an instance.</summary>
    internal UltimateEmptiedEventArgs(Farmer player)
    {
        Player = player;
    }
}