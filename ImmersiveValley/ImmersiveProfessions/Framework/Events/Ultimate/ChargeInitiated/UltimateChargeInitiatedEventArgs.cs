namespace DaLion.Stardew.Professions.Framework.Events.Ultimate;

#region using directives

using System;

#endregion using directives

/// <summary>The arguments for an <see cref="UltimateChargeInitiatedEvent"/>.</summary>
internal sealed class UltimateChargeInitiatedEventArgs : EventArgs, IUltimateChargeInitiatedEventArgs
{
    /// <inheritdoc />
    public Farmer Player { get; }

    /// <inheritdoc />
    public double NewValue { get; }

    /// <summary>Construct an instance.</summary>
    /// <param name="player">The player who triggered the event.</param>
    /// <param name="newValue">The new charge value.</param>
    internal UltimateChargeInitiatedEventArgs(Farmer player, double newValue)
    {
        Player = player;
        NewValue = newValue;
    }
}