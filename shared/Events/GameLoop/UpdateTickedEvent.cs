namespace DaLion.Shared.Events;

#region using directives

using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Wrapper for <see cref="IGameLoopEvents.UpdateTicked"/> allowing dynamic enabling / disabling.</summary>
internal abstract class UpdateTickedEvent : ManagedEvent
{
    /// <summary>Initializes a new instance of the <see cref="UpdateTickedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    protected UpdateTickedEvent(EventManager manager)
        : base(manager)
    {
        manager.ModEvents.GameLoop.UpdateTicked += this.OnUpdateTicked;
    }

    /// <inheritdoc />
    public override void Dispose()
    {
        this.Manager.ModEvents.GameLoop.UpdateTicked -= this.OnUpdateTicked;
    }

    /// <inheritdoc cref="IGameLoopEvents.UpdateTicked"/>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    internal void OnUpdateTicked(object? sender, UpdateTickedEventArgs e)
    {
        if (this.IsEnabled)
        {
            this.OnUpdateTickedImpl(sender, e);
        }
    }

    /// <inheritdoc cref="OnUpdateTicked"/>
    protected abstract void OnUpdateTickedImpl(object? sender, UpdateTickedEventArgs e);
}
