namespace DaLion.Common.Events;

#region using directives

using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Wrapper for <see cref="IContentEvents.AssetsInvalidated"/> allowing dynamic enabling / disabling.</summary>
internal abstract class AssetsInvalidatedEvent : ManagedEvent
{
    /// <summary>Construct an instance.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    protected AssetsInvalidatedEvent(EventManager manager)
        : base(manager) { }

    /// <inheritdoc cref="IContentEvents.AssetsInvalidated"/>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event data.</param>
    internal void OnAssetsInvalidated(object? sender, AssetsInvalidatedEventArgs e)
    {
        if (IsEnabled) OnAssetsInvalidatedImpl(sender, e);
    }

    /// <inheritdoc cref="OnAssetsInvalidated" />
    protected abstract void OnAssetsInvalidatedImpl(object? sender, AssetsInvalidatedEventArgs e);
}