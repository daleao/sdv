using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Locations;
using TheLion.Stardew.Common.Extensions;
using xTile.Dimensions;

namespace TheLion.Stardew.Professions.Framework.Utility;

/// <summary>Holds common methods and properties related to map tilesheets.</summary>
public static class Tiles
{
    /// <summary>Find all tiles in a mine map containing either a ladder or shaft.</summary>
    /// <param name="shaft">The MineShaft location.</param>
    /// <remarks>Credit to <c>pomepome</c>.</remarks>
    public static IEnumerable<Vector2> GetLadderTiles(MineShaft shaft)
    {
        for (var i = 0; i < shaft.Map.GetLayer("Buildings").LayerWidth; ++i)
        for (var j = 0; j < shaft.Map.GetLayer("Buildings").LayerHeight; ++j)
        {
            var index = shaft.getTileIndexAt(new(i, j), "Buildings");
            if (index.IsAnyOf(173, 174)) yield return new(i, j);
        }
    }

    /// <summary>Check if a tile on a map is valid for spawning diggable treasure.</summary>
    /// <param name="tile">The tile to check.</param>
    /// <param name="location">The game location.</param>
    public static bool IsTileValidForTreasure(Vector2 tile, GameLocation location)
    {
        var noSpawn = location.doesTileHaveProperty((int) tile.X, (int) tile.Y, "NoSpawn", "Back");
        return string.IsNullOrEmpty(noSpawn) && location.isTileLocationTotallyClearAndPlaceable(tile) &&
               IsTileClearOfDebris(tile, location) && !location.isBehindBush(tile) && !location.isBehindTree(tile);
    }

    /// <summary>Check if a tile is clear of debris.</summary>
    /// <param name="tile">The tile to check.</param>
    /// <param name="location">The game location.</param>
    public static bool IsTileClearOfDebris(Vector2 tile, GameLocation location)
    {
        return (from debris in location.debris
            where debris.item is not null && debris.Chunks.Count > 0
            select new Vector2((int) (debris.Chunks[0].position.X / Game1.tileSize) + 1,
                (int) (debris.Chunks[0].position.Y / Game1.tileSize) + 1)).All(debrisTile => debrisTile != tile);
    }

    /// <summary>Force a tile to be affected by the hoe.</summary>
    /// <param name="tile">The tile to change.</param>
    /// <param name="location">The game location.</param>
    public static bool MakeTileDiggable(Vector2 tile, GameLocation location)
    {
        if (location.doesTileHaveProperty((int) tile.X, (int) tile.Y, "Diggable", "Back") is not null) return true;

        var digSpot = new Location((int) tile.X * Game1.tileSize, (int) tile.Y * Game1.tileSize);
        location.Map.GetLayer("Back").PickTile(digSpot, Game1.viewport.Size).Properties["Diggable"] = true;
        return false;
    }
}