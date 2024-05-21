namespace DaLion.Professions.Framework.Events.World.BuildingListChanged;

#region using directives

using DaLion.Shared.Events;
using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Initializes a new instance of the <see cref="PrestigedProducerBuildingListChangedEvent"/> class.</summary>
/// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
[UsedImplicitly]
internal sealed class PrestigedProducerBuildingListChangedEvent(EventManager? manager = null)
    : BuildingListChangedEvent(manager ?? ProfessionsMod.EventManager)
{
    /// <inheritdoc />
    public override bool IsEnabled => Game1.game1.DoesAnyPlayerHaveProfession(Profession.Producer, true, true);

    /// <inheritdoc />
    protected override void OnBuildingListChangedImpl(object? sender, BuildingListChangedEventArgs e)
    {
        foreach (var building in e.Added)
        {
            if (building.indoors.Value is AnimalHouse { Name: "Deluxe Coop" } house)
            {
                house.animalLimit.Value += 2;
            }
        }
    }
}
