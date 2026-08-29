namespace DaLion.Professions.Framework.Extensions;

#region using directives

using DaLion.Professions.Framework.UI;
using DaLion.Shared.Extensions;
using DaLion.Shared.Extensions.Collections;
using DaLion.Shared.Extensions.Stardew;
using Microsoft.Xna.Framework;
using StardewValley.Buildings;
using StardewValley.Menus;
using StardewValley.Mods;
using StardewValley.Monsters;
using StardewValley.Objects;

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

    /// <summary>Converts the <paramref name="item"/> into feed data for animal nutrition and stores it in <paramref name="building"/>'s parent location's <seealso cref="ModDataDictionary"/>.</summary>
    /// <param name="building">The silo <see cref="Building"/>.</param>
    /// <param name="item">The crop <see cref="SObject"/>.</param>
    /// <param name="who">The <see cref="Farmer"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="item"/> was converted, otherwise <see langword="false"/>.</returns>
    internal static bool AddPiecesOfCropFeed(this Building building, Item? item, Farmer who)
    {
        if (item is not SObject crop || building.buildingType.Value != "Silo")
        {
            return false;
        }

        var location = building.GetParentLocation();
        if (!Lookups.CategoryByFeedCrop.TryGetValue(crop.QualifiedItemId, out var category))
        {
            return false;
        }

        var feedsPerCategory = Data.Read(location, DataKeys.PiecesOfFeed).ParseDictionary<CropCategory, int>();
        var capacity = location.GetHayCapacity() / 10;
        var amountThatCanBeAdded = feedsPerCategory.TryGetValue(category, out var amount) ? capacity - amount : capacity;
        if (amountThatCanBeAdded <= 0)
        {
            Game1.playSound("cancel");
            return false;
        }

        var amountActuallyAdded = Math.Min(crop.Stack, amountThatCanBeAdded);
        feedsPerCategory.AddOrUpdate(category, amountActuallyAdded, (a, b) => a + b);
        Data.Write(location, DataKeys.PiecesOfFeed, feedsPerCategory.Stringify());
        if (crop.ConsumeStack(amountActuallyAdded) == null)
        {
            who.removeItemFromInventory(crop);
        }

        building.ShowShipment(item, playThrowSound: false);
        var deposited = crop.getOne();
        deposited.Stack = amountActuallyAdded;
        SiloMenuWrapper.LastItemDeposited = (SObject)deposited;
        if (who.ActiveItem is null)
        {
            who.showNotCarrying();
            who.Halt();
        }

        return true;
    }

    /// <summary>Removes the <paramref name="item"/>'s feed data from the <paramref name="building"/>'s parent location's <seealso cref="ModDataDictionary"/>.</summary>
    /// <param name="building">The silo <see cref="Building"/>.</param>
    /// <param name="item">The crop <see cref="SObject"/>.</param>
    /// <param name="stack">The original stack of <paramref name="item"/>.</param>
    /// <remarks><paramref name="stack"/> is needed because it may be consumed before this point by adding to the player's inventory.</remarks>
    internal static void RemovePiecesOfCropFeed(this Building building, Item? item, int stack)
    {
        if (item is not SObject crop || building.buildingType.Value != "Silo")
        {
            return;
        }

        var location = building.GetParentLocation();
        if (!Lookups.CategoryByFeedCrop.TryGetValue(crop.QualifiedItemId, out var category))
        {
            return;
        }

        var feedsPerCategory = Data.Read(location, DataKeys.PiecesOfFeed).ParseDictionary<CropCategory, int>();
        feedsPerCategory.AddOrUpdate(category, stack, (a, b) => a - b);
        Data.Write(location, DataKeys.PiecesOfFeed, feedsPerCategory.Stringify());
    }

    /// <summary>Opens an <see cref="ItemGrabMenu"/> instance to allow depositing crops into the Silo.</summary>
    /// <param name="silo">The <see cref="Building"/>.</param>
    /// <returns><see langword="true"/> (required by vanilla code).</returns>
    internal static bool OpenSiloMenu(this Building silo)
    {
        var menu = new ItemGrabMenu(
            null,
            reverseGrab: true,
            showReceivingMenu: false,
            i => Lookups.CategoryByFeedCrop.ContainsKey(i?.QualifiedItemId ?? string.Empty),
            (i, w) => silo.AddPiecesOfCropFeed(i, w),
            string.Empty,
            null,
            snapToBottom: true,
            canBeExitedWithKey: true,
            playRightClickSound: false,
            allowRightClick: true,
            showOrganizeButton: false,
            ItemGrabMenu.source_none,
            null,
            -1,
            silo);
        State.MenuWrapper = new(silo, menu);
        Game1.activeClickableMenu = menu;
        var player = Game1.player;
        Game1.playSound("shwip");
        if (player.FacingDirection == 1)
        {
            player.Halt();
        }

        return true; // expected by vanilla code
    }

    /// <inheritdoc cref="ShippingBin.showShipment(Item, bool)"/>
    internal static void ShowShipment(this Building building, Item item, bool playThrowSound = true)
    {
        var parentLocation = building.GetParentLocation();
        if (playThrowSound)
        {
            parentLocation.localSound("backpackIN");
        }

        DelayedAction.playSoundAfterDelay("Ship", playThrowSound ? 250 : 0);
        var itemData = ItemRegistry.GetDataOrErrorItem(item.QualifiedItemId);
        var coloredObj = item as ColoredObject;
        var initialPosition = (new Vector2(building.tileX.Value + 0.5f, building.tileY.Value + 1) * 64f) + (new Vector2(7 + Game1.random.Next(6), 2f) * 4f);
        var array = new bool[2] { false, true };
        foreach (var isColorOverlay in array)
        {
            if (isColorOverlay && (coloredObj is null || coloredObj.ColorSameIndexAsParentSheetIndex))
            {
                continue;
            }

            parentLocation.temporarySprites.Add(
                new TemporaryAnimatedSprite(
                    itemData.TextureName,
                    itemData.GetSourceRect(isColorOverlay ? 1 : 0),
                    initialPosition,
                    flipped: false,
                    0f,
                    Color.White)
                {
                    interval = 9999f,
                    scale = 4f,
                    alphaFade = 0.045f,
                    layerDepth = ((building.tileY.Value + 3) * 64 / 10000f) + 0.000225f,
                    motion = new Vector2(0f, 0.3f),
                    acceleration = new Vector2(0f, 0.2f),
                    scaleChange = -0.05f,
                    color = coloredObj?.color.Value ?? Color.White,
                });
        }
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
