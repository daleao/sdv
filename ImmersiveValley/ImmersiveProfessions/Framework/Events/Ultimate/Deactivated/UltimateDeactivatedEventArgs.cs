namespace DaLion.Stardew.Professions.Framework.Events.Ultimate;

#region using directives

using System;

#endregion using directives

/// <summary>The arguments for an <see cref="UltimateDeactivatedEvent"/>.</summary>
public sealed class UltimateDeactivatedEventArgs : EventArgs, IUltimateDeactivatedEventArgs
{
    /// <inheritdoc />
    public Farmer Player { get; }

    /// <summary>Construct an instance.</summary>
    /// <param name="player">The player who triggered the event.</param>
    internal UltimateDeactivatedEventArgs(Farmer player)
    {
        Player = player;
    }
}