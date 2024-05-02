namespace DaLion.Shared.Events;

#region using directives

using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Wrapper for <see cref="IDisplayEvents.RenderedWorld"/> allowing dynamic enabling / disabling.</summary>
public abstract class RenderedWorldEvent : ManagedEvent
{
    /// <summary>Initializes a new instance of the <see cref="RenderedWorldEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    protected RenderedWorldEvent(EventManager manager)
        : base(manager)
    {
        manager.ModEvents.Display.RenderedWorld += this.OnRenderedWorld;
    }

    /// <inheritdoc />
    public override void Dispose()
    {
        this.Manager.ModEvents.Display.RenderedWorld -= this.OnRenderedWorld;
    }

    /// <inheritdoc cref="IDisplayEvents.RenderedWorld"/>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    public void OnRenderedWorld(object? sender, RenderedWorldEventArgs e)
    {
        if (this.IsEnabled)
        {
            this.OnRenderedWorldImpl(sender, e);
        }
    }

    /// <inheritdoc cref="OnRenderedWorld"/>
    protected abstract void OnRenderedWorldImpl(object? sender, RenderedWorldEventArgs e);
}
