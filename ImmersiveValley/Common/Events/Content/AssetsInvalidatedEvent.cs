namespace DaLion.Common.Events;

#region using directives

using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Wrapper for <see cref="IContentEvents.AssetsInvalidated"/> allowing dynamic hooking / unhooking.</summary>
internal abstract class AssetsInvalidatedEvent : BaseEvent
{
    /// <inheritdoc cref="IContentEvents.AssetsInvalidated"/>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event data.</param>
    internal void OnAssetsInvalidated(object sender, AssetsInvalidatedEventArgs e)
    {
        if (hooked.Value || GetType().Name.StartsWith("Static")) OnAssetsInvalidatedImpl(sender, e);
    }

    /// <inheritdoc cref="OnAssetsInvalidated" />
    protected abstract void OnAssetsInvalidatedImpl(object sender, AssetsInvalidatedEventArgs e);
}