namespace DaLion.Shared.Extensions.Stardew;

#region using directives

using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using StardewValley.Buildings;
using StardewValley.TerrainFeatures;

#endregion using directives

/// <summary>Extensions for the <see cref="Character"/> class.</summary>
public static class CharacterExtensions
{
    /// <summary>
    ///     Gets the tile distance between this <paramref name="character"/> and the target <paramref name="tile"/>.
    /// </summary>
    /// <param name="character">The <see cref="Character"/>.</param>
    /// <param name="tile">The target tile.</param>
    /// <returns>The tile distance between <paramref name="character"/> and the <paramref name="tile"/>.</returns>
    public static double DistanceTo(this Character character, Vector2 tile)
    {
        return (character.Tile - tile).Length();
    }

    /// <summary>
    ///     Gets the tile distance between this <paramref name="character"/> and the target <paramref name="building"/>
    ///     in the same <see cref="GameLocation"/>.
    /// </summary>
    /// <param name="character">The <see cref="Character"/>.</param>
    /// <param name="building">The target <see cref="Building"/>.</param>
    /// <returns>The tile distance between <paramref name="character"/> and <paramref name="building"/>.</returns>
    public static double DistanceTo(this Character character, Building building)
    {
        return character.DistanceTo(new Vector2(building.tileX.Value, building.tileY.Value));
    }

    /// <summary>
    ///     Gets the tile distance between this <paramref name="character"/> and some <paramref name="other"/> in the
    ///     same <see cref="GameLocation"/>.
    /// </summary>
    /// <param name="character">The <see cref="Character"/>.</param>
    /// <param name="other">The target <see cref="Character"/>.</param>
    /// <returns>The tile distance between <paramref name="character"/> and <paramref name="other"/>.</returns>
    public static double DistanceTo(this Character character, Character other)
    {
        return character.DistanceTo(other.Tile);
    }

    /// <summary>
    ///     Gets the tile distance between this <paramref name="character"/> and the target <paramref name="obj"/>
    ///     in the same <see cref="GameLocation"/>.
    /// </summary>
    /// <param name="character">The <see cref="Character"/>.</param>
    /// <param name="obj">The target <see cref="SObject"/>.</param>
    /// <returns>The tile distance between <paramref name="character"/> and <paramref name="obj"/>.</returns>
    public static double DistanceTo(this Character character, SObject obj)
    {
        return character.DistanceTo(obj.TileLocation);
    }

    /// <summary>
    ///     Gets the tile distance between this <paramref name="character"/> and the target
    ///     <paramref name="terrainFeature"/> in the same <see cref="GameLocation"/>.
    /// </summary>
    /// <param name="character">The <see cref="Character"/>.</param>
    /// <param name="terrainFeature">The target <see cref="TerrainFeature"/>.</param>
    /// <returns>The tile distance between <paramref name="character"/> and <paramref name="terrainFeature"/>.</returns>
    public static double DistanceTo(this Character character, TerrainFeature terrainFeature)
    {
        return character.DistanceTo(terrainFeature.Tile);
    }

    /// <summary>
    ///     Finds the closest target from among the specified <paramref name="candidates"/> to this
    ///     <paramref name="character"/>.
    /// </summary>
    /// <typeparam name="T">The target type.</typeparam>
    /// <param name="character">The <see cref="Character"/>.</param>
    /// <param name="candidates">The candidate <see cref="Building"/>s, if already available.</param>
    /// <param name="getPosition">A delegate to retrieve the tile coordinates of <typeparamref name="T"/>.</param>
    /// <param name="distance">The distance to the closest <see cref="Building"/>, in number of tiles.</param>
    /// <param name="predicate">An optional condition with which to filter out candidates.</param>
    /// <returns>The closest target from among the specified <paramref name="candidates"/> to this <paramref name="character"/>.</returns>
    public static T? GetClosest<T>(
        this Character character,
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
        foreach (var candidate in candidates)
        {
            var distanceToThisCandidate = character.DistanceTo(getPosition(candidate));
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
    ///     <paramref name="character"/> in the current <see cref="GameLocation"/>.
    /// </summary>
    /// <typeparam name="TBuilding">A sub-type of <see cref="Building"/>.</typeparam>
    /// <param name="character">The <see cref="Character"/>.</param>
    /// <param name="distance">The distance to the closest <see cref="Building"/>, in number of tiles.</param>
    /// <param name="candidates">The candidate <see cref="Building"/>s, if already available.</param>
    /// <param name="predicate">An optional condition with which to filter out candidates.</param>
    /// <returns>The <see cref="Building"/> of type <typeparamref name="TBuilding"/> with the minimal distance to <paramref name="character"/>.</returns>
    public static TBuilding? GetClosestBuilding<TBuilding>(
        this Character character,
        out double distance,
        IEnumerable<TBuilding>? candidates = null,
        Func<TBuilding, bool>? predicate = null)
        where TBuilding : Building
    {
        predicate ??= _ => true;
        candidates ??= character.currentLocation.buildings
            .OfType<TBuilding>()
            .Where(c => predicate(c));
        TBuilding? closest = null;
        var distanceToClosest = double.MaxValue;
        foreach (var candidate in candidates)
        {
            var distanceToThisCandidate = character.DistanceTo(candidate);
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
    ///     Finds the closest <see cref="Character"/> to this one in the current <see cref="GameLocation"/>, and of the
    ///     specified sub-type.
    /// </summary>
    /// <typeparam name="TCharacter">A sub-type of <see cref="Character"/>.</typeparam>
    /// <param name="character">The <see cref="Character"/>.</param>
    /// <param name="distance">The distance to the closest <see cref="Building"/>, in number of tiles.</param>
    /// <param name="candidates">The candidate <see cref="Character"/>s, if already available.</param>
    /// <param name="predicate">An optional condition with which to filter out candidates.</param>
    /// <returns>The <see cref="Character"/> of type <typeparamref name="TCharacter"/> with the minimal distance to <paramref name="character"/>.</returns>
    public static TCharacter? GetClosestCharacter<TCharacter>(
        this Character character,
        out double distance,
        IEnumerable<TCharacter>? candidates = null,
        Func<TCharacter, bool>? predicate = null)
        where TCharacter : Character
    {
        predicate ??= _ => true;
        candidates ??= character.currentLocation.characters
            .OfType<TCharacter>()
            .Where(b => predicate(b));
        TCharacter? closest = null;
        var distanceToClosest = double.MaxValue;
        foreach (var candidate in candidates)
        {
            var distanceToThisCandidate = character.DistanceTo(candidate);
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
    ///     Find the closest <see cref="Farmer"/> to this <paramref name="character"/> in the current
    ///     <see cref="GameLocation"/>.
    /// </summary>
    /// <param name="character">The <see cref="Character"/>.</param>
    /// <param name="distance">The distance to the closest <see cref="Building"/>, in number of tiles.</param>
    /// <param name="candidates">The candidate <see cref="Farmer"/>s, if already available.</param>
    /// <param name="predicate">An optional condition with which to filter out candidates.</param>
    /// <returns>The <see cref="Farmer"/> with the minimal distance to <paramref name="character"/>.</returns>
    /// <remarks>This version is required as <see cref="Farmer"/> references are stored in a different field of <see cref="GameLocation"/>.</remarks>
    public static Farmer? GetClosestFarmer(
        this Character character,
        out double distance,
        IEnumerable<Farmer>? candidates = null,
        Func<Farmer, bool>? predicate = null)
    {
        predicate ??= _ => true;
        candidates ??= character.currentLocation.farmers.Where(f => f != character && predicate(f));
        Farmer? closest = null;
        var distanceToClosest = double.MaxValue;
        foreach (var candidate in candidates)
        {
            var distanceToThisCandidate = character.DistanceTo(candidate);
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
    ///     Find the closest <see cref="SObject"/> of sub-type <typeparamref name="TObject"/> to this
    ///     <paramref name="character"/> in the current <see cref="GameLocation"/>.
    /// </summary>
    /// <typeparam name="TObject">A sub-type of <see cref="SObject"/>.</typeparam>
    /// <param name="character">The <see cref="Character"/>.</param>
    /// <param name="distance">The distance to the closest <see cref="Building"/>, in number of tiles.</param>
    /// <param name="candidates">The candidate <see cref="SObject"/>s, if already available.</param>
    /// <param name="predicate">An optional condition with which to filter out candidates.</param>
    /// <returns>The <see cref="SObject"/> of type <typeparamref name="TObject"/> with the minimal distance to <paramref name="character"/>.</returns>
    public static TObject? GetClosestObject<TObject>(
        this Character character,
        out double distance,
        IEnumerable<TObject>? candidates = null,
        Func<TObject, bool>? predicate = null)
        where TObject : SObject
    {
        predicate ??= _ => true;
        candidates ??= character.currentLocation.Objects.Values
            .OfType<TObject>()
            .Where(o => predicate(o));
        TObject? closest = null;
        var distanceToClosest = double.MaxValue;
        foreach (var candidate in candidates)
        {
            var distanceToThisCandidate = character.DistanceTo(candidate);
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
    ///     Find the closest <see cref="TerrainFeature"/> of sub-type <typeparamref name="TTerrainFeature"/> to this
    ///     <paramref name="character"/> in the current <see cref="GameLocation"/>.
    /// </summary>
    /// <typeparam name="TTerrainFeature">A sub-type of <see cref="TerrainFeature"/>.</typeparam>
    /// <param name="character">The <see cref="Character"/>.</param>
    /// <param name="distance">The distance to the closest <see cref="Building"/>, in number of tiles.</param>
    /// <param name="candidates">The candidate <see cref="TerrainFeature"/>s if already available.</param>
    /// <param name="predicate">An optional condition with which to filter out candidates.</param>
    /// <returns>The <see cref="TerrainFeature"/> of type <typeparamref name="TTerrainFeature"/> with the minimal distance to <paramref name="character"/>.</returns>
    public static TTerrainFeature? GetClosestTerrainFeature<TTerrainFeature>(
        this Character character,
        out double distance,
        IEnumerable<TTerrainFeature>? candidates = null,
        Func<TTerrainFeature, bool>? predicate = null)
        where TTerrainFeature : TerrainFeature
    {
        predicate ??= _ => true;
        candidates ??= character.currentLocation.terrainFeatures.Values
            .OfType<TTerrainFeature>()
            .Where(t => predicate(t));
        TTerrainFeature? closest = null;
        var distanceToClosest = double.MaxValue;
        foreach (var candidate in candidates)
        {
            var distanceToThisCandidate = character.DistanceTo(candidate);
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
}
