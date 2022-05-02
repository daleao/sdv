namespace DaLion.Stardew.Professions.Framework.Events.Content.AssetRequested;

#region using directives

using System.Collections.Generic;
using JetBrains.Annotations;
using StardewModdingAPI.Events;
using StardewValley.GameData.FishPond;

using GameLoop;

#endregion using directives

[UsedImplicitly]
internal class FishPondDataRequestedEvent : AssetRequestedEvent
{
    /// <inheritdoc />
    protected override void OnAssetRequestedImpl(object sender, AssetRequestedEventArgs e)
    {
        if (!e.NameWithoutLocale.IsEquivalentTo("Data/FishPondData")) return;

        e.Edit(asset =>
        {
            // patch legendary fish data
            var data = (List<FishPondData>)asset.Data;
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
        });
    }
}