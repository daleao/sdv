namespace DaLion.Stardew.Professions.Framework.Events.Ultimate;

#region using directives

using Common.Events;
using System;

#endregion using directives

/// <summary>A dynamic event raised when a <see cref="Ultimates.IUltimate"> is gains any charge while it was previously empty.</summary>
internal sealed class UltimateChargeInitiatedEvent : ManagedEvent
{
    private readonly Action<object?, IUltimateChargeInitiatedEventArgs> _OnChargeInitiatedImpl;

    /// <summary>Construct an instance.</summary>
    /// <param name="callback">The delegate to run when the event is raised.</param>
    internal UltimateChargeInitiatedEvent(Action<object?, IUltimateChargeInitiatedEventArgs> callback)
        : base(ModEntry.EventManager)
    {
        _OnChargeInitiatedImpl = callback;
    }

    /// <summary>Raised when a player's combat <see cref="Ultimates.IUltimate"/> gains any charge while it was previously empty.</summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    internal void OnChargeInitiated(object? sender, IUltimateChargeInitiatedEventArgs e)
    {
        if (IsHooked) _OnChargeInitiatedImpl(sender, e);
    }
}