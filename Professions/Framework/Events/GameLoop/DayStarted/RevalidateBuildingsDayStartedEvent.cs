namespace DaLion.Professions.Framework.Events.GameLoop.DayStarted;

#region using directives

using DaLion.Shared.Events;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;
using StardewValley.Buildings;
using StardewValley.Monsters;

#endregion using directives

/// <summary>Initializes a new instance of the <see cref="RevalidateBuildingsDayStartedEvent"/> class.</summary>
/// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
[UsedImplicitly]
internal sealed class RevalidateBuildingsDayStartedEvent(EventManager? manager = null)
    : DayStartedEvent(manager ?? ProfessionsMod.EventManager)
{
    /// <inheritdoc />
    protected override void OnDayStartedImpl(object? sender, DayStartedEventArgs e)
    {
        bool areThereAnyPrestigedBreeders = false,
            areThereAnyPrestigedProducers = false,
            areThereAnyPipers = false;
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

            if (farmer.HasProfession(Profession.Piper))
            {
                areThereAnyPipers = true;
            }
        }

        Utility.ForEachBuilding(b =>
        {
            if (b is FishPond pond)
            {
                pond.UpdateMaximumOccupancy();
                return true; // continue enumeration
            }

            var indoors = b.GetIndoors();
            switch (indoors)
            {
                case AnimalHouse barn when barn.Name.Contains("Barn"):
                    switch (areThereAnyPrestigedBreeders)
                    {
                        case true when barn.Name.Contains("Deluxe") || barn.animalLimit.Value == 12:
                        {
                            barn.animalLimit.Value = 14;
                            if (barn.Objects.TryGetValue(new Vector2(6, 3), out var hopper))
                            {
                                barn.Objects.Remove(hopper.TileLocation);
                                hopper.TileLocation = new Vector2(4, 3);
                                barn.Objects[hopper.TileLocation] = hopper;
                            }

                            break;
                        }

                        case true when barn.Name.Contains("Premium") || barn.animalLimit.Value == 16:
                        {
                            barn.animalLimit.Value = 18;
                            if (barn.Objects.TryGetValue(new Vector2(4, 4), out var hopper))
                            {
                                barn.Objects.Remove(hopper.TileLocation);
                                hopper.TileLocation = new Vector2(2, 5);
                                barn.Objects[hopper.TileLocation] = hopper;
                            }

                            break;
                        }

                        case false when barn.Name.Contains("Deluxe") || barn.animalLimit.Value == 14:
                        {
                            barn.animalLimit.Value = 12;
                            if (barn.Objects.TryGetValue(new Vector2(4, 3), out var hopper))
                            {
                                barn.Objects.Remove(hopper.TileLocation);
                                hopper.TileLocation = new Vector2(6, 3);
                                barn.Objects[hopper.TileLocation] = hopper;
                            }

                            break;
                        }

                        case false when barn.Name.Contains("Premium") || barn.animalLimit.Value == 18:
                        {
                            barn.animalLimit.Value = 16;
                            if (barn.Objects.TryGetValue(new Vector2(2, 5), out var hopper))
                            {
                                barn.Objects.Remove(hopper.TileLocation);
                                hopper.TileLocation = new Vector2(4, 4);
                                barn.Objects[hopper.TileLocation] = hopper;
                            }

                            break;
                        }
                    }

                    ModHelper.GameContent.InvalidateCache("Maps/Barn3");
                    ModHelper.GameContent.InvalidateCache("Maps/SVE_PremiumBarn");
                    break;

                case AnimalHouse coop when coop.Name.Contains("Coop"):
                    coop.animalLimit.Value = areThereAnyPrestigedProducers switch
                    {
                        true when coop.Name.Contains("Deluxe") || coop.animalLimit.Value == 12 => 14,
                        true when coop.Name.Contains("Premium") || coop.animalLimit.Value == 16 => 18,
                        false when coop.Name.Contains("Deluxe") || coop.animalLimit.Value == 14 => 12,
                        false when coop.Name.Contains("Premium") || coop.animalLimit.Value == 18 => 16,
                        _ => coop.animalLimit.Value,
                    };

                    ModHelper.GameContent.InvalidateCache("Maps/Coop3");
                    ModHelper.GameContent.InvalidateCache("Maps/SVE_PremiumCoop");
                    break;

                case SlimeHutch hutch:
                    if (areThereAnyPipers)
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
                        var slimeCount = hutch.characters.OfType<GreenSlime>().Count();
                        while (slimeCount > 20)
                        {
                            hutch.characters.RemoveAt(Game1.random.Next(slimeCount--));
                        }
                    }

                    ModHelper.GameContent.InvalidateCache("Maps/SlimeHutch");
                    break;
            }

            return true; // continue enumeration
        });

        this.Disable();
    }
}
