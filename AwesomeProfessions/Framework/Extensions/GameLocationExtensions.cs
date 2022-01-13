using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Monsters;
using DaLion.Stardew.Common.Extensions;
using xTile.Dimensions;
using SUtility = StardewValley.Utility;

namespace DaLion.Stardew.Professions.Framework.Extensions;

public static class GameLocationExtensions
{
    /// <summary>Whether any farmer in the game location has a specific profession.</summary>
    /// <param name="professionName">The name of the profession.</param>
    public static bool DoesAnyPlayerHereHaveProfession(this GameLocation location, string professionName)
    {
        if (!Context.IsMultiplayer && location.Equals(Game1.currentLocation))
            return Game1.player.HasProfession(professionName);
        return location.farmers.Any(farmer => farmer.HasProfession(professionName));
    }

    /// <summary>Whether any farmer in the game location has a specific profession.</summary>
    /// <param name="professionName">The name of the profession.</param>
    /// <param name="farmers">All the farmer instances in the location with the given profession.</param>
    public static bool DoesAnyPlayerHereHaveProfession(this GameLocation location, string professionName,
        out IList<Farmer> farmers)
    {
        farmers = new List<Farmer>();
        if (!Context.IsMultiplayer && location.Equals(Game1.player.currentLocation) &&
            Game1.player.HasProfession(professionName))
            farmers.Add(Game1.player);
        else
            foreach (var farmer in location.farmers.Where(farmer => farmer.HasProfession(professionName)))
                farmers.Add(farmer);

        return farmers.Any();
    }

    /// <summary>Get the raw fish data for the game location and current game season.</summary>
    public static string[] GetRawFishDataForCurrentSeason(this GameLocation location)
    {
        var locationData =
            Game1.content.Load<Dictionary<string, string>>(PathUtilities.NormalizeAssetName("Data/Locations"));
        return locationData[location.NameOrUniqueName].Split('/')[4 + SUtility.getSeasonNumber(Game1.currentSeason)]
            .Split(' ');
    }

    /// <summary>Get the raw fish data for the game location and all seasons.</summary>
    public static string[] GetRawFishDataForAllSeasons(this GameLocation location)
    {
        var locationData =
            Game1.content.Load<Dictionary<string, string>>(PathUtilities.NormalizeAssetName("Data/Locations"));
        List<string> allSeasonFish = new();
        for (var i = 0; i < 4; ++i)
        {
            var seasonalFishData = locationData[location.NameOrUniqueName].Split('/')[4 + i].Split(' ');
            if (seasonalFishData.Length > 1) allSeasonFish.AddRange(seasonalFishData);
        }

        return allSeasonFish.ToArray();
    }

    /// <summary>Whether the game location can spawn enemies.</summary>
    public static bool IsCombatZone(this GameLocation location)
    {
        return location is MineShaft or Woods or VolcanoDungeon ||
               location.NameOrUniqueName.ContainsAnyOf("CrimsonBadlands", "DeepWoods", "RidgeForest",
                   "SpiritRealm") || location.characters.OfType<Monster>().Any();
    }

    /// <summary>Check if a tile on a map is valid for spawning diggable treasure.</summary>
    /// <param name="tile">The tile to check.</param>
    /// <param name="location">The game location.</param>
    public static bool IsTileValidForTreasure(this GameLocation location, Vector2 tile)
    {
        var noSpawn = location.doesTileHaveProperty((int) tile.X, (int) tile.Y, "NoSpawn", "Back");
        return string.IsNullOrEmpty(noSpawn) && location.isTileLocationTotallyClearAndPlaceable(tile) &&
               IsTileClearOfDebris(location, tile) && !location.isBehindBush(tile) && !location.isBehindTree(tile);
    }

    /// <summary>Check if a tile is clear of debris.</summary>
    /// <param name="tile">The tile to check.</param>
    /// <param name="location">The game location.</param>
    public static bool IsTileClearOfDebris(this GameLocation location, Vector2 tile)
    {
        return (from debris in location.debris
            where debris.item is not null && debris.Chunks.Count > 0
            select new Vector2((int) (debris.Chunks[0].position.X / Game1.tileSize) + 1,
                (int) (debris.Chunks[0].position.Y / Game1.tileSize) + 1)).All(debrisTile => debrisTile != tile);
    }

    /// <summary>Force a tile to be affected by the hoe.</summary>
    /// <param name="tile">The tile to change.</param>
    /// <param name="location">The game location.</param>
    public static bool MakeTileDiggable(this GameLocation location, Vector2 tile)
    {
        if (location.doesTileHaveProperty((int) tile.X, (int) tile.Y, "Diggable", "Back") is not null) return true;

        var digSpot = new Location((int) tile.X * Game1.tileSize, (int) tile.Y * Game1.tileSize);
        location.Map.GetLayer("Back").PickTile(digSpot, Game1.viewport.Size).Properties["Diggable"] = true;
        return false;
    }
}