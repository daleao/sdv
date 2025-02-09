namespace DaLion.Professions.Framework.Events.World.BuildingListChanged;

#region using directives

using DaLion.Shared.Events;
using Shared.Extensions;
using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Initializes a new instance of the <see cref="PrestigedBreederBuildingListChangedEvent"/> class.</summary>
/// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
[UsedImplicitly]
internal sealed class PrestigedBreederBuildingListChangedEvent(EventManager? manager = null)
    : BuildingListChangedEvent(manager ?? ProfessionsMod.EventManager)
{
    /// <inheritdoc />
    public override bool IsEnabled => Game1.game1.DoesAnyPlayerHaveProfession(Profession.Breeder, true, true);

    /// <inheritdoc />
    protected override void OnBuildingListChangedImpl(object? sender, BuildingListChangedEventArgs e)
    {
        foreach (var building in e.Added)
        {
            if (building.indoors.Value is AnimalHouse house && house.Name.Contains("Barn") &&
                house.Name.ContainsAnyOf("Deluxe", "Premium"))
            {
                house.animalLimit.Value += 2;
            }
        }
    }
}
