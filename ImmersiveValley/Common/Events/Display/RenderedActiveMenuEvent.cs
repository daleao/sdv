namespace DaLion.Common.Events;

#region using directives

using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Wrapper for <see cref="IDisplayEvents.RenderedActiveMenu"/> allowing dynamic hooking / unhooking.</summary>
internal abstract class RenderedActiveMenuEvent : ManagedEvent
{
    /// <summary>Construct an instance.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    protected RenderedActiveMenuEvent(EventManager manager)
        : base(manager) { }

    /// <inheritdoc cref="IDisplayEvents.RenderedActiveMenu"/>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    internal void OnRenderedActiveMenu(object? sender, RenderedActiveMenuEventArgs e)
    {
        if (IsHooked) OnRenderedActiveMenuImpl(sender, e);
    }

    /// <inheritdoc cref="OnRenderedActiveMenu" />
    protected abstract void OnRenderedActiveMenuImpl(object? sender, RenderedActiveMenuEventArgs e);
}