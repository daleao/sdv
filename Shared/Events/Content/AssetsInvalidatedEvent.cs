﻿namespace DaLion.Shared.Events;

#region using directives

using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Wrapper for <see cref="IContentEvents.AssetsInvalidated"/> allowing dynamic enabling / disabling.</summary>
public abstract class AssetsInvalidatedEvent : ManagedEvent
{
    /// <summary>Initializes a new instance of the <see cref="AssetsInvalidatedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    protected AssetsInvalidatedEvent(EventManager manager)
        : base(manager)
    {
        manager.ModEvents.Content.AssetsInvalidated += this.OnAssetsInvalidated;
    }

    /// <inheritdoc />
    public override void Dispose()
    {
        this.Manager.ModEvents.Content.AssetsInvalidated -= this.OnAssetsInvalidated;
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc cref="OnAssetsInvalidated"/>
    protected abstract void OnAssetsInvalidatedImpl(object? sender, AssetsInvalidatedEventArgs e);

    /// <inheritdoc cref="IContentEvents.AssetsInvalidated"/>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event data.</param>
    private void OnAssetsInvalidated(object? sender, AssetsInvalidatedEventArgs e)
    {
        if (this.IsEnabled)
        {
            this.OnAssetsInvalidatedImpl(sender, e);
        }
    }
}
