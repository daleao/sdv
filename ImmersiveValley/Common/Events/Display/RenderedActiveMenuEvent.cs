namespace DaLion.Common.Events;

#region using directives

using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Wrapper for <see cref="IDisplayEvents.RenderedActiveMenu"/> allowing dynamic hooking / unhooking.</summary>
internal abstract class RenderedActiveMenuEvent : BaseEvent
{
    /// <inheritdoc cref="IDisplayEvents.RenderedActiveMenu"/>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    internal void OnRenderedActiveMenu(object sender, RenderedActiveMenuEventArgs e)
    {
        if (hooked.Value || GetType().Name.StartsWith("Static")) OnRenderedActiveMenuImpl(sender, e);
    }

    /// <inheritdoc cref="OnRenderedActiveMenu" />
    protected abstract void OnRenderedActiveMenuImpl(object sender, RenderedActiveMenuEventArgs e);
}