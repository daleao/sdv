﻿namespace DaLion.Shared.Events;

#region using directives

using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Wrapper for <see cref="IGameLoopEvents.DayStarted"/> allowing dynamic enabling / disabling.</summary>
public abstract class DayStartedEvent : ManagedEvent
{
    /// <summary>Initializes a new instance of the <see cref="DayStartedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    protected DayStartedEvent(EventManager manager)
        : base(manager)
    {
        manager.ModEvents.GameLoop.DayStarted += this.OnDayStarted;
    }

    /// <inheritdoc />
    public override void Dispose()
    {
        this.Manager.ModEvents.GameLoop.DayStarted -= this.OnDayStarted;
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc cref="OnDayStarted"/>
    protected abstract void OnDayStartedImpl(object? sender, DayStartedEventArgs e);

    /// <summary>Raised after a new in-game day starts, or after connecting to a multiplayer world.</summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    private void OnDayStarted(object? sender, DayStartedEventArgs e)
    {
        if (this.IsEnabled)
        {
            this.OnDayStartedImpl(sender, e);
        }
    }
}
