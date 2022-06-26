namespace DaLion.Stardew.Alchemy.Framework.Events.Toxicity;

#region using directives

using Common.Events;
using System;

#endregion using directives

/// <summary>A dynamic event raised when a player's Toxicity reaches the maximum value.</summary>
internal class ToxicityFilledEvent : ManagedEvent
{
    protected readonly Action<object?, IToxicityFilledEventArgs> _OnChargeInitiatedImpl;

    /// <summary>Construct an instance.</summary>
    /// <param name="callback">The delegate to run when the event is raised.</param>
    internal ToxicityFilledEvent(Action<object?, IToxicityFilledEventArgs> callback)
        : base(ModEntry.EventManager)
    {
        _OnChargeInitiatedImpl = callback;
    }

    /// <summary>Raised when a player's Toxicity reaches the maximum value.</summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    internal void OnFilled(object? sender, IToxicityFilledEventArgs e)
    {
        if (IsHooked) _OnChargeInitiatedImpl(sender, e);
    }
}