namespace DaLion.Professions.Framework.Extensions;

#region using directives

using System.Collections.Generic;
using System.Linq;
using DaLion.Shared.Extensions;
using DaLion.Shared.Extensions.Stardew;
using Microsoft.Xna.Framework;
using StardewValley.Locations;
using StardewValley.TerrainFeatures;
using xTile.Dimensions;

#endregion using directives

/// <summary>Extensions for the <see cref="GameLocation"/> class.</summary>
internal static class GameLocationExtensions
{
    /// <summary>
    ///     Determines whether any <see cref="Farmer"/> in this <paramref name="location"/> has the specified
    ///     <paramref name="profession"/>.
    /// </summary>
    /// <param name="location">The <see cref="GameLocation"/>.</param>
    /// <param name="profession">The <see cref="IProfession"/> to check.</param>
    /// <param name="prestiged">Whether to check for the prestiged variant.</param>
    /// <returns><see langword="true"/> if the <paramref name="location"/> has at least one <see cref="Farmer"/> with the specified <paramref name="profession"/>, otherwise <see langword="false"/>.</returns>
    internal static bool DoesAnyPlayerHereHaveProfession(this GameLocation location, IProfession profession, bool prestiged = false)
    {
        return !Context.IsMultiplayer && location.Equals(Game1.currentLocation)
            ? Game1.player.HasProfession(profession, prestiged)
            : location.farmers.Any(farmer => farmer.HasProfession(profession, prestiged));
    }

    /// <summary>
    ///     Determines whether any <see cref="Farmer"/> in this <paramref name="location"/> has the specified
    ///     <paramref name="profession"/> and gets a <see cref="List{T}"/> of those <see cref="Farmer"/>s.
    /// </summary>
    /// <param name="location">The <see cref="GameLocation"/>.</param>
    /// <param name="profession">The <see cref="IProfession"/> to check.</param>
    /// <param name="players">All the farmer instances in the location with the given profession.</param>
    /// <param name="prestiged">Whether to check for the prestiged variant.</param>
    /// <returns><see langword="true"/> if the <paramref name="location"/> has at least one <see cref="Farmer"/> with the specified <paramref name="profession"/>, otherwise <see langword="false"/>.</returns>
    internal static bool DoesAnyPlayerHereHaveProfession(
        this GameLocation location, IProfession profession, out IEnumerable<Farmer> players, bool prestiged = false)
    {
        if (!Context.IsMultiplayer)
        {
            if (location.Equals(Game1.player.currentLocation) && Game1.player.HasProfession(profession, prestiged))
            {
                players = Game1.player.Collect();
                return true;
            }

            players = [];
            return false;
        }

        players = location.farmers.Where(f => f.HasProfession(profession, prestiged));
        return players.Any();
    }

    /// <summary>Checks whether the <paramref name="location"/> is suitable for a <see cref="TreasureHunts.ScavengerHunt"/>.</summary>
    /// <param name="location">The <see cref="GameLocation"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="location"/> is suitable for a <see cref="TreasureHunts.ScavengerHunt"/>, otherwise <see langword="false"/>.</returns>
    internal static bool IsSuitableScavengerHuntLocation(this GameLocation location)
    {
        return location.IsOutdoors && (!location.IsFarm || Config.AllowScavengerHuntsOnFarm) && location.currentEvent is null;
    }

    /// <summary>Checks whether the <paramref name="location"/> is suitable for a <see cref="TreasureHunts.ProspectorHunt"/>.</summary>
    /// <param name="location">The <see cref="GameLocation"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="location"/> is suitable for a <see cref="TreasureHunts.ProspectorHunt"/>, otherwise <see langword="false"/>.</returns>
    internal static bool IsSuitablePropsectorHuntLocation(this GameLocation location)
    {
        return ((location is MineShaft shaft && !shaft.IsTreasureOrSafeRoom()) || location is VolcanoDungeon) && location.currentEvent is null;
    }

    /// <summary>Determines whether a <paramref name="tile"/> on a map is valid for spawning diggable treasure.</summary>
    /// <param name="location">The <see cref="GameLocation"/>.</param>
    /// <param name="tile">The tile to check.</param>
    /// <returns><see langword="true"/> if the <paramref name="tile"/> is completely clear of any <see cref="SObject"/>, <see cref="TerrainFeature"/> or other map property that would make it inaccessible, otherwise <see langword="false"/>.</returns>
    internal static bool IsTileValidForTreasure(this GameLocation location, Vector2 tile)
    {
        return (!location.objects.TryGetValue(tile, out var @object) || @object == null) &&
               location.CanItemBePlacedHere(tile) &&
               location.getTileIndexAt((int)tile.X, (int)tile.Y, "AlwaysFront") == -1 &&
               location.getTileIndexAt((int)tile.X, (int)tile.Y, "Front") == -1 && !location.isBehindBush(tile) &&
               !location.isBehindTree(tile);
    }

    /// <summary>Determines whether a <paramref name="tile"/> is clear of <see cref="Debris"/>.</summary>
    /// <param name="location">The <see cref="GameLocation"/>.</param>
    /// <param name="tile">The tile to check.</param>
    /// <returns><see langword="true"/> if the <paramref name="tile"/> is clear of <see cref="Debris"/>, otherwise <see langword="false"/>.</returns>
    internal static bool IsTileClearOfDebris(this GameLocation location, Vector2 tile)
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
    internal static void MakeTileDiggable(this GameLocation location, Vector2 tile)
    {
        var (x, y) = tile;
        if (location.doesTileHaveProperty((int)x, (int)y, "Diggable", "Back") is not null)
        {
            return;
        }

        var digSpot = new Location((int)x * Game1.tileSize, (int)y * Game1.tileSize);
        location.Map.GetLayer("Back").PickTile(digSpot, Game1.viewport.Size).Properties["Diggable"] = true;
    }
}
