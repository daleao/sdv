namespace DaLion.Stardew.Professions.Framework.Events.Ultimate;

#region using directives

using System;

#endregion using directives

internal class UltimateDeactivatedEvent : BaseEvent
{
    private readonly Action<object, IUltimateDeactivatedEventArgs> _OnDeactivatedImpl;

    /// <summary>Construct an instance.</summary>
    /// <param name="callback">The delegate to run when the event is raised.</param>
    internal UltimateDeactivatedEvent(Action<object, IUltimateDeactivatedEventArgs> callback)
    {
        _OnDeactivatedImpl = callback;
    }

    /// <summary>Raised when a player's combat Ultimate ends.</summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    internal void OnDeactivated(object sender, IUltimateDeactivatedEventArgs e)
    {
        if (enabled.Value) _OnDeactivatedImpl(sender, e);
    }
}