namespace DaLion.Stardew.Tweaks.Framework.AssetEditors;

#region using directives

using System;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;

#endregion using directives

public class CraftingRecipesEditor : IAssetEditor
{
    /// <inheritdoc />
    public bool CanEdit<T>(IAssetInfo asset)
    {
        return asset.AssetNameEquals(PathUtilities.NormalizeAssetName("Data/CraftingRecipes"));
    }

    /// <inheritdoc />
    public void Edit<T>(IAssetData asset)
    {
        if (asset.AssetNameEquals(PathUtilities.NormalizeAssetName("Data/CraftingRecipes")))
        {
            var data = asset.AsDictionary<string, string>().Data;
            
            var fields = data["Glowstone Ring"].Split('/');
            fields[0] = "517 1 519 1 768 20 769 20";
            data["Glowstone Ring"] = string.Join('/', fields);

            data["Glow Ring"] = "516 2 768 5/Home/517/Ring/Mining 2";
            data["Magnet Ring"] = "518 2 769 5/Home/519/Ring/Mining 2";
            data["Amethyst Ring"] = "529 1 334 1/Home/519/Ring/Mining 2";
            data["Topaz Ring"] = "530 1 334 1/Home/519/Ring/Mining 2";
            data["Aquamarine Ring"] = "531 1 335 1/Home/519/Ring/Mining 2";
            data["Jade Ring"] = "532 1 335 1/Home/519/Ring/Mining 2";
            data["Emerald Ring"] = "533 1 336 1/Home/519/Ring/Mining 2";
            data["Ruby Ring"] = "534 1 336 1/Home/519/Ring/Mining 2";
        }
        else
        {
            throw new InvalidOperationException($"Unexpected asset {asset.AssetName}.");
        }
    }
}