namespace DaLion.Common.Events;

#region using directives

using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Wrapper for <see cref="IDisplayEvents.Rendering"/> allowing dynamic hooking / unhooking.</summary>
internal abstract class RenderingEvent : BaseEvent
{
    /// <inheritdoc cref="IDisplayEvents.Rendering"/>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    internal void OnRendering(object sender, RenderingEventArgs e)
    {
        if (hooked.Value || GetType().Name.StartsWith("Static")) OnRenderingImpl(sender, e);
    }

    /// <inheritdoc cref="OnRendering" />
    protected abstract void OnRenderingImpl(object sender, RenderingEventArgs e);
}