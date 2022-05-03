namespace DaLion.Stardew.Professions.Framework.Events.Ultimate;

#region using directives

using System;

#endregion using directives

internal class UltimateChargeInitiatedEvent : BaseEvent
{
    protected readonly Action<object, IUltimateChargeInitiatedEventArgs> _OnChargeInitiatedImpl;

    /// <summary>Construct an instance.</summary>
    /// <param name="callback">The delegate to run when the event is raised.</param>
    internal UltimateChargeInitiatedEvent(Action<object, IUltimateChargeInitiatedEventArgs> callback)
    {
        _OnChargeInitiatedImpl = callback;
    }

    /// <summary>Raised when a player's combat Ultimate gains any charge while it was previously empty.</summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    internal void OnChargeInitiated(object sender, IUltimateChargeInitiatedEventArgs e)
    {
        if (enabled.Value) _OnChargeInitiatedImpl(sender, e);
    }
}