namespace DaLion.Common.Events;

#region using directives

using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Wrapper for <see cref="IGameLoopEvents.Saved"/> allowing dynamic hooking / unhooking.</summary>
internal abstract class SavedEvent : ManagedEvent
{
    /// <summary>Construct an instance.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    protected SavedEvent(EventManager manager)
        : base(manager) { }

    /// <inheritdoc cref="IGameLoopEvents.Saved"/>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event data.</param>
    internal void OnSaved(object? sender, SavedEventArgs e)
    {
        if (Hooked.Value || GetType().Name.StartsWith("Static")) OnSavedImpl(sender, e);
    }

    /// <inheritdoc cref="OnSaved" />
    protected abstract void OnSavedImpl(object? sender, SavedEventArgs e);
}