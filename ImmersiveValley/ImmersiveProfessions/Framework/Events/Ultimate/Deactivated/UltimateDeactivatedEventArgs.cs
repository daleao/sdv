namespace DaLion.Stardew.Professions.Framework.Events.Ultimate;

#region using directives

using System;
using StardewValley;

#endregion using directives

public class UltimateDeactivatedEventArgs : EventArgs, IUltimateDeactivatedEventArgs
{
    /// <inheritdoc />
    public Farmer Player { get; }

    /// <summary>Construct an instance.</summary>
    internal UltimateDeactivatedEventArgs(Farmer player)
    {
        Player = player;
    }
}