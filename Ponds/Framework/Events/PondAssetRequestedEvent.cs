namespace DaLion.Ponds.Framework.Events;

#region using directives

using System.Collections.Generic;
using DaLion.Shared.Constants;
using DaLion.Shared.Content;
using DaLion.Shared.Events;
using StardewModdingAPI.Events;
using StardewValley.GameData.FishPonds;

#endregion using directives

[UsedImplicitly]
[AlwaysEnabledEvent]
internal sealed class PondAssetRequestedEvent : AssetRequestedEvent
{
    /// <summary>Initializes a new instance of the <see cref="PondAssetRequestedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal PondAssetRequestedEvent(EventManager manager)
        : base(manager)
    {
        this.Edit("Data/FishPondData", new AssetEditor(EditFishPondData, AssetEditPriority.Late));
    }

    private static void EditFishPondData(IAssetData asset)
    {
        // patch algae fish data
        var data = (List<FishPondData>)asset.Data;
        data.InsertRange(data.Count - 1, new List<FishPondData>
        {
            new() // seaweed
            {
                Id = Manifest.UniqueID + "/Seaweed",
                PopulationGates =
                    new Dictionary<int, List<string>>
                    {
                        { 4, new List<string> { "368 3" } }, { 7, new List<string> { "369 5" } },
                    },
                ProducedItems =
                    new List<FishPondReward> { new() { Chance = 1f, ItemId = QualifiedObjectIds.Seaweed, MinQuantity = 1, MaxQuantity = 1 }, },
                RequiredTags = new List<string> { "item_seaweed" },
                SpawnTime = 2,
                Precedence = 0,
            },
            new() // green algae
            {
                Id = Manifest.UniqueID + "/GreenAlgae",
                PopulationGates =
                    new Dictionary<int, List<string>>
                    {
                        { 4, new List<string> { "368 3" } }, { 7, new List<string> { "369 5" } },
                    },
                ProducedItems =
                    new List<FishPondReward> { new() { Chance = 1f, ItemId = QualifiedObjectIds.GreenAlgae, MinQuantity = 1, MaxQuantity = 1 }, },
                RequiredTags = new List<string> { "item_green_algae" },
                SpawnTime = 2,
                Precedence = 0,
            },
            new() // white algae
            {
                Id = Manifest.UniqueID + "/WhiteAlgae",
                PopulationGates =
                    new Dictionary<int, List<string>>
                    {
                        { 4, new List<string> { "368 3" } }, { 7, new List<string> { "369 5" } },
                    },
                ProducedItems =
                    new List<FishPondReward> { new() { Chance = 1f, ItemId = QualifiedObjectIds.WhiteAlgae, MinQuantity = 1, MaxQuantity = 1 } },
                RequiredTags = new List<string> { "item_white_algae" },
                SpawnTime = 2,
                Precedence = 0,
            },
        });
    }
}
