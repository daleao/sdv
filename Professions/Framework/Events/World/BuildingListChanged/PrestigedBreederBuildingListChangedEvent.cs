namespace DaLion.Professions.Framework.Events.World.BuildingListChanged;

#region using directives

using DaLion.Shared.Events;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
internal sealed class PrestigedBreederBuildingListChangedEvent : BuildingListChangedEvent
{
    /// <summary>Initializes a new instance of the <see cref="PrestigedBreederBuildingListChangedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal PrestigedBreederBuildingListChangedEvent(EventManager? manager = null)
        : base(manager ?? ProfessionsMod.EventManager)
    {
    }

    /// <inheritdoc />
    public override bool IsEnabled => Game1.game1.DoesAnyPlayerHaveProfession(Profession.Breeder, true, true);

    /// <inheritdoc />
    protected override void OnBuildingListChangedImpl(object? sender, BuildingListChangedEventArgs e)
    {
        foreach (var building in e.Added)
        {
            if (building.indoors.Value is AnimalHouse { Name: "Deluxe Barn" } house)
            {
                house.animalLimit.Value += 2;
            }
        }
    }
}
