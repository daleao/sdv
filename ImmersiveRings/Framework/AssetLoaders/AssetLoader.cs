namespace DaLion.Stardew.Rings.Framework.AssetLoaders;

#region using directives

using System;
using System.IO;
using StardewModdingAPI;

#endregion using directives

/// <summary>Loads custom mod textures.</summary>
public class AssetLoader : IAssetLoader
{
    /// <inheritdoc />
    public bool CanLoad<T>(IAssetInfo asset)
    {
        return asset.AssetName.Contains(ModEntry.Manifest.UniqueID) && asset.AssetName.Contains("IridiumBand");
    }

    /// <inheritdoc />
    public T Load<T>(IAssetInfo asset)
    {
        var textureName = asset.AssetName.Split('/')[1];
        if (textureName == "IridiumBand")
            return ModEntry.ModHelper.Content.Load<T>(Path.Combine("assets", "iridiumband.png"));
        
        throw new InvalidOperationException($"Unexpected asset '{asset.AssetName}'.");
    }
}