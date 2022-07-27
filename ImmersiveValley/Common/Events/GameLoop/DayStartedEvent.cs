namespace DaLion.Common.Events;

#region using directives

using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Wrapper for <see cref="IGameLoopEvents.DayStarted"/> allowing dynamic enabling / disabling.</summary>
internal abstract class DayStartedEvent : ManagedEvent
{
    /// <summary>Construct an instance.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    protected DayStartedEvent(EventManager manager)
        : base(manager) { }

    /// <summary>Raised after a new in-game day starts, or after connecting to a multiplayer world.</summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    internal void OnDayStarted(object? sender, DayStartedEventArgs e)
    {
        if (IsEnabled) OnDayStartedImpl(sender, e);
    }

    /// <inheritdoc cref="OnDayStarted" />
    protected abstract void OnDayStartedImpl(object? sender, DayStartedEventArgs e);
}