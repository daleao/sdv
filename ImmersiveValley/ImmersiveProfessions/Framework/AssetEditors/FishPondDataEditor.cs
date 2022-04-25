namespace DaLion.Stardew.Professions.Framework.AssetEditors;

#region using directives

using System;
using System.Collections.Generic;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using StardewValley.GameData.FishPond;

#endregion using directives

/// <summary>Edits FishPondData for legendary fish.</summary>
public class FishPondDataEditor : IAssetEditor
{
    /// <inheritdoc />
    public bool CanEdit<T>(IAssetInfo asset)
    {
        return asset.AssetNameEquals(PathUtilities.NormalizeAssetName("Data/FishPondData"));
    }

    /// <inheritdoc />
    public void Edit<T>(IAssetData asset)
    {
        if (!asset.AssetNameEquals(PathUtilities.NormalizeAssetName("Data/FishPondData")))
            throw new InvalidOperationException($"Unexpected asset {asset.AssetName}.");

        // patch legendary fish data
        var data = (List<FishPondData>) asset.Data;
        data.InsertRange(data.Count - 2, new List<FishPondData>()
        {
            new() // legendary fish
            {
                PopulationGates = null,
                ProducedItems = new()
                {
                    new()
                    {
                        Chance = 1f,
                        ItemID = 812, // roe
                        MinQuantity = 1,
                        MaxQuantity = 1
                    }
                },
                RequiredTags = new() {"fish_legendary"},
                SpawnTime = int.MaxValue
            }
        });
    }
}