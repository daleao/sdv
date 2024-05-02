namespace DaLion.Shared.Content;

#region using directives

using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Interface for a class which edits an asset.</summary>
public interface IAssetEditor
{
    /// <summary>Edits the asset.</summary>
    /// <param name="e">Event arguments for an <see cref="IContentEvents.AssetRequested"/> event.</param>
    void Edit(AssetRequestedEventArgs e);
}
