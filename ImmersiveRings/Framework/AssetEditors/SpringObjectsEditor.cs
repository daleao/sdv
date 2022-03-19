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
        ModEntry.ModHelper.Content.Load<Texture2D>(Path.Combine("assets", "gemrings.png"));

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
            var srcArea = new Rectangle(0, 0, 96, 16);
            var targetArea = new Rectangle(17, 351, 96, 16);

            editor.PatchImage(_tileSheet, srcArea, targetArea);
        }
        else
        {
            throw new InvalidOperationException($"Unexpected asset {asset.AssetName}.");
        }
    }
}