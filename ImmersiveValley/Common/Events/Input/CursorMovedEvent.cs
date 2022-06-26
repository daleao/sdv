namespace DaLion.Common.Events;

#region using directives

using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Wrapper for <see cref="IInputEvents.CursorMoved"/> allowing dynamic hooking / unhooking.</summary>
internal abstract class CursorMovedEvent : ManagedEvent
{
    /// <summary>Construct an instance.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    protected CursorMovedEvent(EventManager manager)
        : base(manager) { }

    /// <inheritdoc cref="IInputEvents.CursorMoved"/>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    internal void OnCursorMoved(object? sender, CursorMovedEventArgs e)
    {
        if (Hooked.Value) OnCursorMovedImpl(sender, e);
    }

    /// <inheritdoc cref="OnCursorMoved" />
    protected abstract void OnCursorMovedImpl(object? sender, CursorMovedEventArgs e);
}