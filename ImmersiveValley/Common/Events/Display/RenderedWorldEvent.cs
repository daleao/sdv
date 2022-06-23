namespace DaLion.Common.Events;

#region using directives

using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Wrapper for <see cref="IDisplayEvents.RenderedWorld"/> allowing dynamic hooking / unhooking.</summary>
internal abstract class RenderedWorldEvent : BaseEvent
{
    /// <inheritdoc cref="IDisplayEvents.RenderedWorld"/>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    internal void OnRenderedWorld(object sender, RenderedWorldEventArgs e)
    {
        if (hooked.Value || GetType().Name.StartsWith("Static")) OnRenderedWorldImpl(sender, e);
    }

    /// <inheritdoc cref="OnRenderedWorld" />
    protected abstract void OnRenderedWorldImpl(object sender, RenderedWorldEventArgs e);
}