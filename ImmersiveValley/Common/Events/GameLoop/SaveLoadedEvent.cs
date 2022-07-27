namespace DaLion.Common.Events;

#region using directives

using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Wrapper for <see cref="IGameLoopEvents.SaveLoaded"/> allowing dynamic enabling / disabling.</summary>
internal abstract class SaveLoadedEvent : ManagedEvent
{
    /// <summary>Construct an instance.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    protected SaveLoadedEvent(EventManager manager)
        : base(manager) { }

    /// <inheritdoc cref="IGameLoopEvents.SaveLoaded"/>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event data.</param>
    internal void OnSaveLoaded(object? sender, SaveLoadedEventArgs e)
    {
        if (IsEnabled) OnSaveLoadedImpl(sender, e);
    }

    /// <inheritdoc cref="OnSaveLoaded" />
    protected abstract void OnSaveLoadedImpl(object? sender, SaveLoadedEventArgs e);
}