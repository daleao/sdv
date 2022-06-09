namespace DaLion.Stardew.Professions.Framework.Events.Content;

#region using directives

using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Wrapper for <see cref="IContentEvents.AssetsInvalidated"/> allowing dynamic enabling / disabling.</summary>
internal abstract class AssetsInvalidatedEvent : BaseEvent
{
    /// <inheritdoc cref="IContentEvents.AssetsInvalidated"/>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event data.</param>
    internal void OnAssetsInvalidated(object sender, AssetsInvalidatedEventArgs e)
    {
        if (enabled.Value || GetType().Name.StartsWith("Static")) OnAssetsInvalidatedImpl(sender, e);
    }

    /// <inheritdoc cref="OnAssetsInvalidated" />
    protected abstract void OnAssetsInvalidatedImpl(object sender, AssetsInvalidatedEventArgs e);
}