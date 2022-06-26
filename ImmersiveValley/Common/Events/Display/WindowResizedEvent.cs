namespace DaLion.Common.Events;

#region using directives

using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Wrapper for <see cref="IDisplayEvents.WindowResized"/> allowing dynamic hooking / unhooking.</summary>
internal abstract class WindowResizedEvent : ManagedEvent
{
    /// <summary>Construct an instance.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    protected WindowResizedEvent(EventManager manager)
        : base(manager) { }

    /// <inheritdoc cref="IDisplayEvents.WindowResized"/>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    internal void OnWindowResized(object? sender, WindowResizedEventArgs e)
    {
        if (IsHooked) OnWindowResizedImpl(sender, e);
    }

    /// <inheritdoc cref="OnWindowResized" />
    protected abstract void OnWindowResizedImpl(object? sender, WindowResizedEventArgs e);
}