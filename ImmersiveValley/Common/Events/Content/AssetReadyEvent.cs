namespace DaLion.Common.Events;

#region using directives

using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Wrapper for <see cref="IContentEvents.AssetReady"/> allowing dynamic hooking / unhooking.</summary>
internal abstract class AssetReadyEvent : BaseEvent
{
    /// <inheritdoc cref="IContentEvents.AssetReady"/>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event data.</param>
    internal void OnAssetReady(object sender, AssetReadyEventArgs e)
    {
        if (hooked.Value || GetType().Name.StartsWith("Static")) OnAssetReadyImpl(sender, e);
    }

    /// <inheritdoc cref="OnAssetReady" />
    protected abstract void OnAssetReadyImpl(object sender, AssetReadyEventArgs e);
}