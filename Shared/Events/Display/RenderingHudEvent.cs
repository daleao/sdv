namespace DaLion.Shared.Events;

#region using directives

using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Wrapper for <see cref="IDisplayEvents.RenderingHud"/> allowing dynamic enabling / disabling.</summary>
public abstract class RenderingHudEvent : ManagedEvent
{
    /// <summary>Initializes a new instance of the <see cref="RenderingHudEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    protected RenderingHudEvent(EventManager manager)
        : base(manager)
    {
        manager.ModEvents.Display.RenderingHud += this.OnRenderingHud;
    }

    /// <inheritdoc />
    public override void Dispose()
    {
        this.Manager.ModEvents.Display.RenderingHud -= this.OnRenderingHud;
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc cref="IDisplayEvents.RenderingHud"/>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    public void OnRenderingHud(object? sender, RenderingHudEventArgs e)
    {
        if (this.IsEnabled)
        {
            this.OnRenderingHudImpl(sender, e);
        }
    }

    /// <inheritdoc cref="OnRenderingHud"/>
    protected abstract void OnRenderingHudImpl(object? sender, RenderingHudEventArgs e);
}
