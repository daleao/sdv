namespace DaLion.Common.Events;

#region using directives

using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Wrapper for <see cref="IDisplayEvents.Rendered"/> allowing dynamic hooking / unhooking.</summary>
internal abstract class RenderedEvent : ManagedEvent
{
    /// <summary>Construct an instance.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    protected RenderedEvent(EventManager manager)
        : base(manager) { }

    /// <inheritdoc cref="IDisplayEvents.Rendered"/>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    internal void OnRendered(object? sender, RenderedEventArgs e)
    {
        if (Hooked.Value || GetType().Name.StartsWith("Static")) OnRenderedImpl(sender, e);
    }

    /// <inheritdoc cref="OnRendered" />
    protected abstract void OnRenderedImpl(object? sender, RenderedEventArgs e);
}