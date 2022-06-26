namespace DaLion.Stardew.Professions.Framework.Events.Ultimate;

#region using directives

using System;

using Common.Events;

#endregion using directives

internal sealed class UltimateDeactivatedEvent : ManagedEvent
{
    private readonly Action<object?, IUltimateDeactivatedEventArgs> _OnDeactivatedImpl;

    /// <summary>Construct an instance.</summary>
    /// <param name="callback">The delegate to run when the event is raised.</param>
    internal UltimateDeactivatedEvent(Action<object?, IUltimateDeactivatedEventArgs> callback)
        : base(ModEntry.EventManager)
    {
        _OnDeactivatedImpl = callback;
    }

    /// <summary>Raised when a player's combat Ultimate ends.</summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    internal void OnDeactivated(object? sender, IUltimateDeactivatedEventArgs e)
    {
        if (Hooked.Value) _OnDeactivatedImpl(sender, e);
    }
}