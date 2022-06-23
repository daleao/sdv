namespace DaLion.Stardew.Alchemy.Framework.Events.Toxicity;

#region using directives

using System;

using Common.Events;

#endregion using directives

internal class ToxicityFilledEvent : BaseEvent
{
    protected readonly Action<object, IToxicityFilledEventArgs> _OnChargeInitiatedImpl;

    /// <summary>Construct an instance.</summary>
    /// <param name="callback">The delegate to run when the event is raised.</param>
    internal ToxicityFilledEvent(Action<object, IToxicityFilledEventArgs> callback)
    {
        _OnChargeInitiatedImpl = callback;
    }

    /// <summary>Raised when a player's Toxicity value reaches the maximum value.</summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    internal void OnFilled(object sender, IToxicityFilledEventArgs e)
    {
        if (hooked.Value) _OnChargeInitiatedImpl(sender, e);
    }
}