namespace DaLion.Professions.Framework.Events.GameLoop.DayStarted;

#region using directives

using DaLion.Shared.Events;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;
using StardewValley.Buildings;

#endregion using directives

[UsedImplicitly]
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
            if (b is FishPond pond)
            {
                pond.UpdateMaximumOccupancy();
                return true;
            }

            var indoors = b.GetIndoors();
            switch (indoors)
            {
                case AnimalHouse barn when barn.Name.Contains("Barn"):
                    if (areThereAnyPrestigedBreeders && barn.animalLimit.Value == 12)
                    {
                        barn.animalLimit.Value = 14;
                        if (barn.Objects.TryGetValue(new Vector2(6, 3), out var hopper))
                        {
                            barn.Objects.Remove(hopper.TileLocation);
                            hopper.TileLocation = new Vector2(4, 3);
                            barn.Objects[hopper.TileLocation] = hopper;
                        }
                    }
                    else if (!areThereAnyPrestigedBreeders && barn.animalLimit.Value == 14)
                    {
                        barn.animalLimit.Value = 12;
                        {
                            if (barn.Objects.TryGetValue(new Vector2(4, 3), out var hopper))
                            {
                                barn.Objects.Remove(hopper.TileLocation);
                                hopper.TileLocation = new Vector2(6, 3);
                                barn.Objects[hopper.TileLocation] = hopper;
                            }
                        }
                    }

                    ModHelper.GameContent.InvalidateCache("Maps/Barn3");
                    break;

                case AnimalHouse coop when coop.Name.Contains("Coop"):
                    coop.animalLimit.Value = areThereAnyPrestigedProducers ? 14 : 12;
                    ModHelper.GameContent.InvalidateCache("Maps/Coop3");
                    break;

                case SlimeHutch hutch:
                    if (areThereAnyPrestigedPipers)
                    {
                        Reflector
                            .GetUnboundFieldSetter<SlimeHutch, int>(hutch, "_slimeCapacity")
                            .Invoke(hutch, 30);
                        hutch.waterSpots.SetCount(6);
                    }
                    else
                    {
                        Reflector
                            .GetUnboundFieldSetter<SlimeHutch, int>(hutch, "_slimeCapacity")
                            .Invoke(hutch, 20);
                        hutch.waterSpots.SetCount(4);
                    }

                    ModHelper.GameContent.InvalidateCache("Maps/SlimeHutch");
                    break;
            }

            return true;
        });

        this.Disable();
    }
}
