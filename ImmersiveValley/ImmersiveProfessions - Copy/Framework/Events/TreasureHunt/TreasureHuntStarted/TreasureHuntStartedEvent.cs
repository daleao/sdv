namespace DaLion.Stardew.Professions.Framework.Events.TreasureHunt;

#region using directives

using Common.Events;
using System;

#endregion using directives

/// <summary>A dynamic event raised when a <see cref="TreasureHunts.ITreasureHunt"> starts.</summary>
internal sealed class TreasureHuntStartedEvent : ManagedEvent
{
    private readonly Action<object?, ITreasureHuntStartedEventArgs> _OnStartedImpl;

    /// <summary>Construct an instance.</summary>
    /// <param name="callback">The delegate to run when the event is raised.</param>
    /// <param name="alwaysHooked">Whether the event should be allowed to override the <c>hooked</c> flag.</param>
    internal TreasureHuntStartedEvent(Action<object?, ITreasureHuntStartedEventArgs> callback, bool alwaysHooked = false)
        : base(ModEntry.EventManager)
    {
        _OnStartedImpl = callback;
        AlwaysHooked = alwaysHooked;
    }

    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    internal void OnStarted(object? sender, ITreasureHuntStartedEventArgs e)
    {
        if (IsHooked) _OnStartedImpl(sender, e);
    }
}