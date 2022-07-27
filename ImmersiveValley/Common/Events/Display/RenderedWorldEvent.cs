namespace DaLion.Common.Events;

#region using directives

using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Wrapper for <see cref="IDisplayEvents.RenderedWorld"/> allowing dynamic enabling / disabling.</summary>
internal abstract class RenderedWorldEvent : ManagedEvent
{
    /// <summary>Construct an instance.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    protected RenderedWorldEvent(EventManager manager)
        : base(manager) { }

    /// <inheritdoc cref="IDisplayEvents.RenderedWorld"/>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    internal void OnRenderedWorld(object? sender, RenderedWorldEventArgs e)
    {
        if (IsEnabled) OnRenderedWorldImpl(sender, e);
    }

    /// <inheritdoc cref="OnRenderedWorld" />
    protected abstract void OnRenderedWorldImpl(object? sender, RenderedWorldEventArgs e);
}