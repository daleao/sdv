namespace DaLion.Stardew.Rings.Framework.AssetEditors;

#region using directives

using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;

#endregion using directives

/// <summary>Edits SpringObjects sprites.</summary>
public class SpringObjectsEditor : IAssetEditor
{
    private readonly Texture2D _tileSheet =
        ModEntry.ModHelper.Content.Load<Texture2D>(Path.Combine("assets", "rings.png"));

    /// <inheritdoc />
    public bool CanEdit<T>(IAssetInfo asset)
    {
        return asset.AssetNameEquals(PathUtilities.NormalizeAssetName("Maps/springobjects"));
    }

    /// <inheritdoc />
    public void Edit<T>(IAssetData asset)
    {
        if (asset.AssetNameEquals(PathUtilities.NormalizeAssetName("Maps/springobjects")))
        {
            var editor = asset.AsImage();
            Rectangle srcArea, targetArea;

            if (ModEntry.Config.CraftableGemRings)
            {
                srcArea = new(18, 0, 88, 12);
                targetArea = new(21, 353, 88, 12);
                editor.PatchImage(_tileSheet, srcArea, targetArea);
            }

            if (ModEntry.Config.ForgeableIridiumBand)
            {
                srcArea = new(0, 2, 12, 12);
                targetArea = new(371, 339, 12, 12);
                editor.PatchImage(_tileSheet, srcArea, targetArea);
            }
        }
        else
        {
            throw new InvalidOperationException($"Unexpected asset {asset.AssetName}.");
        }
    }
}