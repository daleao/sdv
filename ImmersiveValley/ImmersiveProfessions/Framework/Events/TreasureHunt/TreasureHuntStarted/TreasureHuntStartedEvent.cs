namespace DaLion.Stardew.Professions.Framework.Events.TreasureHunt;

#region using directives

using System;

using Common.Events;

#endregion using directives

/// <summary>A dynamic event raised when a <see cref="TreasureHunts.ITreasureHunt"> starts.</summary>
internal sealed class TreasureHuntStartedEvent : ManagedEvent
{
    private readonly Action<object?, ITreasureHuntStartedEventArgs> _OnStartedImpl;

    /// <summary>Construct an instance.</summary>
    /// <param name="callback">The delegate to run when the event is raised.</param>
    internal TreasureHuntStartedEvent(Action<object?, ITreasureHuntStartedEventArgs> callback)
        : base(ModEntry.EventManager)
    {
        _OnStartedImpl = callback;
    }

    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    internal void OnStarted(object? sender, ITreasureHuntStartedEventArgs e)
    {
        if (IsHooked) _OnStartedImpl(sender, e);
    }
}