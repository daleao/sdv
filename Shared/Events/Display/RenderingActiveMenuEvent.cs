namespace DaLion.Shared.Events;

#region using directives

using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Wrapper for <see cref="IDisplayEvents.RenderingActiveMenu"/> allowing dynamic enabling / disabling.</summary>
public abstract class RenderingActiveMenuEvent : ManagedEvent
{
    /// <summary>Initializes a new instance of the <see cref="RenderingActiveMenuEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    protected RenderingActiveMenuEvent(EventManager manager)
        : base(manager)
    {
        manager.ModEvents.Display.RenderingActiveMenu += this.OnRenderingActiveMenu;
    }

    /// <inheritdoc />
    public override void Dispose()
    {
        this.Manager.ModEvents.Display.RenderingActiveMenu -= this.OnRenderingActiveMenu;
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc cref="OnRenderingActiveMenu"/>
    protected abstract void OnRenderingActiveMenuImpl(object? sender, RenderingActiveMenuEventArgs e);

    /// <inheritdoc cref="IDisplayEvents.RenderingActiveMenu"/>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    private void OnRenderingActiveMenu(object? sender, RenderingActiveMenuEventArgs e)
    {
        if (this.IsEnabled)
        {
            this.OnRenderingActiveMenuImpl(sender, e);
        }
    }
}
