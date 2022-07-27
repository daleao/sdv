namespace DaLion.Common.Events;

#region using directives

using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Wrapper for <see cref="IDisplayEvents.RenderedHud"/> allowing dynamic enabling / disabling.</summary>
internal abstract class RenderedHudEvent : ManagedEvent
{
    /// <summary>Construct an instance.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    protected RenderedHudEvent(EventManager manager)
        : base(manager) { }

    /// <inheritdoc cref="IDisplayEvents.RenderedHud"/>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    internal void OnRenderedHud(object? sender, RenderedHudEventArgs e)
    {
        if (IsEnabled) OnRenderedHudImpl(sender, e);
    }

    /// <inheritdoc cref="OnRenderedHud" />
    protected abstract void OnRenderedHudImpl(object? sender, RenderedHudEventArgs e);
}