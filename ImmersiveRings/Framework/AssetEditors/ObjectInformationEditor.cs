namespace DaLion.Stardew.Rings.Framework.AssetEditors;

#region using directives

using System;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;

#endregion using directives

/// <summary>Edits ObjectInformation data.</summary>
public class ObjectInformationEditor : IAssetEditor
{
    /// <inheritdoc />
    public bool CanEdit<T>(IAssetInfo asset)
    {
        return asset.AssetNameEquals(PathUtilities.NormalizeAssetName("Data/ObjectInformation"));
    }

    /// <inheritdoc />
    public void Edit<T>(IAssetData asset)
    {
        if (asset.AssetNameEquals(PathUtilities.NormalizeAssetName("Data/ObjectInformation")))
        {
            var data = asset.AsDictionary<int, string>().Data;

            string[] fields;
            if (ModEntry.Config.ForgeIridiumBand)
            {
                fields = data[Framework.Constants.IRIDIUM_BAND_INDEX_I].Split('/');
                fields[5] = ModEntry.ModHelper.Translation.Get("rings.iridium");
                data[Framework.Constants.IRIDIUM_BAND_INDEX_I] = string.Join('/', fields);
            }
        }
        else
        {
            throw new InvalidOperationException($"Unexpected asset {asset.AssetName}.");
        }
    }
}