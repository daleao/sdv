namespace DaLion.Stardew.Ponds.Framework.Events;

#region using directives

using System.Collections.Generic;
using StardewValley.GameData.FishPond;
using JetBrains.Annotations;
using StardewModdingAPI.Events;

using Common.Events;

#endregion using directives

[UsedImplicitly]
internal sealed class PondAssetRequestedEvent : AssetRequestedEvent
{
    /// <inheritdoc />
    protected override void OnAssetRequestedImpl(object sender, AssetRequestedEventArgs e)
    {
        if (!e.NameWithoutLocale.IsEquivalentTo("Data/FishPondData")) return;

        e.Edit(asset =>
        {
            // patch algae fish data
            var data = (List<FishPondData>)asset.Data;
            data.InsertRange(data.Count - 2, new List<FishPondData>
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
        });
    }
}