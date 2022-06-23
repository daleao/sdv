namespace DaLion.Common.Events;

#region using directives

using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Wrapper for <see cref="IDisplayEvents.RenderingWorld"/> allowing dynamic hooking / unhooking.</summary>
internal abstract class RenderingWorldEvent : BaseEvent
{
    /// <inheritdoc cref="IDisplayEvents.RenderingWorld"/>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    internal void OnRenderingWorld(object sender, RenderingWorldEventArgs e)
    {
        if (hooked.Value || GetType().Name.StartsWith("Static")) OnRenderingWorldImpl(sender, e);
    }

    /// <inheritdoc cref="OnRenderingWorld" />
    protected abstract void OnRenderingWorldImpl(object sender, RenderingWorldEventArgs e);
}