namespace DaLion.Common.Events;

#region using directives

using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Wrapper for <see cref="IDisplayEvents.RenderingActiveMenu"/> allowing dynamic hooking / unhooking.</summary>
internal abstract class RenderingActiveMenuEvent : ManagedEvent
{
    /// <summary>Construct an instance.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    protected RenderingActiveMenuEvent(EventManager manager)
        : base(manager) { }

    /// <inheritdoc cref="IDisplayEvents.RenderingActiveMenu"/>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    internal void OnRenderingActiveMenu(object? sender, RenderingActiveMenuEventArgs e)
    {
        if (Hooked.Value || GetType().Name.StartsWith("Static")) OnRenderingActiveMenuImpl(sender, e);
    }

    /// <inheritdoc cref="OnRenderingActiveMenu" />
    protected abstract void OnRenderingActiveMenuImpl(object? sender, RenderingActiveMenuEventArgs e);
}