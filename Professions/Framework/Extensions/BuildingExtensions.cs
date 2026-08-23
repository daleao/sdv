namespace DaLion.Professions.Framework.Extensions;

#region using directives

using DaLion.Shared.Extensions.Stardew;
using Microsoft.Xna.Framework;
using StardewValley.Buildings;
using StardewValley.Monsters;

#endregion using directives

/// <summary>Extensions for the <see cref="Building"/> class.</summary>
internal static class BuildingExtensions
{
    /// <summary>Determines whether the owner of the <paramref name="building"/> has the specified <paramref name="profession"/>.</summary>
    /// <param name="building">The <see cref="Building"/>.</param>
    /// <param name="profession">A <see cref="IProfession"/>.</param>
    /// <param name="prestiged">Whether to check for the prestiged variant.</param>
    /// <returns><see langword="true"/> if the <see cref="Farmer"/> who owns the <paramref name="building"/> has the <paramref name="profession"/>, otherwise <see langword="false"/>.</returns>
    internal static bool DoesOwnerHaveProfession(this Building building, IProfession profession, bool prestiged = false)
    {
        return building.GetOwner().HasProfession(profession, prestiged);
    }

    /// <summary>Determines whether the owner of the <paramref name="building"/>---or any <see cref="Farmer"/> instance in the game session, if allowed by the module's settings---has the specified <paramref name="profession"/>.</summary>
    /// <param name="building">The <see cref="Building"/>.</param>
    /// <param name="profession">A <see cref="IProfession"/>.</param>
    /// <param name="prestiged">Whether to check for the prestiged variant.</param>
    /// <returns><see langword="true"/> if the <see cref="Farmer"/> who owns the <paramref name="building"/> has the <paramref name="profession"/>, otherwise <see langword="false"/>.</returns>
    internal static bool DoesOwnerHaveProfessionOrLax(this Building building, IProfession profession, bool prestiged = false)
    {
        return building.GetOwner().HasProfessionOrLax(profession, prestiged);
    }

    /// <summary>Checks whether the <paramref name="building"/> is owned by the specified <see cref="Farmer"/>, or if <see cref="ProfessionsConfig.LaxOwnershipRequirements"/> is enabled in the mod's config settings.</summary>
    /// <param name="building">The <see cref="Building"/>.</param>
    /// <param name="farmer">The <see cref="Farmer"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="building"/>'s owner value is equal to the unique ID of the specified <paramref name="farmer"/> or if <see cref="ProfessionsConfig.LaxOwnershipRequirements"/> is enabled in the mod's config settings, otherwise <see langword="false"/>.</returns>
    internal static bool IsOwnedByOrLax(this Building building, Farmer farmer)
    {
        return building.IsOwnedBy(farmer) || Config.LaxOwnershipRequirements;
    }

    /// <summary>Applies applicable profession rules to the <paramref name="building"/>.</summary>
    /// <param name="building">The <see cref="Building"/>.</param>
    /// <param name="areThereAnyPrestigedBreeders">Whether any player in the game world has the prestiged Breeder profession.</param>
    /// <param name="areThereAnyPrestigedProducers">Whether any player in the game world has the prestiged Producer profession.</param>
    /// <param name="areThereAnyPipers">Whether any player in the game world has the Piper profession.</param>
    internal static void ApplyProfessionRules(
        this Building building,
        bool areThereAnyPrestigedBreeders = false,
        bool areThereAnyPrestigedProducers = false,
        bool areThereAnyPipers = false)
    {
        if (building is FishPond pond)
        {
            pond.UpdateMaximumOccupancy();
            return; // continue enumeration
        }

        var indoors = building.GetIndoors();
        switch (indoors)
        {
            case AnimalHouse house:
                if (house.Name.Contains("Barn"))
                {
                    var barn = house;
                    switch (areThereAnyPrestigedBreeders)
                    {
                        case true when barn.Name.Contains("Deluxe") && barn.animalLimit.Value == 12:
                            {
                                barn.animalLimit.Value = 14;
                                if (barn.Objects.TryGetValue(new Vector2(6, 3), out var hopper))
                                {
                                    barn.Objects.Remove(hopper.TileLocation);
                                    hopper.TileLocation = new Vector2(4, 3);
                                    barn.Objects[hopper.TileLocation] = hopper;
                                    barn.feedAllAnimals();
                                }

                                break;
                            }

                        case true when barn.Name.Contains("Premium") && barn.animalLimit.Value == 16:
                            {
                                barn.animalLimit.Value = 18;
                                if (barn.Objects.TryGetValue(new Vector2(4, 4), out var hopper))
                                {
                                    barn.Objects.Remove(hopper.TileLocation);
                                    hopper.TileLocation = new Vector2(2, 5);
                                    barn.Objects[hopper.TileLocation] = hopper;
                                    barn.feedAllAnimals();
                                }

                                break;
                            }

                        case false when barn.Name.Contains("Deluxe") && barn.animalLimit.Value == 14:
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

                        case false when barn.Name.Contains("Premium") && barn.animalLimit.Value == 18:
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
                }
                else if (house.Name.Contains("Coop"))
                {
                    var coop = house;
                    house.animalLimit.Value = areThereAnyPrestigedProducers switch
                    {
                        true when coop.Name.Contains("Deluxe") && coop.animalLimit.Value == 12 => 14,
                        true when coop.Name.Contains("Premium") && coop.animalLimit.Value == 16 => 18,
                        false when coop.Name.Contains("Deluxe") && coop.animalLimit.Value == 14 => 12,
                        false when coop.Name.Contains("Premium") && coop.animalLimit.Value == 18 => 16,
                        _ => coop.animalLimit.Value,
                    };

                    ModHelper.GameContent.InvalidateCache("Maps/Coop3");
                    ModHelper.GameContent.InvalidateCache("Maps/SVE_PremiumCoop");
                }

                break;

            case SlimeHutch hutch:
                if (areThereAnyPipers)
                {
                    Reflector
                        .GetUnboundFieldSetter<SlimeHutch, int>(hutch, "_slimeCapacity")
                        .Invoke(hutch, 30);
                    hutch.Objects.Remove(new Vector2(16, 5));
                    hutch.Objects.Remove(new Vector2(16, 10));
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
    }
}
