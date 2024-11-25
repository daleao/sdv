namespace DaLion.Shared.Extensions.Stardew;

#region using directives

using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using StardewValley.Buildings;
using StardewValley.TerrainFeatures;

#endregion using directives

/// <summary>Extensions for the <see cref="TerrainFeature"/> class.</summary>
public static class TerrainFeatureExtensions
{
    /// <summary>
    ///     Gets the squared pixel distance between this <paramref name="terrainFeature"/> and the target <paramref name="position"/>.
    /// </summary>
    /// <param name="terrainFeature">The <see cref="Character"/>.</param>
    /// <param name="position">The target tile.</param>
    /// <returns>The squared pixel distance between <paramref name="terrainFeature"/> and the <paramref name="position"/>.</returns>
    public static float SquaredPixelDistance(this TerrainFeature terrainFeature, Vector2 position)
    {
        var dx = (terrainFeature.Tile.X * Game1.tileSize) - position.X;
        var dy = (terrainFeature.Tile.Y * Game1.tileSize) - position.Y;
        return (dx * dx) + (dy * dy);
    }

    /// <summary>
    ///     Gets the squared tile distance between this <paramref name="terrainFeature"/> and the target <paramref name="tile"/>.
    /// </summary>
    /// <param name="terrainFeature">The <see cref="Character"/>.</param>
    /// <param name="tile">The target tile.</param>
    /// <returns>The squared tile distance between <paramref name="terrainFeature"/> and the <paramref name="tile"/>.</returns>
    public static float SquaredTileDistance(this TerrainFeature terrainFeature, Vector2 tile)
    {
        var dx = terrainFeature.Tile.X - tile.X;
        var dy = terrainFeature.Tile.Y - tile.Y;
        return (dx * dx) + (dy * dy);
    }

    /// <summary>
    ///     Finds the closest tile from among the specified <paramref name="candidates"/> to this
    ///     <paramref name="terrainFeature"/>.
    /// </summary>
    /// <param name="terrainFeature">The <see cref="TerrainFeature"/>.</param>
    /// <param name="candidates">The candidate <see cref="TerrainFeature"/>s, if already available.</param>
    /// <returns>The closest tile from among the specified <paramref name="candidates"/> to this <paramref name="terrainFeature"/>.</returns>
    public static Vector2 GetClosestTile(this TerrainFeature terrainFeature, IEnumerable<Vector2> candidates)
    {
        var closest = terrainFeature.Tile;
        var distanceToClosest = float.MaxValue;
        foreach (var candidate in candidates)
        {
            var distanceToThisCandidate = terrainFeature.SquaredTileDistance(candidate);
            if (distanceToThisCandidate >= distanceToClosest)
            {
                continue;
            }

            closest = candidate;
            distanceToClosest = distanceToThisCandidate;
        }

        return closest;
    }

    /// <summary>
    ///     Finds the closest target from among the specified <paramref name="candidates"/> to this
    ///     <paramref name="terrainFeature"/>.
    /// </summary>
    /// <typeparam name="T">The target type.</typeparam>
    /// <param name="terrainFeature">The <see cref="TerrainFeature"/>.</param>
    /// <param name="candidates">The candidate <typeparamref name="T"/>s, if already available.</param>
    /// <param name="getPosition">A delegate to retrieve the tile coordinates of <typeparamref name="T"/>.</param>
    /// <param name="distance">The actual tile distance to the closest candidate found.</param>
    /// <param name="predicate">An optional condition with which to filter out candidates.</param>
    /// <returns>The closest target from among the specified <paramref name="candidates"/> to this <paramref name="terrainFeature"/>.</returns>
    public static T? GetClosest<T>(
        this TerrainFeature terrainFeature,
        IEnumerable<T> candidates,
        Func<T, Vector2> getPosition,
        out float distance,
        Func<T, bool>? predicate = null)
        where T : class
    {
        predicate ??= _ => true;
        candidates = candidates.Where(c => predicate(c));
        T? closest = null;
        var distanceToClosest = float.MaxValue;
        foreach (var candidate in candidates.Skip(1))
        {
            var distanceToThisCandidate = terrainFeature.SquaredPixelDistance(getPosition(candidate));
            if (distanceToThisCandidate >= distanceToClosest)
            {
                continue;
            }

            closest = candidate;
            distanceToClosest = distanceToThisCandidate;
        }

        distance = distanceToClosest;
        return closest;
    }

    /// <summary>
    ///     Finds the closest <see cref="Building"/> of subtype <typeparamref name="TBuilding"/> to this
    ///     <paramref name="terrainFeature"/> in the current <see cref="GameLocation"/>.
    /// </summary>
    /// <typeparam name="TBuilding">A subtype of <see cref="Building"/>.</typeparam>
    /// <param name="terrainFeature">The <see cref="TerrainFeature"/>.</param>
    /// <param name="candidates">The candidate <typeparamref name="TBuilding"/>s, if already available.</param>
    /// <param name="predicate">An optional condition with which to filter out candidates.</param>
    /// <returns>The <see cref="Building"/> of type <typeparamref name="TBuilding"/> with the minimal distance to <paramref name="terrainFeature"/>.</returns>
    public static TBuilding? GetClosestBuilding<TBuilding>(
        this TerrainFeature terrainFeature,
        IEnumerable<TBuilding>? candidates = null,
        Func<TBuilding, bool>? predicate = null)
        where TBuilding : Building
    {
        candidates ??= terrainFeature.Location.buildings.OfType<TBuilding>();
        return terrainFeature.GetClosest(candidates, b => b.GetBoundingBox().Center.ToVector2(), out _, predicate);
    }

    /// <summary>
    ///     Finds the closest <see cref="NPC"/> of subtype <typeparamref name="TCharacter"/> to this
    ///     <paramref name="terrainFeature"/> in the current <see cref="GameLocation"/>.
    /// </summary>
    /// <typeparam name="TCharacter">A subtype of <see cref="Character"/>.</typeparam>
    /// <param name="terrainFeature">The <see cref="TerrainFeature"/>.</param>
    /// <param name="candidates">The candidate <typeparamref name="TCharacter"/>s, if already available.</param>
    /// <param name="predicate">An optional condition with which to filter out candidates.</param>
    /// <returns>The <see cref="Character"/> of type <typeparamref name="TCharacter"/> with the minimal distance to <paramref name="terrainFeature"/>.</returns>
    public static TCharacter? GetClosestCharacter<TCharacter>(
        this TerrainFeature terrainFeature,
        IEnumerable<TCharacter>? candidates = null,
        Func<TCharacter, bool>? predicate = null)
        where TCharacter : Character
    {
        candidates ??= terrainFeature.Location.characters.OfType<TCharacter>();
        return terrainFeature.GetClosest(candidates, c => c.Position, out _, predicate);
    }

    /// <summary>
    ///     Finds the closest <see cref="Farmer"/> to this <paramref name="terrainFeature"/> in the current
    ///     <see cref="GameLocation"/>.
    /// </summary>
    /// <param name="terrainFeature">The <see cref="TerrainFeature"/>.</param>
    /// <param name="candidates">The candidate <see cref="Farmer"/>s, if already available.</param>
    /// <param name="predicate">An optional condition with which to filter out candidates.</param>
    /// <returns>The <see cref="Farmer"/> with the minimal distance to <paramref name="terrainFeature"/>.</returns>
    public static Farmer? GetClosestFarmer(
        this TerrainFeature terrainFeature,
        IEnumerable<Farmer>? candidates = null,
        Func<Farmer, bool>? predicate = null)
    {
        candidates ??= terrainFeature.Location.farmers;
        return terrainFeature.GetClosest(candidates, f => f.Position, out _, predicate);
    }

    /// <summary>
    ///     Finds the closest <see cref="SObject"/> of subtype <typeparamref name="TObject"/> to this
    ///     <paramref name="terrainFeature"/> in the current <see cref="GameLocation"/>.
    /// </summary>
    /// <typeparam name="TObject">A subtype of <see cref="SObject"/>.</typeparam>
    /// <param name="terrainFeature">The <see cref="TerrainFeature"/>.</param>
    /// <param name="candidates">The candidate <typeparamref name="TObject"/>s, if already available.</param>
    /// <param name="predicate">An optional condition with which to filter out candidates.</param>
    /// <returns>The <see cref="SObject"/> of type <typeparamref name="TObject"/> with the minimal distance to <paramref name="terrainFeature"/>.</returns>
    public static TObject? GetClosestObject<TObject>(
        this TerrainFeature terrainFeature,
        IEnumerable<TObject>? candidates = null,
        Func<TObject, bool>? predicate = null)
        where TObject : SObject
    {
        candidates ??= terrainFeature.Location.Objects.Values.OfType<TObject>();
        return terrainFeature.GetClosest(candidates, o => o.TileLocation * Game1.tileSize, out _, predicate);
    }

    /// <summary>
    ///     Finds the closest <see cref="TerrainFeature"/> to this one in the current <see cref="GameLocation"/>, and of
    ///     the specified subtype.
    /// </summary>
    /// <typeparam name="TTerrainFeature">A subtype of <see cref="SObject"/>.</typeparam>
    /// <param name="terrainFeature">The <see cref="TerrainFeature"/>.</param>
    /// <param name="candidates">The candidate <typeparamref name="TTerrainFeature"/>s, if already available.</param>
    /// <param name="predicate">An optional condition with which to filter out candidates.</param>
    /// <returns>The <see cref="TerrainFeature"/> of type <typeparamref name="TTerrainFeature"/> with the minimal distance to <paramref name="terrainFeature"/>.</returns>
    public static TTerrainFeature? GetClosestTerrainFeature<TTerrainFeature>(
        this TerrainFeature terrainFeature,
        IEnumerable<TTerrainFeature>? candidates = null,
        Func<TTerrainFeature, bool>? predicate = null)
        where TTerrainFeature : TerrainFeature
    {
        predicate ??= _ => true;
        candidates ??= terrainFeature.Location.terrainFeatures.Values.OfType<TTerrainFeature>();
        return terrainFeature.GetClosest(
            candidates,
            t => t.Tile * Game1.tileSize,
            out _,
            t => !ReferenceEquals(t, terrainFeature) && predicate(t));
    }
}
