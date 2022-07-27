namespace DaLion.Stardew.Professions.Framework.Events.Ultimate;

#region using directives

using System;

#endregion using directives

/// <summary>The arguments for an <see cref="UltimateChargeIncreasedEvent"/>.</summary>
public sealed class UltimateChargeIncreasedEventArgs : EventArgs, IUltimateChargeIncreasedEventArgs
{
    /// <inheritdoc />
    public Farmer Player { get; }

    /// <inheritdoc />
    public double OldValue { get; }

    /// <inheritdoc />
    public double NewValue { get; }

    /// <summary>Construct an instance.</summary>
    /// <param name="player">The player who triggered the event.</param>
    /// <param name="oldValue">The old charge value.</param>
    /// <param name="newValue">The new charge value.</param>
    internal UltimateChargeIncreasedEventArgs(Farmer player, double oldValue, double newValue)
    {
        Player = player;
        OldValue = oldValue;
        NewValue = newValue;
    }
}