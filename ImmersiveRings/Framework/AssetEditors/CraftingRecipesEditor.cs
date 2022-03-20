namespace DaLion.Stardew.Rings.Framework.AssetEditors;

#region using directives

using System;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;

#endregion using directives

/// <summary>Edits CraftingRecipes data.</summary>
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

            string[] fields;
            if (ModEntry.Config.ImmersiveGlowstoneRecipe)
            {
                fields = data["Glowstone Ring"].Split('/');
                fields[0] = "517 1 519 1 768 20 769 20";
                data["Glowstone Ring"] = string.Join('/', fields);

            }

            if (ModEntry.Config.CraftableGlowAndMagnetRings)
            {
                data["Glow Ring"] = "516 2 768 10/Home/517/Ring/Mining 2";
                data["Magnet Ring"] = "518 2 769 10/Home/519/Ring/Mining 2";
            }

            if (ModEntry.Config.CraftableGemRings)
            {
                data["Amethyst Ring"] = "66 1 334 5/Home/529/Ring/Combat 2";
                data["Topaz Ring"] = "68 1 334 5/Home/530/Ring/Combat 2";
                data["Aquamarine Ring"] = "62 1 335 5/Home/531/Ring/Combat 4";
                data["Jade Ring"] = "70 1 335 5/Home/532/Ring/Combat 4";
                data["Emerald Ring"] = "60 1 336 5/Home/533/Ring/Combat 6";
                data["Ruby Ring"] = "64 1 336 5/Home/534/Ring/Combat 6";
            }

            if (ModEntry.Config.ForgeableIridiumBand)
            {
                fields = data["Iridium Band"].Split('/');
                fields[0] = "337 5 768 100 769 100";
                data["Iridium Band"] = string.Join('/', fields);
            }
        }
        else
        {
            throw new InvalidOperationException($"Unexpected asset {asset.AssetName}.");
        }
    }
}