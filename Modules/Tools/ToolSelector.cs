namespace DaLion.Overhaul.Modules.Tools;

#region using directives

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using DaLion.Shared.Extensions.Stardew;
using Microsoft.Xna.Framework;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;

#endregion using directives

/// <summary>Smart <see cref="Tool"/> selector.</summary>
internal static class ToolSelector
{
    /// <summary>Attempts to select the most appropriate tool for the given <paramref name="tile"/>.</summary>
    /// <param name="tile">The action tile.</param>
    /// <param name="who">The <see cref="Farmer"/>.</param>
    /// <param name="location">The current <see cref="GameLocation"/>.</param>
    /// <param name="selectable">The <see cref="SelectableTool"/>.</param>
    /// <returns>The integer index of the appropriate <see cref="Tool"/> in <paramref name="who"/>'s inventory.</returns>
    internal static bool TryFor(
        Vector2 tile, Farmer who, GameLocation location, [NotNullWhen(true)] out SelectableTool? selectable)
    {
        return TryForAnimals(tile, location, out selectable) ||
               TryForObjects(tile, location, out selectable) ||
               TryForTerrainFeatures(tile, location, out selectable) ||
               TryForResourceClumps(tile, location, out selectable) ||
               TryForPetBowl(tile, location, out selectable) ||
               TryForPanningSpots(tile, location, who, out selectable) ||
               TryForWaterRefill(tile, location, out selectable) ||
               TryForFishing(tile, location, out selectable) ||
               TryForTillableSoil(tile, location, out selectable);
    }

    private static bool TryForAnimals(Vector2 tile, GameLocation location, [NotNullWhen(true)] out SelectableTool? selectable)
    {
        selectable = null;

        if (!ToolsModule.State.SelectableToolByType.ContainsKey(typeof(MilkPail)) &&
            !ToolsModule.State.SelectableToolByType.ContainsKey(typeof(Shears)))
        {
            return false;
        }

        var animals = location switch
        {
            Farm farm => farm.animals.Values.ToList(),
            AnimalHouse house => house.animals.Values.ToList(),
            _ => new List<FarmAnimal>(),
        };

        if (animals.Count == 0)
        {
            return false;
        }

        var r = new Rectangle(((int)tile.X * 64) - 32, ((int)tile.Y * 64) - 32, 64, 64);
        return animals.FirstOrDefault(a => a.GetHarvestBoundingBox().Intersects(r)) is { } animal &&
               ((animal.type.Value.Contains("Cow") &&
                 ToolsModule.State.SelectableToolByType.TryGetValue(typeof(MilkPail), out selectable)) ||
                (animal.type.Value == "Sheep" &&
                 ToolsModule.State.SelectableToolByType.TryGetValue(typeof(Shears), out selectable))) &&
               selectable.HasValue;
    }

    private static bool TryForObjects(Vector2 tile, GameLocation location, [NotNullWhen(true)] out SelectableTool? selectable)
    {
        selectable = null;
        return location.Objects.TryGetValue(tile, out var @object) &&
               ((@object.IsStone() && ToolsModule.State.SelectableToolByType.TryGetValue(typeof(Pickaxe), out selectable)) ||
                (@object.IsTwig() && ToolsModule.State.SelectableToolByType.TryGetValue(typeof(Axe), out selectable)) ||
                (@object.IsArtifactSpot() &&
                 ToolsModule.State.SelectableToolByType.TryGetValue(typeof(Hoe), out selectable)) ||
                (@object.IsWeed() &&
                 ToolsModule.State.SelectableToolByType.TryGetValue(typeof(MeleeWeapon), out selectable))) &&
               selectable.HasValue;
    }

    private static bool TryForTerrainFeatures(Vector2 tile, GameLocation location, [NotNullWhen(true)] out SelectableTool? selectable)
    {
        selectable = null;
        return location.terrainFeatures.TryGetValue(tile, out var feature) &&
               ((feature is Tree or FruitTree &&
                ToolsModule.State.SelectableToolByType.TryGetValue(typeof(Axe), out selectable)) ||
               (feature is Grass &&
                ToolsModule.State.SelectableToolByType.TryGetValue(typeof(MeleeWeapon), out selectable)) ||
               (feature is HoeDirt dirt1 && dirt1.needsWatering() && dirt1.state.Value < 1 &&
                ToolsModule.State.SelectableToolByType.TryGetValue(typeof(WateringCan), out selectable)) ||
               (feature is HoeDirt { crop: { } crop } dirt2 && dirt2.readyForHarvest() &&
                crop.harvestMethod.Value == 1 &&
                ToolsModule.State.SelectableToolByType.TryGetValue(typeof(MeleeWeapon), out selectable))) &&
               selectable.HasValue;
    }

    private static bool TryForResourceClumps(Vector2 tile, GameLocation location, [NotNullWhen(true)] out SelectableTool? selectable)
    {
        selectable = null;
        var r = new Rectangle((int)tile.X * 64, (int)tile.Y * 64, 64, 64);
        return location.resourceClumps.FirstOrDefault(c => c.getBoundingBox(c.tile.Value).Intersects(r)) is { } clump &&
               ((clump.parentSheetIndex.Value is 600 or 602 &&
                 ToolsModule.State.SelectableToolByType.TryGetValue(typeof(Axe), out selectable)) ||
                (clump.parentSheetIndex.Value is 622 or 672 or 752 or 754 or 756 or 758 &&
                 ToolsModule.State.SelectableToolByType.TryGetValue(typeof(Pickaxe), out selectable))) &&
               selectable.HasValue;
    }

    private static bool TryForPetBowl(Vector2 tile, GameLocation location, [NotNullWhen(true)] out SelectableTool? selectable)
    {
        selectable = null;
        return location is Farm farm && farm.getTileIndexAt((int)tile.X, (int)tile.Y, "Buildings") == 1938 &&
               !farm.petBowlWatered.Value &&
               ToolsModule.State.SelectableToolByType.TryGetValue(typeof(Pickaxe), out selectable) && selectable.HasValue;
    }

    private static bool TryForPanningSpots(Vector2 tile, GameLocation location, Farmer who, [NotNullWhen(true)] out SelectableTool? selectable)
    {
        selectable = null;
        if (location.orePanPoint is null)
        {
            return false;
        }

        var orePanRect = new Rectangle(
            (location.orePanPoint.X * Game1.tileSize) - Game1.tileSize,
            (location.orePanPoint.Y * Game1.tileSize) - Game1.tileSize,
            Game1.tileSize * 4,
            Game1.tileSize * 4);
        return orePanRect.Contains((int)tile.X * Game1.tileSize, (int)tile.Y * Game1.tileSize) &&
               Utility.distance(who.getStandingX(), orePanRect.Center.X, who.getStandingY(), orePanRect.Center.Y) <=
               192f && ToolsModule.State.SelectableToolByType.TryGetValue(typeof(Pan), out selectable) && selectable.HasValue;
    }

    private static bool TryForWaterRefill(Vector2 tile, GameLocation location, [NotNullWhen(true)] out SelectableTool? selectable)
    {
        selectable = null;
        return location.CanRefillWateringCanOnTile((int)tile.X, (int)tile.Y) &&
               ToolsModule.State.SelectableToolByType.TryGetValue(typeof(WateringCan), out selectable) &&
               selectable?.Tool is WateringCan { IsBottomless: false } can && can.WaterLeft < can.waterCanMax;
    }

    private static bool TryForFishing(Vector2 tile, GameLocation location, [NotNullWhen(true)] out SelectableTool? selectable)
    {
        selectable = null;
        var nextTile = tile.GetNextTile(Game1.player.FacingDirection);
        var cursorTile = Game1.currentCursorTile;
        return location.waterTiles is not null &&
               ((location.isTileOnMap(nextTile) && location.waterTiles[(int)nextTile.X, (int)nextTile.Y]) ||
                (location.isTileOnMap(cursorTile) && location.waterTiles[(int)cursorTile.X, (int)cursorTile.Y] && (cursorTile - nextTile).Length() <= 6)) &&
               ToolsModule.State.SelectableToolByType.TryGetValue(typeof(FishingRod), out selectable) &&
               selectable.HasValue;
    }

    private static bool TryForTillableSoil(Vector2 tile, GameLocation location, [NotNullWhen(true)] out SelectableTool? selectable)
    {
        selectable = null;
        return location.doesTileHaveProperty((int)tile.X, (int)tile.Y, "Diggable", "Back") is not null &&
               ToolsModule.State.SelectableToolByType.TryGetValue(typeof(Hoe), out selectable) && selectable.HasValue;
    }
}
