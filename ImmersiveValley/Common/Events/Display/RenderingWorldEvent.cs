namespace DaLion.Common.Events;

#region using directives

using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Wrapper for <see cref="IDisplayEvents.RenderingWorld"/> allowing dynamic hooking / unhooking.</summary>
internal abstract class RenderingWorldEvent : ManagedEvent
{
    /// <summary>Construct an instance.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    protected RenderingWorldEvent(EventManager manager)
        : base(manager) { }

    /// <inheritdoc cref="IDisplayEvents.RenderingWorld"/>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    internal void OnRenderingWorld(object? sender, RenderingWorldEventArgs e)
    {
        if (Hooked.Value || GetType().Name.StartsWith("Static")) OnRenderingWorldImpl(sender, e);
    }

    /// <inheritdoc cref="OnRenderingWorld" />
    protected abstract void OnRenderingWorldImpl(object? sender, RenderingWorldEventArgs e);
}