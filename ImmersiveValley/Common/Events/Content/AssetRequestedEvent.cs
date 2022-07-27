namespace DaLion.Common.Events;

#region using directives

using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Wrapper for <see cref="IContentEvents.AssetRequested"/> allowing dynamic enabling / disabling.</summary>
internal abstract class AssetRequestedEvent : ManagedEvent
{
    /// <summary>Construct an instance.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    protected AssetRequestedEvent(EventManager manager)
        : base(manager) { }

    /// <inheritdoc cref="IContentEvents.AssetRequested"/>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event data.</param>
    internal void OnAssetRequested(object? sender, AssetRequestedEventArgs e)
    {
        if (IsEnabled) OnAssetRequestedImpl(sender, e);
    }

    /// <inheritdoc cref="OnAssetRequested" />
    protected abstract void OnAssetRequestedImpl(object? sender, AssetRequestedEventArgs e);
}