namespace DaLion.Shared.Events;

#region using directives

using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Wrapper for <see cref="IGameLoopEvents.TimeChanged"/> allowing dynamic enabling / disabling.</summary>
public abstract class TimeChangedEvent : ManagedEvent
{
    /// <summary>Initializes a new instance of the <see cref="TimeChangedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    protected TimeChangedEvent(EventManager manager)
        : base(manager)
    {
        manager.ModEvents.GameLoop.TimeChanged += this.OnTimeChanged;
    }

    /// <inheritdoc />
    public override void Dispose()
    {
        this.Manager.ModEvents.GameLoop.TimeChanged -= this.OnTimeChanged;
    }

    /// <inheritdoc cref="IGameLoopEvents.TimeChanged"/>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event data.</param>
    public void OnTimeChanged(object? sender, TimeChangedEventArgs e)
    {
        if (this.IsEnabled)
        {
            this.OnTimeChangedImpl(sender, e);
        }
    }

    /// <inheritdoc cref="OnTimeChanged"/>
    protected abstract void OnTimeChangedImpl(object? sender, TimeChangedEventArgs e);
}
