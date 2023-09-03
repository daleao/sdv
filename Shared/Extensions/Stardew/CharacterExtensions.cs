namespace DaLion.Shared.Extensions.Stardew;

#region using directives

using System.Collections.Generic;
using System.Linq;
using DaLion.Shared.ModData;
using Microsoft.Xna.Framework;
using StardewValley.Buildings;
using StardewValley.Locations;
using StardewValley.TerrainFeatures;

#endregion using directives

/// <summary>Extensions for the <see cref="Character"/> class.</summary>
public static class CharacterExtensions
{
    /// <summary>
    ///     Gets the tile distance between this <paramref name="character"/> and the target <paramref name="building"/>
    ///     in the same <see cref="GameLocation"/>.
    /// </summary>
    /// <param name="character">The <see cref="Character"/>.</param>
    /// <param name="building">The target <see cref="Building"/>.</param>
    /// <returns>The tile distance between <paramref name="character"/> and <paramref name="building"/>.</returns>
    public static double DistanceTo(this Character character, Building building)
    {
        return (character.getTileLocation() - new Vector2(building.tileX.Value, building.tileY.Value)).Length();
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
        return (character.getTileLocation() - other.getTileLocation()).Length();
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
        return (character.getTileLocation() - obj.TileLocation).Length();
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
        return (character.getTileLocation() - terrainFeature.currentTileLocation).Length();
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
        if (character.currentLocation is not BuildableGameLocation buildable)
        {
            distance = 0;
            return null;
        }

        predicate ??= _ => true;
        candidates ??= buildable.buildings
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

    /// <inheritdoc cref="ModDataIO.Read(Character, string, string, string)"/>
    public static string Read(this Character character, string field, string defaultValue = "", string modId = "")
    {
        return ModDataIO.Read(character, field, defaultValue, modId);
    }

    /// <inheritdoc cref="ModDataIO.Read{T}(Character, string, T, string)"/>
    public static T Read<T>(this Character character, string field, T defaultValue = default, string modId = "")
        where T : struct
    {
        return ModDataIO.Read(character, field, defaultValue, modId);
    }

    /// <inheritdoc cref="ModDataIO.Write(Character, string, string?)"/>
    public static void Write(this Character character, string field, string? value)
    {
        ModDataIO.Write(character, field, value);
    }

    /// <inheritdoc cref="ModDataIO.WriteIfNotExists(Character, string, string?)"/>
    public static void WriteIfNotExists(this Character character, string field, string? value)
    {
        ModDataIO.WriteIfNotExists(character, field, value);
    }

    /// <inheritdoc cref="ModDataIO.Append(Character, string, string, string)"/>
    public static void Append(this Character character, string field, string value, string separator = ",")
    {
        ModDataIO.Append(character, field, value, separator);
    }

    /// <inheritdoc cref="ModDataIO.Increment{T}(Character, string, T)"/>
    public static void Increment<T>(this Character character, string field, T amount)
        where T : struct
    {
        ModDataIO.Increment(character, field, amount);
    }

    /// <inheritdoc cref="ModDataIO.Increment{T}(Character, string)"/>
    public static void Increment(this Character character, string field)
    {
        ModDataIO.Increment(character, field, 1);
    }
}
