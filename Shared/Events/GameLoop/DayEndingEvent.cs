namespace DaLion.Shared.Events;

#region using directives

using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Wrapper for <see cref="IGameLoopEvents.DayEnding"/> allowing dynamic enabling / disabling.</summary>
internal abstract class DayEndingEvent : ManagedEvent
{
    /// <summary>Initializes a new instance of the <see cref="DayEndingEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    protected DayEndingEvent(EventManager manager)
        : base(manager)
    {
        manager.ModEvents.GameLoop.DayEnding += this.OnDayEnding;
    }

    /// <inheritdoc />
    public override void Dispose()
    {
        this.Manager.ModEvents.GameLoop.DayEnding -= this.OnDayEnding;
    }

    /// <inheritdoc cref="IGameLoopEvents.DayEnding"/>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    internal void OnDayEnding(object? sender, DayEndingEventArgs e)
    {
        if (this.IsEnabled)
        {
            this.OnDayEndingImpl(sender, e);
        }
    }

    /// <inheritdoc cref="OnDayEnding"/>
    protected abstract void OnDayEndingImpl(object? sender, DayEndingEventArgs e);
}
