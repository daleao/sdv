﻿namespace DaLion.Shared.Events;

#region using directives

using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Wrapper for <see cref="IDisplayEvents.RenderedHud"/> allowing dynamic enabling / disabling.</summary>
public abstract class RenderedHudEvent : ManagedEvent
{
    /// <summary>Initializes a new instance of the <see cref="RenderedHudEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    protected RenderedHudEvent(EventManager manager)
        : base(manager)
    {
        manager.ModEvents.Display.RenderedHud += this.OnRenderedHud;
    }

    /// <inheritdoc />
    public override void Dispose()
    {
        this.Manager.ModEvents.Display.RenderedHud -= this.OnRenderedHud;
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc cref="OnRenderedHud"/>
    protected abstract void OnRenderedHudImpl(object? sender, RenderedHudEventArgs e);

    /// <inheritdoc cref="IDisplayEvents.RenderedHud"/>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    private void OnRenderedHud(object? sender, RenderedHudEventArgs e)
    {
        if (this.IsEnabled)
        {
            this.OnRenderedHudImpl(sender, e);
        }
    }
}
