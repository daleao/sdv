namespace DaLion.Stardew.Tweaks.Framework;

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
        return asset.AssetName.Contains(ModEntry.Manifest.UniqueID) && asset.AssetName.Contains("BetterHoneyMeadIcons");
    }

    /// <inheritdoc />
    public T Load<T>(IAssetInfo asset)
    {
        var assetName = asset.AssetName.Split('/')[1];
        if (assetName == "BetterHoneyMeadIcons")
            return ModEntry.ModHelper.Content.Load<T>(Path.Combine("assets",
                "mead-" + ModEntry.Config.HoneyMeadStyle.ToLower() + ".png"));
        
        throw new InvalidOperationException($"Unexpected asset '{assetName}'.");
    }
}