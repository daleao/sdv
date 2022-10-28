namespace DaLion.Shared.Events;

#region using directives

using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Wrapper for <see cref="IDisplayEvents.Rendered"/> allowing dynamic enabling / disabling.</summary>
internal abstract class RenderedEvent : ManagedEvent
{
    /// <summary>Initializes a new instance of the <see cref="RenderedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    protected RenderedEvent(EventManager manager)
        : base(manager)
    {
        manager.ModEvents.Display.Rendered += this.OnRendered;
    }

    /// <inheritdoc cref="IDisplayEvents.Rendered"/>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    internal void OnRendered(object? sender, RenderedEventArgs e)
    {
        if (this.IsEnabled)
        {
            this.OnRenderedImpl(sender, e);
        }
    }

    /// <inheritdoc cref="OnRendered"/>
    protected abstract void OnRenderedImpl(object? sender, RenderedEventArgs e);
}
