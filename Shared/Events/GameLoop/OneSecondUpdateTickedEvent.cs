namespace DaLion.Shared.Events;

#region using directives

using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Wrapper for <see cref="IGameLoopEvents.OneSecondUpdateTicked"/> allowing dynamic enabling / disabling.</summary>
public abstract class OneSecondUpdateTickedEvent : ManagedEvent
{
    /// <summary>Initializes a new instance of the <see cref="OneSecondUpdateTickedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    protected OneSecondUpdateTickedEvent(EventManager manager)
        : base(manager)
    {
        manager.ModEvents.GameLoop.OneSecondUpdateTicked += this.OnOneSecondUpdateTicked;
    }

    /// <inheritdoc />
    public override void Dispose()
    {
        this.Manager.ModEvents.GameLoop.OneSecondUpdateTicked -= this.OnOneSecondUpdateTicked;
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc cref="IGameLoopEvents.OneSecondUpdateTicked"/>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    public void OnOneSecondUpdateTicked(object? sender, OneSecondUpdateTickedEventArgs e)
    {
        if (this.IsEnabled)
        {
            this.OnOneSecondUpdateTickedImpl(sender, e);
        }
    }

    /// <inheritdoc cref="OnOneSecondUpdateTicked"/>
    protected abstract void OnOneSecondUpdateTickedImpl(object? sender, OneSecondUpdateTickedEventArgs e);
}
