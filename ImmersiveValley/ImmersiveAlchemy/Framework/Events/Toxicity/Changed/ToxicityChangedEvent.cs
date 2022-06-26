namespace DaLion.Stardew.Alchemy.Framework.Events.Toxicity;

#region using directives

using System;

using Common.Commands;
using Common.Events;

#endregion using directives

internal class ToxicityChangedEvent : ManagedEvent
{
    protected readonly Action<object?, IToxicityChangedEventArgs> _OnChargeInitiatedImpl;

    /// <summary>Construct an instance.</summary>
    /// <param name="callback">The delegate to run when the event is raised.</param>
    internal ToxicityChangedEvent(Action<object?, IToxicityChangedEventArgs> callback)
        : base(ModEntry.EventManager)
    {
        _OnChargeInitiatedImpl = callback;
    }

    /// <summary>Raised when a player's Toxicity value changes.</summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    internal void OnChanged(object? sender, IToxicityChangedEventArgs e)
    {
        if (Hooked.Value) _OnChargeInitiatedImpl(sender, e);
    }
}