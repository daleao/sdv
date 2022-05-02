namespace DaLion.Stardew.Professions.Framework.Events.GameLoop;

#region using directives

using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Wrapper for <see cref="IContentEvents.AssetRequested"/> allowing dynamic enabling / disabling.</summary>
internal abstract class AssetRequestedEvent : BaseEvent
{
    /// <inheritdoc cref="IContentEvents.AssetRequested"/>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event data.</param>
    public void OnAssetRequested(object sender, AssetRequestedEventArgs e)
    {
        if (enabled.Value || GetType().Name.StartsWith("Static")) OnAssetRequestedImpl(sender, e);
    }

    /// <inheritdoc cref="OnAssetRequested" />
    protected abstract void OnAssetRequestedImpl(object sender, AssetRequestedEventArgs e);
}