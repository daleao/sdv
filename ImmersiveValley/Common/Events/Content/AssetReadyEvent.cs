namespace DaLion.Common.Events;

#region using directives

using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Wrapper for <see cref="IContentEvents.AssetReady"/> allowing dynamic hooking / unhooking.</summary>
internal abstract class AssetReadyEvent : ManagedEvent
{
    /// <summary>Construct an instance.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    protected AssetReadyEvent(EventManager manager)
        : base(manager) { }

    /// <inheritdoc cref="IContentEvents.AssetReady"/>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event data.</param>
    internal void OnAssetReady(object? sender, AssetReadyEventArgs e)
    {
        if (IsHooked) OnAssetReadyImpl(sender, e);
    }

    /// <inheritdoc cref="OnAssetReady" />
    protected abstract void OnAssetReadyImpl(object? sender, AssetReadyEventArgs e);
}