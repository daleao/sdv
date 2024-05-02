namespace DaLion.Ponds.Framework.Events;

#region using directives

using DaLion.Shared.Events;
using DaLion.Shared.Extensions;
using DaLion.Shared.Extensions.Stardew;
using StardewModdingAPI.Events;
using StardewValley.Buildings;

#endregion using directives

[UsedImplicitly]
[AlwaysEnabledEvent]
internal sealed class PondSaveLoadedEvent : SaveLoadedEvent
{
    /// <summary>Initializes a new instance of the <see cref="PondSaveLoadedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal PondSaveLoadedEvent(EventManager manager)
        : base(manager)
    {
    }

    public override bool IsEnabled => Context.IsMainPlayer;

    /// <inheritdoc />
    protected override void OnSaveLoadedImpl(object? sender, SaveLoadedEventArgs e)
    {
        var buildings = Game1.getFarm().buildings;
        for (var i = 0; i < buildings.Count; i++)
        {
            var building = buildings[i];
            if (building is not FishPond pond || pond.isUnderConstruction() ||
                !pond.fishType.Value.ContainsAnyOf("160", "899") ||
                Data.ReadAs<int>(pond, DataKeys.FamilyLivingHere, modId: "DaLion.Professions") is not (var familyCount and > 0))
            {
                continue;
            }

            var data = pond.GetFishPondData();
            {
                if (data is null)
                {
                    break;
                }
            }

            var mates = Math.Min(pond.FishCount - familyCount, familyCount);
            data.SpawnTime = 12 / mates;
        }
    }
}
