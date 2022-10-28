namespace DaLion.Shared.Events;

#region using directives

using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Wrapper for <see cref="IGameLoopEvents.Saved"/> allowing dynamic enabling / disabling.</summary>
internal abstract class SavedEvent : ManagedEvent
{
    /// <summary>Initializes a new instance of the <see cref="SavedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    protected SavedEvent(EventManager manager)
        : base(manager)
    {
        manager.ModEvents.GameLoop.Saved += this.OnSaved;
    }

    /// <inheritdoc cref="IGameLoopEvents.Saved"/>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event data.</param>
    internal void OnSaved(object? sender, SavedEventArgs e)
    {
        if (this.IsEnabled)
        {
            this.OnSavedImpl(sender, e);
        }
    }

    /// <inheritdoc cref="OnSaved"/>
    protected abstract void OnSavedImpl(object? sender, SavedEventArgs e);
}
