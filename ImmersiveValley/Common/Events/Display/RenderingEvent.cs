namespace DaLion.Common.Events;

#region using directives

using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Wrapper for <see cref="IDisplayEvents.Rendering"/> allowing dynamic enabling / disabling.</summary>
internal abstract class RenderingEvent : ManagedEvent
{
    /// <summary>Initializes a new instance of the <see cref="RenderingEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    protected RenderingEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc cref="IDisplayEvents.Rendering"/>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    internal void OnRendering(object? sender, RenderingEventArgs e)
    {
        if (this.IsEnabled)
        {
            this.OnRenderingImpl(sender, e);
        }
    }

    /// <inheritdoc cref="OnRendering"/>
    protected abstract void OnRenderingImpl(object? sender, RenderingEventArgs e);
}
