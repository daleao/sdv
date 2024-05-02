namespace DaLion.Professions.Framework.Events.GameLoop.DayStarted;

#region using directives

using DaLion.Shared.Events;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;
using StardewValley.Buildings;

#endregion using directives

[UsedImplicitly]
[AlwaysEnabledEvent]
internal sealed class RevalidateBuildingsDayStartedEvent : DayStartedEvent
{
    /// <summary>Initializes a new instance of the <see cref="RevalidateBuildingsDayStartedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal RevalidateBuildingsDayStartedEvent(EventManager? manager = null)
        : base(manager ?? ProfessionsMod.EventManager)
    {
    }

    /// <inheritdoc />
    protected override void OnDayStartedImpl(object? sender, DayStartedEventArgs e)
    {
        bool areThereAnyPrestigedBreeders = false,
            areThereAnyPrestigedProducers = false,
            areThereAnyPrestigedPipers = false;
        foreach (var farmer in Game1.getAllFarmers())
        {
            if (farmer.HasProfession(Profession.Breeder, true))
            {
                areThereAnyPrestigedBreeders = true;
            }
            else if (farmer.HasProfession(Profession.Producer, true))
            {
                areThereAnyPrestigedProducers = true;
            }

            if (farmer.HasProfession(Profession.Piper, true))
            {
                areThereAnyPrestigedPipers = true;
            }
        }

        Utility.ForEachBuilding(b =>
        {
            if (b.isUnderConstruction())
            {
                return true;
            }

            if (b is FishPond pond)
            {
                pond.UpdateMaximumOccupancy();
                return true;
            }

            var indoors = b.GetIndoors();
            switch (indoors)
            {
                case AnimalHouse { Name: "Deluxe Barn" } barn:
                    if (areThereAnyPrestigedBreeders)
                    {
                        barn.animalLimit.Value = 14;
                        barn.Objects.Values
                            .First(o => o.QualifiedItemId == QualifiedBigCraftableIds.FeedHopper)
                            .TileLocation = new Vector2(4, 3);
                    }
                    else
                    {
                        barn.animalLimit.Value = 12;
                        barn.Objects.Values
                            .First(o => o.QualifiedItemId == QualifiedBigCraftableIds.FeedHopper)
                            .TileLocation = new Vector2(6, 3);
                    }

                    ModHelper.GameContent.InvalidateCache("Maps/Barn3");
                    break;

                case AnimalHouse { Name: "Deluxe Coop" } coop:
                    coop.animalLimit.Value = areThereAnyPrestigedProducers ? 14 : 12;
                    ModHelper.GameContent.InvalidateCache("Maps/Coop3");
                    break;

                case SlimeHutch hutch:
                    if (areThereAnyPrestigedPipers)
                    {
                        var capacity = Reflector
                            .GetUnboundFieldGetter<SlimeHutch, int>(hutch, "_slimeCapacity")
                            .Invoke(hutch);
                        Reflector
                            .GetUnboundFieldSetter<SlimeHutch, int>(hutch, "_slimeCapacity")
                            .Invoke(hutch, (int)(capacity * 1.5f));
                        hutch.waterSpots.SetCount(6);
                    }
                    else
                    {
                        Reflector
                            .GetUnboundFieldSetter<SlimeHutch, int>(hutch, "_slimeCapacity")
                            .Invoke(hutch, -1);
                        hutch.waterSpots.SetCount(4);
                    }

                    break;
            }

            return true;
        });

        this.Disable();
    }
}
