namespace DaLion.Stardew.Professions.Framework.Events.TreasureHunt;

#region using directives

using Common.Events;
using System;

#endregion using directives

/// <summary>A dynamic event raised when a <see cref="TreasureHunts.ITreasureHunt"> ends.</summary>
internal sealed class TreasureHuntEndedEvent : ManagedEvent
{
    private readonly Action<object?, ITreasureHuntEndedEventArgs> _OnEndedImpl;

    /// <summary>Construct an instance.</summary>
    /// <param name="callback">The delegate to run when the event is raised.</param>
    /// <param name="alwaysHooked">Whether the event should be allowed to override the <c>hooked</c> flag.</param>
    internal TreasureHuntEndedEvent(Action<object?, ITreasureHuntEndedEventArgs> callback, bool alwaysHooked = false)
        : base(ModEntry.EventManager)
    {
        _OnEndedImpl = callback;
        AlwaysHooked = alwaysHooked;
    }

    /// <summary>Raised when a <see cref="TreasureHunts.ITreasureHunt"> ends.</summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    internal void OnEnded(object? sender, ITreasureHuntEndedEventArgs e)
    {
        if (IsHooked) _OnEndedImpl(sender, e);
    }
}