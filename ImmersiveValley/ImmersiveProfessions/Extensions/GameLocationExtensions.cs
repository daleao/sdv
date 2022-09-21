namespace DaLion.Stardew.Professions.Extensions;

#region using directives

using System.Collections.Generic;
using System.Linq;
using DaLion.Common.Extensions;
using DaLion.Stardew.Professions.Framework;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Utilities;
using StardewValley.Locations;
using StardewValley.Monsters;
using StardewValley.TerrainFeatures;
using xTile.Dimensions;

#endregion using directives

/// <summary>Extensions for the <see cref="GameLocation"/> class.</summary>
public static class GameLocationExtensions
{
    /// <summary>
    ///     Determines whether any <see cref="Farmer"/> in this <paramref name="location"/> has the specified
    ///     <paramref name="profession"/>.
    /// </summary>
    /// <param name="location">The <see cref="GameLocation"/>.</param>
    /// <param name="profession">The <see cref="IProfession"/> to check.</param>
    /// <returns><see langword="true"/> if the <paramref name="location"/> has at least one <see cref="Farmer"/> with the specified <paramref name="profession"/>, otherwise <see langword="false"/>.</returns>
    public static bool DoesAnyPlayerHereHaveProfession(this GameLocation location, IProfession profession)
    {
        if (!Context.IsMultiplayer && location.Equals(Game1.currentLocation))
        {
            return Game1.player.HasProfession(profession);
        }

        return location.farmers.Any(farmer => farmer.HasProfession(profession));
    }

    /// <summary>
    ///     Determines whether any <see cref="Farmer"/> in this <paramref name="location"/> has the specified
    ///     <paramref name="profession"/> and gets a <see cref="List{T}"/> of those <see cref="Farmer"/>s.
    /// </summary>
    /// <param name="location">The <see cref="GameLocation"/>.</param>
    /// <param name="profession">The <see cref="IProfession"/> to check.</param>
    /// <param name="farmers">All the farmer instances in the location with the given profession.</param>
    /// <returns><see langword="true"/> if the <paramref name="location"/> has at least one <see cref="Farmer"/> with the specified <paramref name="profession"/>, otherwise <see langword="false"/>.</returns>
    public static bool DoesAnyPlayerHereHaveProfession(
        this GameLocation location, IProfession profession, out IList<Farmer> farmers)
    {
        farmers = new List<Farmer>();
        if (!Context.IsMultiplayer && location.Equals(Game1.player.currentLocation) &&
            Game1.player.HasProfession(profession))
        {
            farmers.Add(Game1.player);
        }
        else
        {
            foreach (var farmer in location.farmers.Where(farmer => farmer.HasProfession(profession)))
            {
                farmers.Add(farmer);
            }
        }

        return farmers.Count > 0;
    }

    /// <summary>Gets the raw fish data for this <paramref name="location"/> during the current game season.</summary>
    /// <param name="location">The <see cref="GameLocation"/>.</param>
    /// <returns>The raw fish data for <paramref name="location"/> and the current game season.</returns>
    public static string[] GetRawFishDataForCurrentSeason(this GameLocation location)
    {
        var locationData =
            Game1.content.Load<Dictionary<string, string>>(PathUtilities.NormalizeAssetName("Data/Locations"));
        return locationData[location.NameOrUniqueName].Split('/')[4 + Utility.getSeasonNumber(Game1.currentSeason)]
            .Split(' ');
    }

    /// <summary>Gets the raw fish data for this <paramref name="location"/> including all seasons.</summary>
    /// <param name="location">The <see cref="GameLocation"/>.</param>
    /// <returns>The raw fish data for <paramref name="location"/> and for all seasons.</returns>
    public static string[] GetRawFishDataForAllSeasons(this GameLocation location)
    {
        var locationData =
            Game1.content.Load<Dictionary<string, string>>(PathUtilities.NormalizeAssetName("Data/Locations"));
        List<string> allSeasonFish = new();
        for (var i = 0; i < 4; ++i)
        {
            var seasonalFishData = locationData[location.NameOrUniqueName].Split('/')[4 + i].Split(' ');
            if (seasonalFishData.Length > 1)
            {
                allSeasonFish.AddRange(seasonalFishData);
            }
        }

        return allSeasonFish.ToArray();
    }

    /// <summary>Determines whether this <paramref name="location"/> is a dungeon.</summary>
    /// <param name="location">The <see cref="GameLocation"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="location"/> is a <see cref="MineShaft"/> or one of several recognized dungeon locations, otherwise <see langword="false"/>.</returns>
    /// <remarks>Includes locations from Stardew Valley Expanded, Ridgeside Village and Moon Misadventures.</remarks>
    public static bool IsDungeon(this GameLocation location)
    {
        return location is MineShaft or BugLand or VolcanoDungeon ||
               location.NameOrUniqueName.ContainsAnyOf(
                   "CrimsonBadlands",
                   "DeepWoods",
                   "Highlands",
                   "RidgeForest",
                   "SpiritRealm",
                   "AsteroidsDungeon");
    }

    /// <summary>Determines whether this <paramref name="location"/> has spawned enemies.</summary>
    /// <param name="location">The <see cref="GameLocation"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="location"/> is has at least one living monster and is not a <see cref="SlimeHutch"/>, otherwise <see langword="false"/>.</returns>
    public static bool HasMonsters(this GameLocation location)
    {
        return location.characters.OfType<Monster>().Any() && location is not SlimeHutch;
    }

    /// <summary>Determines whether a <paramref name="tile"/> on a map is valid for spawning diggable treasure.</summary>
    /// <param name="location">The <see cref="GameLocation"/>.</param>
    /// <param name="tile">The tile to check.</param>
    /// <returns><see langword="true"/> if the <paramref name="tile"/> is completely clear of any <see cref="SObject"/>, <see cref="TerrainFeature"/> or other map property that would make it inaccessible, otherwise <see langword="false"/>.</returns>
    public static bool IsTileValidForTreasure(this GameLocation location, Vector2 tile)
    {
        return (!location.objects.TryGetValue(tile, out var o) || o == null) &&
               location.doesTileHaveProperty((int)tile.X, (int)tile.Y, "Spawnable", "Back") != null &&
               !location.doesEitherTileOrTileIndexPropertyEqual((int)tile.X, (int)tile.Y, "Spawnable", "Back", "F") &&
               location.isTileLocationTotallyClearAndPlaceable(tile) &&
               location.getTileIndexAt((int)tile.X, (int)tile.Y, "AlwaysFront") == -1 &&
               location.getTileIndexAt((int)tile.X, (int)tile.Y, "Front") == -1 && !location.isBehindBush(tile) &&
               !location.isBehindTree(tile);
    }

    /// <summary>Determines whether a <paramref name="tile"/> is clear of <see cref="Debris"/>.</summary>
    /// <param name="location">The <see cref="GameLocation"/>.</param>
    /// <param name="tile">The tile to check.</param>
    /// <returns><see langword="true"/> if the <paramref name="tile"/> is clear of <see cref="Debris"/>, otherwise <see langword="false"/>.</returns>
    public static bool IsTileClearOfDebris(this GameLocation location, Vector2 tile)
    {
        return (from debris in location.debris
            where debris.item is not null && debris.Chunks.Count > 0
            select new Vector2(
                (int)(debris.Chunks[0].position.X / Game1.tileSize) + 1,
                (int)(debris.Chunks[0].position.Y / Game1.tileSize) + 1))
            .All(debrisTile => debrisTile != tile);
    }

    /// <summary>Forces a <paramref name="tile"/> to be susceptible to a <see cref="StardewValley.Tools.Hoe"/>.</summary>
    /// <param name="location">The <see cref="GameLocation"/>.</param>
    /// <param name="tile">The tile to change.</param>
    /// <returns><see langword="true"/> if the <paramref name="tile"/>'s "Diggable" property was changed, otherwise <see langword="false"/>.</returns>
    public static bool MakeTileDiggable(this GameLocation location, Vector2 tile)
    {
        var (x, y) = tile;
        if (location.doesTileHaveProperty((int)x, (int)y, "Diggable", "Back") is not null)
        {
            return true;
        }

        var digSpot = new Location((int)x * Game1.tileSize, (int)y * Game1.tileSize);
        location.Map.GetLayer("Back").PickTile(digSpot, Game1.viewport.Size).Properties["Diggable"] = true;
        return false;
    }
}
