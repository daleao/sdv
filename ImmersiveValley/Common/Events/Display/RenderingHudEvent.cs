namespace DaLion.Common.Events;

#region using directives

using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Wrapper for <see cref="IDisplayEvents.RenderingHud"/> allowing dynamic hooking / unhooking.</summary>
internal abstract class RenderingHudEvent : BaseEvent
{
    /// <inheritdoc cref="IDisplayEvents.RenderingHud"/>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    internal void OnRenderingHud(object sender, RenderingHudEventArgs e)
    {
        if (hooked.Value || GetType().Name.StartsWith("Static")) OnRenderingHudImpl(sender, e);
    }

    /// <inheritdoc cref="OnRenderingHud" />
    protected abstract void OnRenderingHudImpl(object sender, RenderingHudEventArgs e);
}