namespace DaLion.Common.Extensions.Stardew;

#region using directives

using System;
using System.Collections.Generic;
using System.Linq;
using DaLion.Common.ModData;
using Microsoft.Xna.Framework;
using StardewValley.Buildings;
using StardewValley.Locations;
using StardewValley.TerrainFeatures;

#endregion using directives

/// <summary>Extensions for the <see cref="TerrainFeature"/> class.</summary>
public static class TerrainFeatureExtensions
{
    /// <summary>
    ///     Gets the tile distance between this <paramref name="terrainFeature"/> and the target
    ///     <paramref name="building"/> in the same <see cref="GameLocation"/>.
    /// </summary>
    /// <param name="terrainFeature">The <see cref="TerrainFeature"/>.</param>
    /// <param name="building">The target <see cref="Building"/>.</param>
    /// <returns>The tile distance between <paramref name="terrainFeature"/> and <paramref name="building"/>.</returns>
    public static double DistanceTo(this TerrainFeature terrainFeature, Building building)
    {
        return (terrainFeature.currentTileLocation - new Vector2(building.tileX.Value, building.tileY.Value)).Length();
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
        return (terrainFeature.currentTileLocation - character.getTileLocation()).Length();
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
        return (terrainFeature.currentTileLocation - obj.TileLocation).Length();
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
        return (terrainFeature.currentTileLocation - other.currentTileLocation).Length();
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
        this TerrainFeature terrainFeature, IEnumerable<TBuilding>? candidates = null, Func<TBuilding, bool>? predicate = null)
        where TBuilding : Building
    {
        if (terrainFeature.currentLocation is not BuildableGameLocation buildable)
        {
            return null;
        }

        predicate ??= _ => true;
        var candidatesArr = candidates?.ToArray() ?? buildable.buildings.OfType<TBuilding>().Where(t => predicate(t)).ToArray();
        var distanceToClosest = double.MaxValue;
        switch (candidatesArr.Length)
        {
            case 0:
                return null;
            case 1:
                return candidatesArr[0];
        }

        TBuilding? closest = null;
        foreach (var candidate in candidatesArr)
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
        this TerrainFeature terrainFeature, IEnumerable<TCharacter>? candidates = null, Func<TCharacter, bool>? predicate = null)
        where TCharacter : Character
    {
        predicate ??= _ => true;
        var candidatesArr = candidates?.ToArray() ??
                            terrainFeature.currentLocation.characters.OfType<TCharacter>().Where(t => predicate(t)).ToArray();
        var distanceToClosest = double.MaxValue;
        switch (candidatesArr.Length)
        {
            case 0:
                return null;
            case 1:
                return candidatesArr[0];
        }

        TCharacter? closest = null;
        foreach (var candidate in candidatesArr)
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
        this TerrainFeature terrainFeature, IEnumerable<Farmer>? candidates = null, Func<Farmer, bool>? predicate = null)
    {
        predicate ??= _ => true;
        var candidatesArr = candidates?.ToArray() ??
                            terrainFeature.currentLocation.farmers.Where(f => predicate(f)).ToArray();
        var distanceToClosest = double.MaxValue;
        switch (candidatesArr.Length)
        {
            case 0:
                return null;
            case 1:
                return candidatesArr[0];
        }

        Farmer? closest = null;
        foreach (var candidate in candidatesArr)
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
        this TerrainFeature terrainFeature, IEnumerable<TObject>? candidates = null, Func<TObject, bool>? predicate = null)
        where TObject : SObject
    {
        predicate ??= _ => true;
        var candidatesArr = candidates?.ToArray() ??
                            terrainFeature.currentLocation.Objects.Values.OfType<TObject>().Where(o => predicate(o))
                                .ToArray();
        var distanceToClosest = double.MaxValue;
        switch (candidatesArr.Length)
        {
            case 0:
                return null;
            case 1:
                return candidatesArr[0];
        }

        TObject? closest = null;
        foreach (var candidate in candidatesArr)
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
        this TerrainFeature terrainFeature, IEnumerable<TTerrainFeature>? candidates = null, Func<TTerrainFeature, bool>? predicate = null)
        where TTerrainFeature : TerrainFeature
    {
        predicate ??= _ => true;
        var candidatesArr = candidates?.ToArray() ?? terrainFeature.currentLocation.terrainFeatures.Values.OfType<TTerrainFeature>()
            .Where(t => predicate(t)).ToArray();
        var distanceToClosest = double.MaxValue;
        switch (candidatesArr.Length)
        {
            case 0:
                return null;
            case 1:
                return candidatesArr[0];
        }

        TTerrainFeature? closest = null;
        foreach (var candidate in candidatesArr)
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

    /// <inheritdoc cref="ModDataIO.Read(TerrainFeature, string, string, string)"/>
    public static string Read(
        this TerrainFeature terrainFeature, string field, string defaultValue = "", string modId = "")
    {
        return ModDataIO.Read(terrainFeature, field, defaultValue, modId);
    }

    /// <inheritdoc cref="ModDataIO.Read{T}(TerrainFeature, string, T, string)"/>
    public static T Read<T>(
        this TerrainFeature terrainFeature, string field, T defaultValue = default, string modId = "")
        where T : struct
    {
        return ModDataIO.Read(terrainFeature, field, defaultValue, modId);
    }

    /// <inheritdoc cref="ModDataIO.Write(TerrainFeature, string, string?)"/>
    public static void Write(this TerrainFeature terrainFeature, string field, string? value)
    {
        ModDataIO.Write(terrainFeature, field, value);
    }

    /// <inheritdoc cref="ModDataIO.WriteIfNotExists(TerrainFeature, string, string?)"/>
    public static void WriteIfNotExists(this TerrainFeature terrainFeature, string field, string? value)
    {
        ModDataIO.WriteIfNotExists(terrainFeature, field, value);
    }

    /// <inheritdoc cref="ModDataIO.Append(TerrainFeature, string, string, string)"/>
    public static void Append(this TerrainFeature terrainFeature, string field, string value, string separator = ",")
    {
        ModDataIO.Append(terrainFeature, field, value, separator);
    }

    /// <inheritdoc cref="ModDataIO.Increment{T}(TerrainFeature, string, T)"/>
    public static void Increment<T>(this TerrainFeature terrainFeature, string field, T amount)
        where T : struct
    {
        ModDataIO.Increment(terrainFeature, field, amount);
    }

    /// <inheritdoc cref="ModDataIO.Increment{T}(TerrainFeature, string)"/>
    public static void Increment(this TerrainFeature terrainFeature, string field)
    {
        ModDataIO.Increment(terrainFeature, field, 1);
    }
}
