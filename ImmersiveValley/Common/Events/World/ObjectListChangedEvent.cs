namespace DaLion.Common.Events;

#region using directives

using StardewModdingAPI.Events;

#endregion region using directives

/// <summary>Wrapper for <see cref="IWorldEvents.ObjectListChanged"/> allowing dynamic enabling / disabling.</summary>
internal abstract class ObjectListChangedEvent : ManagedEvent
{
    /// <summary>Construct an instance.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    protected ObjectListChangedEvent(EventManager manager)
        : base(manager) { }

    /// <inheritdoc cref="IWorldEvents.ObjectListChanged"/>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    internal void OnObjectListChanged(object? sender, ObjectListChangedEventArgs e)
    {
        if (IsEnabled) OnObjectListChangedImpl(sender, e);
    }

    /// <inheritdoc cref="OnObjectListChanged" />
    protected abstract void OnObjectListChangedImpl(object? sender, ObjectListChangedEventArgs e);
}