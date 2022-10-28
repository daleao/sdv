namespace DaLion.Shared.Extensions.Stardew;

#region using directives

using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using StardewValley.Buildings;
using StardewValley.Locations;
using StardewValley.TerrainFeatures;

#endregion using directives

/// <summary>Extensions for the <see cref="SObject"/> class.</summary>
public static class SObjectExtensions
{
    /// <summary>Get the <see cref="Farmer"/> instance who owns this <paramref name="object"/>.</summary>
    /// <param name="object">The <see cref="SObject"/>.</param>
    /// <returns>The <see cref="Farmer"/> instance who purchased, found or crafted the <paramref name="object"/>, or the host of the game session if not found.</returns>
    public static Farmer GetOwner(this SObject @object)
    {
        return Game1.getFarmerMaybeOffline(@object.owner.Value) ?? Game1.MasterPlayer;
    }

    /// <summary>
    ///     Gets the tile distance between this <paramref name="object"/> and the target <paramref name="building"/> in
    ///     the same <see cref="GameLocation"/>.
    /// </summary>
    /// <param name="object">The <see cref="SObject"/>.</param>
    /// <param name="building">The target <see cref="Building"/>.</param>
    /// <returns>The tile distance between <paramref name="object"/> and <paramref name="building"/>.</returns>
    public static double DistanceTo(this SObject @object, Building building)
    {
        return (@object.TileLocation - new Vector2(building.tileX.Value, building.tileY.Value)).Length();
    }

    /// <summary>
    ///     Gets the tile distance between this <paramref name="object"/> and the target <paramref name="character"/>
    ///     in the same <see cref="GameLocation"/>.
    /// </summary>
    /// <param name="object">The <see cref="SObject"/>.</param>
    /// <param name="character">The target <see cref="Character"/>.</param>
    /// <returns>The tile distance between <paramref name="object"/> and <paramref name="character"/>.</returns>
    public static double DistanceTo(this SObject @object, Character character)
    {
        return (@object.TileLocation - character.getTileLocation()).Length();
    }

    /// <summary>
    ///     Gets the tile distance between this <paramref name="object"/> and some <paramref name="other"/> in the same
    ///     <see cref="GameLocation"/>.
    /// </summary>
    /// <param name="object">The <see cref="SObject"/>.</param>
    /// <param name="other">The target <see cref="SObject"/>.</param>
    /// <returns>The tile distance between <paramref name="object"/> and <paramref name="other"/>.</returns>
    public static double DistanceTo(this SObject @object, SObject other)
    {
        return (@object.TileLocation - other.TileLocation).Length();
    }

    /// <summary>
    ///     Gets the tile distance between this <paramref name="object"/> and the target
    ///     <paramref name="terrainFeature"/> in the same <see cref="GameLocation"/>.
    /// </summary>
    /// <param name="object">The <see cref="SObject"/>.</param>
    /// <param name="terrainFeature">The target <see cref="TerrainFeature"/>.</param>
    /// <returns>The tile distance between <paramref name="object"/> and <paramref name="terrainFeature"/>.</returns>
    public static double DistanceTo(this SObject @object, TerrainFeature terrainFeature)
    {
        return (@object.TileLocation - terrainFeature.currentTileLocation).Length();
    }

    /// <summary>
    ///     Find the closest <see cref="Building"/> of sub-type <typeparamref name="TBuilding"/> to this
    ///     <paramref name="object"/> in the current <see cref="GameLocation"/>.
    /// </summary>
    /// <typeparam name="TBuilding">A sub-type of <see cref="Building"/>.</typeparam>
    /// <param name="object">The <see cref="SObject"/>.</param>
    /// <param name="location">The object's current location.</param>
    /// <param name="candidates">The candidate <see cref="Building"/>s, if already available.</param>
    /// <param name="predicate">An optional condition with which to filter out candidates.</param>
    /// <returns>The <see cref="Building"/> of type <typeparamref name="TBuilding"/> with the minimal distance to <paramref name="object"/>.</returns>
    public static TBuilding? GetClosestBuilding<TBuilding>(
        this SObject @object, GameLocation location, IEnumerable<TBuilding>? candidates = null, Func<TBuilding, bool>? predicate = null)
        where TBuilding : Building
    {
        if (location is not BuildableGameLocation buildable)
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
            var distanceToThisCandidate = @object.DistanceTo(candidate);
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
    ///     Find the closest <see cref="Character"/> of sub-type <typeparamref name="TCharacter"/> to this <paramref name="object"/>
    ///     in the current <see cref="GameLocation"/>.
    /// </summary>
    /// <typeparam name="TCharacter">A sub-type of <see cref="Character"/>.</typeparam>
    /// <param name="object">The <see cref="SObject"/>.</param>
    /// <param name="location">The object's current location.</param>
    /// <param name="candidates">The candidate <see cref="Character"/>s, if already available.</param>
    /// <param name="predicate">An optional condition with which to filter out candidates.</param>
    /// <returns>The <see cref="Character"/> of type <typeparamref name="TCharacter"/> with the minimal distance to <paramref name="object"/>.</returns>
    public static TCharacter? GetClosestCharacter<TCharacter>(
        this SObject @object, GameLocation location, IEnumerable<TCharacter>? candidates = null, Func<TCharacter, bool>? predicate = null)
        where TCharacter : Character
    {
        predicate ??= _ => true;
        var candidatesArr = candidates?.ToArray() ?? location.characters.OfType<TCharacter>().Where(t => predicate(t)).ToArray();
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
            var distanceToThisCandidate = @object.DistanceTo(candidate);
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
    ///     Find the closest <see cref="Farmer"/> to this <paramref name="object"/> in the current
    ///     <see cref="GameLocation"/>.
    /// </summary>
    /// <param name="object">The <see cref="SObject"/>.</param>
    /// <param name="location">The object's current location.</param>
    /// <param name="candidates">The candidate <see cref="Farmer"/>s, if already available.</param>
    /// <param name="predicate">An optional condition with which to filter out candidates.</param>
    /// <returns>The <see cref="Farmer"/> with the minimal distance to <paramref name="object"/>.</returns>
    /// <remarks>This version is required as <see cref="Farmer"/> references are stored in a different field of <see cref="GameLocation"/>.</remarks>
    public static Farmer? GetClosestFarmer(
        this SObject @object, GameLocation location, IEnumerable<Farmer>? candidates = null, Func<Farmer, bool>? predicate = null)
    {
        predicate ??= _ => true;
        var candidatesArr = candidates?.ToArray() ?? location.farmers.Where(f => predicate(f)).ToArray();
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
            var distanceToThisCandidate = @object.DistanceTo(candidate);
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
    ///     Find the closest <see cref="SObject"/> to this one in the current <see cref="GameLocation"/>, and of the
    ///     specified sub-type.
    /// </summary>
    /// <typeparam name="TObject">A sub-type of <see cref="SObject"/>.</typeparam>
    /// <param name="object">The <see cref="SObject"/>.</param>
    /// <param name="location">The object's current location.</param>
    /// <param name="candidates">The candidate <see cref="SObject"/>s, if already available.</param>
    /// <param name="predicate">An optional condition with which to filter out candidates.</param>
    /// <returns>The <see cref="SObject"/> of type <typeparamref name="TObject"/> with the minimal distance to <paramref name="object"/>.</returns>
    public static TObject? GetClosestObject<TObject>(
        this SObject @object, GameLocation location, IEnumerable<TObject>? candidates = null, Func<TObject, bool>? predicate = null)
        where TObject : SObject
    {
        predicate ??= _ => true;
        var candidatesArr = candidates?.ToArray() ??
                            location.Objects.Values.OfType<TObject>().Where(o => predicate(o)).ToArray();
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
            var distanceToThisCandidate = @object.DistanceTo(candidate);
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
    ///     Find the closest <see cref="TerrainFeature"/> of sub-type <typeparamref name="TTerrainFeature"/> to this
    ///     <paramref name="object"/> in the current <see cref="GameLocation"/>.
    /// </summary>
    /// <typeparam name="TTerrainFeature">A sub-type of <see cref="TerrainFeature"/>.</typeparam>
    /// <param name="object">The <see cref="SObject"/>.</param>
    /// <param name="location">The object's current location.</param>
    /// <param name="candidates">The candidate <see cref="TerrainFeature"/>s, if already available.</param>
    /// <param name="predicate">An optional condition with which to filter out candidates.</param>
    /// <returns>The <see cref="TerrainFeature"/> of type <typeparamref name="TTerrainFeature"/> with the minimal distance to <paramref name="object"/>.</returns>
    public static TTerrainFeature? GetClosestTerrainFeature<TTerrainFeature>(
        this SObject @object, GameLocation location, IEnumerable<TTerrainFeature>? candidates = null, Func<TTerrainFeature, bool>? predicate = null)
        where TTerrainFeature : TerrainFeature
    {
        predicate ??= _ => true;
        var candidatesArr = candidates?.ToArray() ?? location.terrainFeatures.Values.OfType<TTerrainFeature>()
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
            var distanceToThisCandidate = @object.DistanceTo(candidate);
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
