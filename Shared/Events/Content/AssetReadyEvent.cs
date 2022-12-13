namespace DaLion.Shared.Events;

#region using directives

using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Wrapper for <see cref="IContentEvents.AssetReady"/> allowing dynamic enabling / disabling.</summary>
internal abstract class AssetReadyEvent : ManagedEvent
{
    /// <summary>Initializes a new instance of the <see cref="AssetReadyEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    protected AssetReadyEvent(EventManager manager)
        : base(manager)
    {
        manager.ModEvents.Content.AssetReady += this.OnAssetReady;
    }

    /// <inheritdoc />
    public override void Dispose()
    {
        this.Manager.ModEvents.Content.AssetReady -= this.OnAssetReady;
    }

    /// <inheritdoc cref="IContentEvents.AssetReady"/>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event data.</param>
    internal void OnAssetReady(object? sender, AssetReadyEventArgs e)
    {
        if (this.IsEnabled)
        {
            this.OnAssetReadyImpl(sender, e);
        }
    }

    /// <inheritdoc cref="OnAssetReady"/>
    protected abstract void OnAssetReadyImpl(object? sender, AssetReadyEventArgs e);
}
