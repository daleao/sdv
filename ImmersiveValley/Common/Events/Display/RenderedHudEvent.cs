namespace DaLion.Common.Events;

#region using directives

using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Wrapper for <see cref="IDisplayEvents.RenderedHud"/> allowing dynamic hooking / unhooking.</summary>
internal abstract class RenderedHudEvent : BaseEvent
{
    /// <inheritdoc cref="IDisplayEvents.RenderedHud"/>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    internal void OnRenderedHud(object sender, RenderedHudEventArgs e)
    {
        if (hooked.Value || GetType().Name.StartsWith("Static")) OnRenderedHudImpl(sender, e);
    }

    /// <inheritdoc cref="OnRenderedHud" />
    protected abstract void OnRenderedHudImpl(object sender, RenderedHudEventArgs e);
}