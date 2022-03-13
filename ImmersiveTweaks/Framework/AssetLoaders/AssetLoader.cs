namespace DaLion.Stardew.Tweaks.Framework.AssetLoaders;

#region using directives

using System;
using System.IO;
using StardewModdingAPI;
using StardewValley;

using Common.Extensions;

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
        var textureName = asset.AssetName.Split('/')[1];
        if (textureName == "BetterHoneyMeadIcons")
            return ModEntry.ModHelper.Content.Load<T>(Path.Combine("assets", "sprites",
                "mead-" + ModEntry.Config.HoneyMeadStyle.ToLower() + ".png"));
        else
            throw new InvalidOperationException($"Unexpected asset '{asset.AssetName}'.");
    }
}