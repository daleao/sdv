namespace DaLion.Stardew.Alchemy.Framework.Events.Toxicity;

#region using directives

using System;

using Common.Events;

#endregion using directives

internal class ToxicityChangedEvent : BaseEvent
{
    protected readonly Action<object, IToxicityChangedEventArgs> _OnChargeInitiatedImpl;

    /// <summary>Construct an instance.</summary>
    /// <param name="callback">The delegate to run when the event is raised.</param>
    internal ToxicityChangedEvent(Action<object, IToxicityChangedEventArgs> callback)
    {
        _OnChargeInitiatedImpl = callback;
    }

    /// <summary>Raised when a player's Toxicity value changes.</summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    internal void OnChanged(object sender, IToxicityChangedEventArgs e)
    {
        if (hooked.Value) _OnChargeInitiatedImpl(sender, e);
    }
}