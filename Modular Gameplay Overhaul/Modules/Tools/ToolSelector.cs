namespace DaLion.Overhaul.Modules.Tools;

#region using directives

using System.Collections.Generic;
using System.Linq;
using DaLion.Shared.Extensions.Stardew;
using Microsoft.Xna.Framework;
using NetFabric.Hyperlinq;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;

#endregion using directives

/// <summary>Smart <see cref="Tool"/> selector.</summary>
internal static class ToolSelector
{
    /// <summary>Intelligently selects the appropriate tool for the given <paramref name="tile"/>.</summary>
    /// <param name="tile">The action tile.</param>
    /// <param name="who">The <see cref="Farmer"/>.</param>
    /// <param name="location">The current <see cref="GameLocation"/>.</param>
    /// <returns>The integer index of the appropriate <see cref="Tool"/> in <paramref name="who"/>'s inventory.</returns>
    internal static int SmartSelect(Vector2 tile, Farmer who, GameLocation location)
    {
        return TryForAnimals(tile, location, who, out var toolIndex) ||
               TryForObjects(tile, location, who, out toolIndex) ||
               TryForTerrainFeatures(tile, location, who, out toolIndex) ||
               TryForResourceClumps(tile, location, who, out toolIndex) ||
               TryForPetBowl(tile, location, who, out toolIndex) ||
               TryForPanningSpots(tile, location, who, out toolIndex) ||
               TryForWaterRefill(tile, location, who, out toolIndex) ||
               TryForFishing(tile, location, who, out toolIndex) ||
               TryForTillableSoil(tile, location, who, out toolIndex)
            ? toolIndex
            : -1;
    }

    private static bool TryForAnimals(Vector2 tile, GameLocation location, Farmer who, out int toolIndex)
    {
        toolIndex = -1;

        var animalTools = ToolsModule.State.SelectableTools
            .AsValueEnumerable()
            .Where(t => t is MilkPail or Shears)
            .ToArray();
        if (animalTools.Length == 0)
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
        if (animals.FirstOrDefault(a => a.GetHarvestBoundingBox().Intersects(r)) is not { } animal)
        {
            return false;
        }

        if (animal.type.Value.Contains("Cow") && animalTools.FirstOrDefault(t => t is MilkPail) is { } pail)
        {
            toolIndex = who.Items.IndexOf(pail);
            return true;
        }

        if (animal.type.Value == "Sheep" && animalTools.FirstOrDefault(t => t is Shears) is { } shears)
        {
            toolIndex = who.Items.IndexOf(shears);
            return true;
        }

        return false;
    }

    private static bool TryForObjects(Vector2 tile, GameLocation location, Farmer who, out int toolIndex)
    {
        toolIndex = -1;
        if (!location.Objects.TryGetValue(tile, out var @object))
        {
            return false;
        }

        if (@object.IsStone() && ToolsModule.State.SelectableTools.FirstOrDefault(t => t is Pickaxe) is { } pick)
        {
            toolIndex = who.Items.IndexOf(pick);
            return true;
        }

        if (@object.IsTwig() && ToolsModule.State.SelectableTools.FirstOrDefault(t => t is Axe) is { } axe)
        {
            toolIndex = who.Items.IndexOf(axe);
            return true;
        }

        if (@object.IsArtifactSpot() && ToolsModule.State.SelectableTools.FirstOrDefault(t => t is Hoe) is { } hoe)
        {
            toolIndex = who.Items.IndexOf(hoe);
            return true;
        }

        if (@object.IsWeed() && ToolsModule.State.SelectableTools.FirstOrDefault(t => t is MeleeWeapon w && !w.isScythe()) is { } scythe)
        {
            toolIndex = who.Items.IndexOf(scythe);
            return true;
        }

        return false;
    }

    private static bool TryForTerrainFeatures(Vector2 tile, GameLocation location, Farmer who, out int toolIndex)
    {
        toolIndex = -1;
        if (!location.terrainFeatures.TryGetValue(tile, out var feature))
        {
            return false;
        }

        toolIndex = feature switch
        {
            Tree or FruitTree when ToolsModule.State.SelectableTools.FirstOrDefault(t => t is Axe) is { } axe => who.Items.IndexOf(axe),
            Grass when ToolsModule.State.SelectableTools.FirstOrDefault(t => t is MeleeWeapon w && w.isScythe()) is { } scythe => who.Items.IndexOf(scythe),
            HoeDirt dirt1 when dirt1.needsWatering() && dirt1.state.Value < 1 && ToolsModule.State.SelectableTools.FirstOrDefault(t => t is WateringCan) is { } can => who.Items.IndexOf(can),
            HoeDirt { crop: { } crop } dirt2 when dirt2.readyForHarvest() && crop.harvestMethod.Value == 1 && ToolsModule.State.SelectableTools.FirstOrDefault(t => t is MeleeWeapon w && w.isScythe()) is { } scythe => who.Items.IndexOf(scythe),
            _ => -1,
        };

        return toolIndex >= 0;
    }

    private static bool TryForResourceClumps(Vector2 tile, GameLocation location, Farmer who, out int toolIndex)
    {
        toolIndex = -1;
        var r = new Rectangle((int)tile.X * 64, (int)tile.Y * 64, 64, 64);
        if (location.resourceClumps.FirstOrDefault(c => c.getBoundingBox(c.tile.Value).Intersects(r)) is not { } clump)
        {
            return false;
        }

        toolIndex = clump.parentSheetIndex.Value switch
        {
            600 or 602 when ToolsModule.State.SelectableTools.FirstOrDefault(t => t is Axe) is { } axe => who.Items.IndexOf(axe),
            622 or 672 or 752 or 754 or 756 or 758 when ToolsModule.State.SelectableTools.FirstOrDefault(t => t is Pickaxe) is { } pick => who.Items.IndexOf(pick),
            _ => -1,
        };

        return toolIndex >= 0;
    }

    private static bool TryForPetBowl(Vector2 tile, GameLocation location, Farmer who, out int toolIndex)
    {
        toolIndex = -1;
        if (location is not Farm farm || farm.getTileIndexAt((int)tile.X, (int)tile.Y, "Buildings") != 1938 ||
            farm.petBowlWatered.Value || ToolsModule.State.SelectableTools.FirstOrDefault(t => t is WateringCan) is not { } can)
        {
            return false;
        }

        toolIndex = who.Items.IndexOf(can);
        return true;
    }

    private static bool TryForPanningSpots(Vector2 tile, GameLocation location, Farmer who, out int toolIndex)
    {
        toolIndex = -1;
        if (location.orePanPoint is null)
        {
            return false;
        }

        var orePanRect = new Rectangle((location.orePanPoint.X * Game1.tileSize) - Game1.tileSize, (location.orePanPoint.Y * Game1.tileSize) - Game1.tileSize, Game1.tileSize * 4, Game1.tileSize * 4);
        if (!orePanRect.Contains((int)tile.X * Game1.tileSize, (int)tile.Y * Game1.tileSize) ||
            !(Utility.distance(who.getStandingX(), orePanRect.Center.X, who.getStandingY(), orePanRect.Center.Y) <= 192f) ||
            ToolsModule.State.SelectableTools.FirstOrDefault(t => t is Pan) is not { } pan)
        {
            return false;
        }

        toolIndex = who.Items.IndexOf(pan);
        return true;
    }

    private static bool TryForWaterRefill(Vector2 tile, GameLocation location, Farmer who, out int toolIndex)
    {
        toolIndex = location.CanRefillWateringCanOnTile((int)tile.X, (int)tile.Y) &&
                    ToolsModule.State.SelectableTools.FirstOrDefault(t => t is WateringCan) is WateringCan can &&
                    !can.IsBottomless && can.WaterLeft < can.waterCanMax
            ? who.Items.IndexOf(can)
            : -1;
        return toolIndex >= 0;
    }

    private static bool TryForFishing(Vector2 tile, GameLocation location, Farmer who, out int toolIndex)
    {
        toolIndex = location.waterTiles is not null && location.waterTiles[(int)tile.X, (int)tile.Y] &&
                    ToolsModule.State.SelectableTools.FirstOrDefault(t => t is FishingRod) is { } rod
            ? who.Items.IndexOf(rod)
            : -1;
        return toolIndex >= 0;
    }

    private static bool TryForTillableSoil(Vector2 tile, GameLocation location, Farmer who, out int toolIndex)
    {
        toolIndex = location.doesTileHaveProperty((int)tile.X, (int)tile.Y, "Diggable", "Back") is not null &&
                   ToolsModule.State.SelectableTools.FirstOrDefault(t => t is Hoe) is { } hoe
            ? who.Items.IndexOf(hoe)
            : -1;
        return toolIndex >= 0;
    }
}
