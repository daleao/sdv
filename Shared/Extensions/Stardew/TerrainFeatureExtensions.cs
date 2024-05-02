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
    ///     Gets the tile distance between this <paramref name="terrainFeature"/> and the target <paramref name="tile"/>.
    /// </summary>
    /// <param name="terrainFeature">The <see cref="Character"/>.</param>
    /// <param name="tile">The target tile.</param>
    /// <returns>The tile distance between <paramref name="terrainFeature"/> and the <paramref name="tile"/>.</returns>
    public static double DistanceTo(this TerrainFeature terrainFeature, Vector2 tile)
    {
        return (terrainFeature.Tile - tile).Length();
    }

    /// <summary>
    ///     Gets the tile distance between this <paramref name="terrainFeature"/> and the target
    ///     <paramref name="building"/> in the same <see cref="GameLocation"/>.
    /// </summary>
    /// <param name="terrainFeature">The <see cref="TerrainFeature"/>.</param>
    /// <param name="building">The target <see cref="Building"/>.</param>
    /// <returns>The tile distance between <paramref name="terrainFeature"/> and <paramref name="building"/>.</returns>
    public static double DistanceTo(this TerrainFeature terrainFeature, Building building)
    {
        return terrainFeature.DistanceTo(new Vector2(building.tileX.Value, building.tileY.Value));
    }

    /// <summary>
    ///     Gets the tile distance between this <paramref name="terrainFeature"/> and the target
    ///     <paramref name="character"/> in the same <see cref="GameLocation"/>.
    /// </summary>
    /// <param name="terrainFeature">The <see cref="TerrainFeature"/>.</param>
    /// <param name="character">The target <see cref="Character"/>.</param>
    /// <returns>The tile distance between <paramref name="terrainFeature"/> and <paramref name="character"/>.</returns>
    public static double DistanceTo(this TerrainFeature terrainFeature, Character character)
    {
        return terrainFeature.DistanceTo(character.Tile);
    }

    /// <summary>
    ///     Get the tile distance between this <paramref name="terrainFeature"/> and the target
    ///     <paramref name="obj"/> in the same <see cref="GameLocation"/>.
    /// </summary>
    /// <param name="terrainFeature">The <see cref="TerrainFeature"/>.</param>
    /// <param name="obj">The target <see cref="SObject"/>.</param>
    /// <returns>The tile distance between <paramref name="terrainFeature"/> and <paramref name="obj"/>.</returns>
    public static double DistanceTo(this TerrainFeature terrainFeature, SObject obj)
    {
        return terrainFeature.DistanceTo(obj.TileLocation);
    }

    /// <summary>
    ///     Gets the tile distance between this <paramref name="terrainFeature"/> and some this <paramref name="other"/>
    ///     in the same <see cref="GameLocation"/>.
    /// </summary>
    /// <param name="terrainFeature">The <see cref="TerrainFeature"/>.</param>
    /// <param name="other">The target <see cref="TerrainFeature"/>.</param>
    /// <returns>The tile distance between <paramref name="terrainFeature"/> and <paramref name="other"/>.</returns>
    public static double DistanceTo(this TerrainFeature terrainFeature, TerrainFeature other)
    {
        return terrainFeature.DistanceTo(other.Tile);
    }

    /// <summary>
    ///     Finds the closest target from among the specified <paramref name="candidates"/> to this
    ///     <paramref name="terrainFeature"/>.
    /// </summary>
    /// <typeparam name="T">The target type.</typeparam>
    /// <param name="terrainFeature">The <see cref="TerrainFeature"/>.</param>
    /// <param name="candidates">The candidate <see cref="Building"/>s, if already available.</param>
    /// <param name="getPosition">A delegate to retrieve the tile coordinates of <typeparamref name="T"/>.</param>
    /// <param name="distance">The distance to the closest <see cref="Building"/>, in number of tiles.</param>
    /// <param name="predicate">An optional condition with which to filter out candidates.</param>
    /// <returns>The closest target from among the specified <paramref name="candidates"/> to this <paramref name="terrainFeature"/>.</returns>
    public static T? GetClosest<T>(
        this TerrainFeature terrainFeature,
        IEnumerable<T> candidates,
        Func<T, Vector2> getPosition,
        out double distance,
        Func<T, bool>? predicate = null)
        where T : class
    {
        predicate ??= _ => true;
        candidates = candidates.Where(c => predicate(c));
        T? closest = null;
        var distanceToClosest = double.MaxValue;
        foreach (var candidate in candidates.Skip(1))
        {
            var distanceToThisCandidate = terrainFeature.DistanceTo(getPosition(candidate));
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
    ///     Finds the closest <see cref="Building"/> of sub-type <typeparamref name="TBuilding"/> to this
    ///     <paramref name="terrainFeature"/> in the current <see cref="GameLocation"/>.
    /// </summary>
    /// <typeparam name="TBuilding">A sub-type of <see cref="Building"/>.</typeparam>
    /// <param name="terrainFeature">The <see cref="TerrainFeature"/>.</param>
    /// <param name="candidates">The candidate <see cref="Building"/>s, if already available.</param>
    /// <param name="predicate">An optional condition with which to filter out candidates.</param>
    /// <returns>The <see cref="Building"/> of type <typeparamref name="TBuilding"/> with the minimal distance to <paramref name="terrainFeature"/>.</returns>
    public static TBuilding? GetClosestBuilding<TBuilding>(
        this TerrainFeature terrainFeature,
        IEnumerable<TBuilding>? candidates = null,
        Func<TBuilding, bool>? predicate = null)
        where TBuilding : Building
    {
        predicate ??= _ => true;
        candidates ??= terrainFeature.Location.buildings
            .OfType<TBuilding>()
            .Where(t => predicate(t));
        TBuilding? closest = null;
        var distanceToClosest = double.MaxValue;
        foreach (var candidate in candidates)
        {
            var distanceToThisCandidate = terrainFeature.DistanceTo(candidate);
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
    ///     Finds the closest <see cref="NPC"/> of sub-type <typeparamref name="TCharacter"/> to this
    ///     <paramref name="terrainFeature"/> in the current <see cref="GameLocation"/>.
    /// </summary>
    /// <typeparam name="TCharacter">A sub-type of <see cref="Character"/>.</typeparam>
    /// <param name="terrainFeature">The <see cref="TerrainFeature"/>.</param>
    /// <param name="candidates">The candidate <see cref="NPC"/>s, if already available.</param>
    /// <param name="predicate">An optional condition with which to filter out candidates.</param>
    /// <returns>The <see cref="Character"/> of type <typeparamref name="TCharacter"/> with the minimal distance to <paramref name="terrainFeature"/>.</returns>
    public static TCharacter? GetClosestCharacter<TCharacter>(
        this TerrainFeature terrainFeature,
        IEnumerable<TCharacter>? candidates = null,
        Func<TCharacter, bool>? predicate = null)
        where TCharacter : Character
    {
        predicate ??= _ => true;
        candidates ??= terrainFeature.Location.characters
            .OfType<TCharacter>()
            .Where(t => predicate(t));
        TCharacter? closest = null;
        var distanceToClosest = double.MaxValue;
        foreach (var candidate in candidates)
        {
            var distanceToThisCandidate = terrainFeature.DistanceTo(candidate);
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
        predicate ??= _ => true;
        candidates ??= terrainFeature.Location.farmers.Where(f => predicate(f));
        Farmer? closest = null;
        var distanceToClosest = double.MaxValue;
        foreach (var candidate in candidates)
        {
            var distanceToThisCandidate = terrainFeature.DistanceTo(candidate);
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
    ///     Finds the closest <see cref="SObject"/> of sub-type <typeparamref name="TObject"/> to this
    ///     <paramref name="terrainFeature"/> in the current <see cref="GameLocation"/>.
    /// </summary>
    /// <typeparam name="TObject">A sub-type of <see cref="SObject"/>.</typeparam>
    /// <param name="terrainFeature">The <see cref="TerrainFeature"/>.</param>
    /// <param name="candidates">The candidate <see cref="SObject"/>s, if already available.</param>
    /// <param name="predicate">An optional condition with which to filter out candidates.</param>
    /// <returns>The <see cref="SObject"/> of type <typeparamref name="TObject"/> with the minimal distance to <paramref name="terrainFeature"/>.</returns>
    public static TObject? GetClosestObject<TObject>(
        this TerrainFeature terrainFeature,
        IEnumerable<TObject>? candidates = null,
        Func<TObject, bool>? predicate = null)
        where TObject : SObject
    {
        predicate ??= _ => true;
        candidates ??= terrainFeature.Location.Objects.Values
            .OfType<TObject>()
            .Where(o => predicate(o));
        TObject? closest = null;
        var distanceToClosest = double.MaxValue;
        foreach (var candidate in candidates)
        {
            var distanceToThisCandidate = terrainFeature.DistanceTo(candidate);
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
    ///     Finds the closest <see cref="TerrainFeature"/> to this one in the current <see cref="GameLocation"/>, and of
    ///     the specified sub-type.
    /// </summary>
    /// <typeparam name="TTerrainFeature">A sub-type of <see cref="SObject"/>.</typeparam>
    /// <param name="terrainFeature">The <see cref="TerrainFeature"/>.</param>
    /// <param name="candidates">The candidate <see cref="TerrainFeature"/>s, if already available.</param>
    /// <param name="predicate">An optional condition with which to filter out candidates.</param>
    /// <returns>The <see cref="TerrainFeature"/> of type <typeparamref name="TTerrainFeature"/> with the minimal distance to <paramref name="terrainFeature"/>.</returns>
    public static TTerrainFeature? GetClosestTerrainFeature<TTerrainFeature>(
        this TerrainFeature terrainFeature,
        IEnumerable<TTerrainFeature>? candidates = null,
        Func<TTerrainFeature, bool>? predicate = null)
        where TTerrainFeature : TerrainFeature
    {
        predicate ??= _ => true;
        candidates ??= terrainFeature.Location.terrainFeatures.Values
            .OfType<TTerrainFeature>()
            .Where(t => predicate(t));
        TTerrainFeature? closest = null;
        var distanceToClosest = double.MaxValue;
        foreach (var candidate in candidates)
        {
            var distanceToThisCandidate = terrainFeature.DistanceTo(candidate);
            if (distanceToThisCandidate >= distanceToClosest)
            {
                continue;
            }

            closest = candidate;
            distanceToClosest = distanceToThisCandidate;
        }

        return closest;
    }
}
