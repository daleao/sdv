namespace DaLion.Shared.Content;

#region using directives

using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Interface for a class which provides an asset.</summary>
public interface IAssetProvider
{
    /// <summary>Provides the asset.</summary>
    /// <param name="e">Event arguments for an <see cref="IContentEvents.AssetRequested"/> event.</param>
    void Provide(AssetRequestedEventArgs e);
}
