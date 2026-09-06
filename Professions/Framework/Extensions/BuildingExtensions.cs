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
        if (item is not SObject feed || building.buildingType.Value != "Silo" || !feed.IsValidAnimalFeed(out var feedCategory))
        {
            return false;
        }

        var location = building.GetParentLocation();
        var storedFeedsPerCategory = Data.Read(location, DataKeys.PiecesOfFeed).ParseDictionary<string, int>();
        var capacity = location.GetHayCapacity() / 10;
        var amountThatCanBeAdded = storedFeedsPerCategory.TryGetValue(feedCategory.Id, out var amount) ? capacity - amount : capacity;
        if (amountThatCanBeAdded <= 0)
        {
            Game1.playSound("cancel");
            return false;
        }

        var amountActuallyAdded = Math.Min(feed.Stack, amountThatCanBeAdded);
        storedFeedsPerCategory.AddOrUpdate(feedCategory.Id, amountActuallyAdded, (a, b) => a + b);
        Data.Write(location, DataKeys.PiecesOfFeed, storedFeedsPerCategory.Stringify());
        if (feed.ConsumeStack(amountActuallyAdded) == null)
        {
            who.removeItemFromInventory(feed);
        }

        building.ShowShipment(item, playThrowSound: false);
        var deposited = feed.getOne();
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
        if (item is not SObject feed || building.buildingType.Value != "Silo" || !feed.IsValidAnimalFeed(out var feedCategory))
        {
            return;
        }

        var location = building.GetParentLocation();
        var feedsPerCategory = Data.Read(location, DataKeys.PiecesOfFeed).ParseDictionary<string, int>();
        feedsPerCategory.AddOrUpdate(feedCategory.Id, stack, (a, b) => a - b);
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
            i => i is not null && i is SObject obj && obj.IsValidAnimalFeed(out _) && i.IsCrop(excludeFlowers: true),
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
        var shouldFixAllAnimals = false;
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
                                if (barn.Objects.TryGetValue(new Vector2(4, 3), out var object1))
                                {
                                    object1.performRemoveAction();
                                    Game1.createItemDebris(object1.getOne(), object1.TileLocation * 64f, Game1.down, location: barn);
                                    barn.Objects.Remove(object1.TileLocation);
                                }

                                if (barn.Objects.TryGetValue(new Vector2(5, 3), out var object2))
                                {
                                    object2.performRemoveAction();
                                    Game1.createItemDebris(object2.getOne(), object2.TileLocation * 64f, Game1.down, location: barn);
                                    barn.Objects.Remove(object2.TileLocation);
                                }

                                if (barn.Objects.TryGetValue(new Vector2(6, 3), out var object3))
                                {
                                    object3.performRemoveAction();
                                    barn.Objects.Remove(object3.TileLocation);
                                    if (object3.QualifiedItemId == QIDs.FeedHopper)
                                    {
                                        object3.TileLocation = new Vector2(4, 3);
                                        barn.Objects.Add(object3.TileLocation, object3);
                                        barn.feedAllAnimals();
                                    }
                                    else
                                    {
                                        Game1.createItemDebris(object3.getOne(), object3.TileLocation * 64f, Game1.down, location: barn);
                                    }
                                }

                                break;
                            }

                        case true when barn.Name.Contains("Premium") && barn.animalLimit.Value == 16:
                            {
                                barn.animalLimit.Value = 18;
                                if (barn.Objects.TryGetValue(new Vector2(4, 4), out var @object))
                                {
                                    @object.performRemoveAction();
                                    Game1.createItemDebris(@object.getOne(), @object.TileLocation * 64f, Game1.down, location: barn);
                                    barn.Objects.Remove(@object.TileLocation);
                                }

                                break;
                            }

                        case false when barn.Name.Contains("Deluxe") && barn.animalLimit.Value == 14:
                            {
                                barn.animalLimit.Value = 12;
                                barn.Objects.Remove(new Vector2(6, 3));
                                barn.Objects.Remove(new Vector2(5, 3));
                                if (barn.Objects.TryGetValue(new Vector2(4, 3), out var @object) && @object.QualifiedItemId == QIDs.FeedHopper)
                                {
                                    @object.performRemoveAction();
                                    barn.Objects.Remove(@object.TileLocation);
                                    @object.TileLocation = new Vector2(6, 3);
                                    barn.Objects.Add(@object.TileLocation, @object);
                                }

                                for (var i = barn.animalsThatLiveHere.Count; i > 12; i--)
                                {
                                    var toRelocate = barn.animals.Values.Choose()!;
                                    toRelocate.home = null;
                                    toRelocate.homeInterior = null;
                                    shouldFixAllAnimals = true;
                                }

                                break;
                            }

                        case false when barn.Name.Contains("Premium") && barn.animalLimit.Value == 18:
                            {
                                barn.animalLimit.Value = 16;
                                barn.Objects.Remove(new Vector2(5, 4));
                                barn.Objects.Remove(new Vector2(4, 4));
                                var (tile, hopper) = barn.Objects.Pairs.FirstOrDefault(pair => pair.Value.QualifiedItemId == QIDs.FeedHopper);
                                if (hopper is not null)
                                {
                                    hopper.performRemoveAction();
                                    barn.Objects.Remove(tile);
                                    hopper.TileLocation = new Vector2(4, 4);
                                    barn.Objects.Add(hopper.TileLocation, hopper);
                                }

                                for (var i = barn.animalsThatLiveHere.Count; i > 16; i--)
                                {
                                    var toRelocate = barn.animals.Values.Choose()!;
                                    toRelocate.home = null;
                                    toRelocate.homeInterior = null;
                                    shouldFixAllAnimals = true;
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
                    switch (areThereAnyPrestigedProducers)
                    {
                        case true when coop.Name.Contains("Deluxe") && coop.animalLimit.Value == 12:
                            coop.animalLimit.Value = 14;
                            if (coop.Objects.TryGetValue(new Vector2(4, 3), out var @object))
                            {
                                @object.performRemoveAction();
                                Game1.createItemDebris(@object.getOne(), @object.TileLocation * 64f, Game1.down, location: coop);
                                coop.Objects.Remove(@object.TileLocation);
                            }

                            break;
                        case true when coop.Name.Contains("Premium") && coop.animalLimit.Value == 16:
                            {
                                coop.animalLimit.Value = 18;
                                if (coop.Objects.TryGetValue(new Vector2(3, 4), out var object1))
                                {
                                    object1.performRemoveAction();
                                    Game1.createItemDebris(object1.getOne(), object1.TileLocation * 64f, Game1.down, location: coop);
                                    coop.Objects.Remove(object1.TileLocation);
                                }

                                if (coop.Objects.TryGetValue(new Vector2(22, 4), out var object2))
                                {
                                    object2.performRemoveAction();
                                    Game1.createItemDebris(object2.getOne(), object2.TileLocation * 64f, Game1.down, location: coop);
                                    coop.Objects.Remove(object2.TileLocation);
                                }

                                break;
                            }

                        case false when coop.Name.Contains("Deluxe") && coop.animalLimit.Value == 14:
                            coop.animalLimit.Value = 12;
                            coop.Objects.Remove(new Vector2(5, 3));
                            coop.Objects.Remove(new Vector2(18, 3));
                            for (var i = coop.animalsThatLiveHere.Count; i > 12; i--)
                            {
                                var toRelocate = coop.animals.Values.Choose()!;
                                toRelocate.home = null;
                                toRelocate.homeInterior = null;
                                shouldFixAllAnimals = true;
                            }

                            break;
                        case false when coop.Name.Contains("Premium") && coop.animalLimit.Value == 18:
                            {
                                coop.animalLimit.Value = 16;
                                coop.Objects.Remove(new Vector2(4, 4));
                                coop.Objects.Remove(new Vector2(21, 4));
                                var (tile1, hopper) = coop.Objects.Pairs.FirstOrDefault(pair => pair.Value.QualifiedItemId == QIDs.FeedHopper);
                                if (hopper is not null)
                                {
                                    hopper.performRemoveAction();
                                    coop.Objects.Remove(tile1);
                                    hopper.TileLocation = new Vector2(3, 4);
                                    coop.Objects.Add(hopper.TileLocation, hopper);
                                }

                                var (tile2, incubator) = coop.Objects.Pairs.FirstOrDefault(pair => pair.Value.QualifiedItemId == QIDs.Incubator);
                                if (incubator is not null)
                                {
                                    incubator.performRemoveAction();
                                    coop.Objects.Remove(tile2);
                                    incubator.TileLocation = new Vector2(22, 4);
                                    coop.Objects.Add(incubator.TileLocation, incubator);
                                }

                                for (var i = coop.animalsThatLiveHere.Count; i > 16; i--)
                                {
                                    var toRelocate = coop.animals.Values.Choose()!;
                                    toRelocate.home = null;
                                    toRelocate.homeInterior = null;
                                    shouldFixAllAnimals = true;
                                }

                                break;
                            }

                    }

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
                    if (hutch.Objects.TryGetValue(new Vector2(16, 5), out var object1))
                    {
                        object1.performRemoveAction();
                        hutch.debris.Add(new Debris(object1, object1.TileLocation + new Vector2(-1, 0), object1.TileLocation + new Vector2(-1, 0)));
                        hutch.Objects.Remove(object1.TileLocation);
                    }

                    if (hutch.Objects.TryGetValue(new Vector2(16, 10), out var object2))
                    {
                        object2.performRemoveAction();
                        hutch.debris.Add(new Debris(object2, object2.TileLocation + new Vector2(-1, 0), object2.TileLocation + new Vector2(-1, 0)));
                        hutch.Objects.Remove(object2.TileLocation);
                    }

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
                        hutch.characters.RemoveAt(Random.Shared.Next(slimeCount--));
                    }
                }

                ModHelper.GameContent.InvalidateCache("Maps/SlimeHutch");
                break;
        }

        if (shouldFixAllAnimals)
        {
            Utility.fixAllAnimals();
        }
    }
}
