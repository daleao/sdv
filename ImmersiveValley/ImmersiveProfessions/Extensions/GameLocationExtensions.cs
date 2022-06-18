namespace DaLion.Stardew.Professions.Extensions;

#region using directives

using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Monsters;
using xTile.Dimensions;

using Common.Extensions;
using Framework;

using SUtility = StardewValley.Utility;

#endregion using directives

/// <summary>Extensions for the <see cref="GameLocation"/> class.</summary>
public static class GameLocationExtensions
{
    /// <summary>Whether any farmer in this location has a specific profession.</summary>
    /// <param name="profession">The <see cref="IProfession"/> to check.</param>
    public static bool DoesAnyPlayerHereHaveProfession(this GameLocation location, IProfession profession)
    {
        if (!Context.IsMultiplayer && location.Equals(Game1.currentLocation))
            return Game1.player.HasProfession(profession);
        return location.farmers.Any(farmer => farmer.HasProfession(profession));
    }

    /// <summary>Whether any farmer in this location has a specific profession.</summary>
    /// <param name="profession">The <see cref="IProfession"/> to check.</param>
    /// <param name="farmers">All the farmer instances in the location with the given profession.</param>
    public static bool DoesAnyPlayerHereHaveProfession(this GameLocation location, IProfession profession,
        out IList<Farmer> farmers)
    {
        farmers = new List<Farmer>();
        if (!Context.IsMultiplayer && location.Equals(Game1.player.currentLocation) &&
            Game1.player.HasProfession(profession))
            farmers.Add(Game1.player);
        else
            foreach (var farmer in location.farmers.Where(farmer => farmer.HasProfession(profession)))
                farmers.Add(farmer);

        return farmers.Any();
    }

    /// <summary>Find a character with the specified hash code.</summary>
    /// <typeparam name="T">A subtype of <see cref="Character"/>.</typeparam>
    /// <param name="hash">An integer hash code.</param>
    public static bool FindCharacterByHash<T>(this GameLocation location, int hash, out T character) where T : Character
    {
        character = location.characters.OfType<T>().FirstOrDefault(c => c.GetHashCode() == hash);
        return character is not null;
    }

    /// <summary>Get the raw fish data for this location during the current game season.</summary>
    public static string[] GetRawFishDataForCurrentSeason(this GameLocation location)
    {
        var locationData =
            Game1.content.Load<Dictionary<string, string>>(PathUtilities.NormalizeAssetName("Data/Locations"));
        return locationData[location.NameOrUniqueName].Split('/')[4 + SUtility.getSeasonNumber(Game1.currentSeason)]
            .Split(' ');
    }

    /// <summary>Get the raw fish data for this location including all seasons.</summary>
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

    /// <summary>Whether this location can is a dungeon.</summary>
    public static bool IsDungeon(this GameLocation location)
    {
        return location is MineShaft shaft && !shaft.IsTreasureOrSafeRoom() ||
               location is BugLand or VolcanoDungeon ||
               location.NameOrUniqueName.ContainsAnyOf("CrimsonBadlands", "DeepWoods", "Highlands", "RidgeForest",
                   "SpiritRealm", "AsteroidsDungeon");
    }

    /// <summary>Whether this location has spawned enemies.</summary>
    public static bool HasMonsters(this GameLocation location)
    {
        return location.characters.OfType<Monster>().Any() && location is not SlimeHutch;
    }

    /// <summary>Check if a tile on a map is valid for spawning diggable treasure.</summary>
    /// <param name="tile">The tile to check.</param>
    public static bool IsTileValidForTreasure(this GameLocation location, Vector2 tile)
    {
        return (!location.objects.TryGetValue(tile, out var o) || o == null) &&
               location.doesTileHaveProperty((int) tile.X, (int) tile.Y, "Spawnable", "Back") != null &&
               !location.doesEitherTileOrTileIndexPropertyEqual((int) tile.X, (int) tile.Y, "Spawnable", "Back", "F") &&
               location.isTileLocationTotallyClearAndPlaceable(tile) &&
               location.getTileIndexAt((int) tile.X, (int) tile.Y, "AlwaysFront") == -1 &&
               location.getTileIndexAt((int) tile.X, (int) tile.Y, "Front") == -1 && !location.isBehindBush(tile) &&
               !location.isBehindTree(tile);
    }

    /// <summary>Check if a tile is clear of debris.</summary>
    /// <param name="tile">The tile to check.</param>
    public static bool IsTileClearOfDebris(this GameLocation location, Vector2 tile)
    {
        return (from debris in location.debris
            where debris.item is not null && debris.Chunks.Count > 0
            select new Vector2((int) (debris.Chunks[0].position.X / Game1.tileSize) + 1,
                (int) (debris.Chunks[0].position.Y / Game1.tileSize) + 1)).All(debrisTile => debrisTile != tile);
    }

    /// <summary>Force a tile to be affected by the hoe.</summary>
    /// <param name="tile">The tile to change.</param>
    public static bool MakeTileDiggable(this GameLocation location, Vector2 tile)
    {
        var (x, y) = tile;
        if (location.doesTileHaveProperty((int) x, (int) y, "Diggable", "Back") is not null) return true;

        var digSpot = new Location((int) x * Game1.tileSize, (int) y * Game1.tileSize);
        location.Map.GetLayer("Back").PickTile(digSpot, Game1.viewport.Size).Properties["Diggable"] = true;
        return false;
    }
}