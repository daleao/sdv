namespace DaLion.Common.Events;

#region using directives

using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Wrapper for <see cref="IContentEvents.AssetRequested"/> allowing dynamic hooking / unhooking.</summary>
internal abstract class AssetRequestedEvent : BaseEvent
{
    /// <inheritdoc cref="IContentEvents.AssetRequested"/>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event data.</param>
    internal void OnAssetRequested(object sender, AssetRequestedEventArgs e)
    {
        if (hooked.Value || GetType().Name.StartsWith("Static")) OnAssetRequestedImpl(sender, e);
    }

    /// <inheritdoc cref="OnAssetRequested" />
    protected abstract void OnAssetRequestedImpl(object sender, AssetRequestedEventArgs e);
}