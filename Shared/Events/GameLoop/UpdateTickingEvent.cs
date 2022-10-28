namespace DaLion.Shared.Events;

#region using directives

using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Wrapper for <see cref="IGameLoopEvents.UpdateTicking"/> allowing dynamic enabling / disabling.</summary>
internal abstract class UpdateTickingEvent : ManagedEvent
{
    /// <summary>Initializes a new instance of the <see cref="UpdateTickingEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    protected UpdateTickingEvent(EventManager manager)
        : base(manager)
    {
        manager.ModEvents.GameLoop.UpdateTicking += this.OnUpdateTicking;
    }

    /// <inheritdoc cref="IGameLoopEvents.UpdateTicking"/>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    internal void OnUpdateTicking(object? sender, UpdateTickingEventArgs e)
    {
        if (this.IsEnabled)
        {
            this.OnUpdateTickingImpl(sender, e);
        }
    }

    /// <inheritdoc cref="OnUpdateTicking"/>
    protected abstract void OnUpdateTickingImpl(object? sender, UpdateTickingEventArgs e);
}
