namespace DaLion.Stardew.Professions.Framework.Events.Ultimate;

#region using directives

using System;

using Common.Events;

#endregion using directives

internal sealed class UltimateActivatedEvent : BaseEvent
{
    private readonly Action<object, IUltimateActivatedEventArgs> _OnActivatedImpl;

    /// <summary>Construct an instance.</summary>
    /// <param name="callback">The delegate to run when the event is raised.</param>
    internal UltimateActivatedEvent(Action<object, IUltimateActivatedEventArgs> callback)
    {
        _OnActivatedImpl = callback;
    }

    /// <summary>Raised when a player activates their combat Ultimate.</summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    internal void OnActivated(object sender, IUltimateActivatedEventArgs e)
    {
        if (hooked.Value) _OnActivatedImpl(sender, e);
    }
}