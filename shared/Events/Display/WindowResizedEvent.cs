namespace DaLion.Shared.Events;

#region using directives

using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Wrapper for <see cref="IDisplayEvents.WindowResized"/> allowing dynamic enabling / disabling.</summary>
internal abstract class WindowResizedEvent : ManagedEvent
{
    /// <summary>Initializes a new instance of the <see cref="WindowResizedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    protected WindowResizedEvent(EventManager manager)
        : base(manager)
    {
        manager.ModEvents.Display.WindowResized += this.OnWindowResized;
    }

    /// <inheritdoc />
    public override void Dispose()
    {
        this.Manager.ModEvents.Display.WindowResized -= this.OnWindowResized;
    }

    /// <inheritdoc cref="IDisplayEvents.WindowResized"/>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    internal void OnWindowResized(object? sender, WindowResizedEventArgs e)
    {
        if (this.IsEnabled)
        {
            this.OnWindowResizedImpl(sender, e);
        }
    }

    /// <inheritdoc cref="OnWindowResized"/>
    protected abstract void OnWindowResizedImpl(object? sender, WindowResizedEventArgs e);
}
