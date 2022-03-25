namespace DaLion.Stardew.FishPonds.Framework;

#region using directives

using System;
using System.Collections.Generic;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using StardewValley.GameData.FishPond;

#endregion using directives

/// <summary>Edits FishPondData for algae and seaweed.</summary>
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

        // patch algae fish data
        var data = (List<FishPondData>) asset.Data;
        data.InsertRange(data.Count - 2, new List<FishPondData>()
        {
            new() // seaweed
            {
                PopulationGates = new()
                {
                    {4, new(){"368 3"}},
                    {7, new(){"369 5"}}
                },
                ProducedItems = new()
                {
                    new()
                    {
                        Chance = 1f,
                        ItemID = 152,
                        MinQuantity = 1,
                        MaxQuantity = 1
                    }
                },
                RequiredTags = new() {"item_seaweed"},
                SpawnTime = 2
            },
            new() // green algae
            {
                PopulationGates = new()
                {
                    {4, new(){"368 3"}},
                    {7, new(){"369 5"}}
                },
                ProducedItems = new()
                {
                    new()
                    {
                        Chance = 1f,
                        ItemID = 153,
                        MinQuantity = 1,
                        MaxQuantity = 1
                    }
                },
                RequiredTags = new() {"item_green_algae"},
                SpawnTime = 2
            },
            new() // white algae
            {
                PopulationGates = new()
                {
                    {4, new(){"368 3"}},
                    {7, new(){"369 5"}}
                },
                ProducedItems = new()
                {
                    new()
                    {
                        Chance = 1f,
                        ItemID = 157,
                        MinQuantity = 1,
                        MaxQuantity = 1
                    }
                },
                RequiredTags = new() {"item_white_algae"},
                SpawnTime = 2
            }
        });
    }
}