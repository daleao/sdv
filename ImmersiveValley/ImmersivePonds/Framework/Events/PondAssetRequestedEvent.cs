namespace DaLion.Stardew.Ponds.Framework.Events;

#region using directives

using System.Collections.Generic;
using DaLion.Common.Events;
using StardewModdingAPI.Events;
using StardewValley.GameData.FishPond;

#endregion using directives

[UsedImplicitly]
internal sealed class PondAssetRequestedEvent : AssetRequestedEvent
{
    /// <summary>Initializes a new instance of the <see cref="PondAssetRequestedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal PondAssetRequestedEvent(EventManager manager)
        : base(manager)
    {
        this.AlwaysEnabled = true;
    }

    /// <inheritdoc />
    protected override void OnAssetRequestedImpl(object? sender, AssetRequestedEventArgs e)
    {
        if (e.NameWithoutLocale.IsEquivalentTo("Data/FishPondData"))
        {
            e.Edit(EditFishPondData, AssetEditPriority.Late);
        }
    }

    private static void EditFishPondData(IAssetData asset)
    {
        // patch algae fish data
        var data = (List<FishPondData>)asset.Data;
        data.InsertRange(data.Count - 1, new List<FishPondData>
        {
            new() // seaweed
            {
                PopulationGates =
                    new Dictionary<int, List<string>>
                    {
                        { 4, new List<string> { "368 3" } }, { 7, new List<string> { "369 5" } },
                    },
                ProducedItems =
                    new List<FishPondReward> { new() { Chance = 1f, ItemID = 152, MinQuantity = 1, MaxQuantity = 1 }, },
                RequiredTags = new List<string> { "item_seaweed" },
                SpawnTime = 2,
            },
            new() // green algae
            {
                PopulationGates =
                    new Dictionary<int, List<string>>
                    {
                        { 4, new List<string> { "368 3" } }, { 7, new List<string> { "369 5" } },
                    },
                ProducedItems =
                    new List<FishPondReward> { new() { Chance = 1f, ItemID = 153, MinQuantity = 1, MaxQuantity = 1 }, },
                RequiredTags = new List<string> { "item_green_algae" },
                SpawnTime = 2,
            },
            new() // white algae
            {
                PopulationGates =
                    new Dictionary<int, List<string>>
                    {
                        { 4, new List<string> { "368 3" } }, { 7, new List<string> { "369 5" } },
                    },
                ProducedItems =
                    new List<FishPondReward> { new() { Chance = 1f, ItemID = 157, MinQuantity = 1, MaxQuantity = 1 } },
                RequiredTags = new List<string> { "item_white_algae" },
                SpawnTime = 2,
            },
        });
    }
}
