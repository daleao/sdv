namespace DaLion.Common.Extensions.Stardew;

#region using directives

using Microsoft.Xna.Framework;
using StardewValley.Buildings;
using StardewValley.Locations;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;
using System.Linq;

#endregion using directives

/// <summary>Extensions for the <see cref="SObject"/> class.</summary>
public static class SObjectExtensions
{
    /// <summary>Get the <see cref="Farmer"/> instance that owns this object.</summary>
    public static Farmer GetOwner(this SObject @object) =>
        Game1.getFarmerMaybeOffline(@object.owner.Value) ?? Game1.MasterPlayer;

    /// <summary>Get the tile distance between the object and a target <see cref="Building"/> in the same <see cref="GameLocation"/>.</summary>
    /// <param name="@object">The target object.</param>
    public static double DistanceTo(this SObject @object, Building building) =>
        (@object.TileLocation - new Vector2(building.tileX.Value, building.tileY.Value)).Length();

    /// <summary>Get the tile distance between the object and a target <see cref="Character"/> in the same <see cref="GameLocation"/>.</summary>
    /// <param name="character">The target character.</param>
    public static double DistanceTo(this SObject @object, Character character) =>
        (@object.TileLocation - character.getTileLocation()).Length();

    /// <summary>Get the tile distance between this and another object in the same <see cref="GameLocation"/>.</summary>
    /// <param name="@object">The target object.</param>
    public static double DistanceTo(this SObject @object, SObject other) =>
        (@object.TileLocation - @object.TileLocation).Length();

    /// <summary>Get the tile distance between the object and a target <see cref="TerrainFeature"/> in the same <see cref="GameLocation"/>.</summary>
    /// <param name="terrain">The target terrain feature.</param>
    public static double DistanceTo(this SObject @object, TerrainFeature terrain) =>
        (@object.TileLocation - terrain.currentTileLocation).Length();

    /// <summary>Find the closest <see cref="Building"/> to this object in the current <see cref="GameLocation"/> of the specified type.</summary>
    /// <param name="location">The object's current location.</param>
    /// <param name="candidates">The candidate buildings, if already available.</param>
    /// <param name="predicate">An optional condition with which to filter out candidates.</param>
    public static T? GetClosestBuilding<T>(this SObject @object, GameLocation location,
        IEnumerable<T>? candidates = null,
        Func<T, bool>? predicate = null) where T : Building
    {
        if (location is not BuildableGameLocation buildable) return null;

        predicate ??= _ => true;
        var candidatesArr = candidates?.ToArray() ?? buildable.buildings.OfType<T>().Where(t => predicate(t)).ToArray();
        var distanceToClosest = double.MaxValue;
        switch (candidatesArr.Length)
        {
            case 0:
                return null;
            case 1:
                return candidatesArr[0];
        }

        T? closest = null;
        foreach (var candidate in candidatesArr)
        {
            var distanceToThisCandidate = @object.DistanceTo(candidate);
            if (distanceToThisCandidate >= distanceToClosest) continue;

            closest = candidate;
            distanceToClosest = distanceToThisCandidate;
        }

        return closest;
    }

    /// <summary>Find the closest <see cref="Farmer"/> to this object in the current <see cref="GameLocation"/>.</summary>
    /// <param name="location">The object's current location.</param>
    /// <param name="candidates">The candidate farmers, if already available.</param>
    /// <param name="predicate">An optional condition with which to filter out candidates.</param>
    public static Farmer? GetClosestFarmer(this SObject @object, GameLocation location,
        IEnumerable<Farmer>? candidates = null,
        Func<Farmer, bool>? predicate = null)
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
            if (distanceToThisCandidate >= distanceToClosest) continue;

            closest = candidate;
            distanceToClosest = distanceToThisCandidate;
        }

        return closest;
    }

    /// <summary>Find the closest <see cref="SObject"/> to this object in the current <see cref="GameLocation"/> of the specified type.</summary>
    /// <param name="location">The object's current location.</param>
    /// <param name="candidates">The candidate objects, if already available.</param>
    /// <param name="predicate">An optional condition with which to filter out candidates.</param>
    public static T? GetClosestObject<T>(this SObject @object, GameLocation location, IEnumerable<T>? candidates = null,
        Func<T, bool>? predicate = null) where T : SObject
    {
        predicate ??= _ => true;
        var candidatesArr = candidates?.ToArray() ??
                            location.Objects.Values.OfType<T>().Where(o => predicate(o)).ToArray();
        var distanceToClosest = double.MaxValue;
        switch (candidatesArr.Length)
        {
            case 0:
                return null;
            case 1:
                return candidatesArr[0];
        }

        T? closest = null;
        foreach (var candidate in candidatesArr)
        {
            var distanceToThisCandidate = @object.DistanceTo(candidate);
            if (distanceToThisCandidate >= distanceToClosest) continue;

            closest = candidate;
            distanceToClosest = distanceToThisCandidate;
        }

        return closest;
    }

    /// <summary>Find the closest <see cref="NPC"/> to this object in the current <see cref="GameLocation"/> of the specified type.</summary>
    /// <param name="location">The object's current location.</param>
    /// <param name="candidates">The candidate NPCs, if already available.</param>
    /// <param name="predicate">An optional condition with which to filter out candidates.</param>
    public static T? GetClosestNPC<T>(this SObject @object, GameLocation location, IEnumerable<T>? candidates = null,
        Func<T, bool>? predicate = null) where T : NPC
    {
        predicate ??= _ => true;
        var candidatesArr = candidates?.ToArray() ?? location.characters.OfType<T>().Where(t => predicate(t)).ToArray();
        var distanceToClosest = double.MaxValue;
        switch (candidatesArr.Length)
        {
            case 0:
                return null;
            case 1:
                return candidatesArr[0];
        }

        T? closest = null;
        foreach (var candidate in candidatesArr)
        {
            var distanceToThisCandidate = @object.DistanceTo(candidate);
            if (distanceToThisCandidate >= distanceToClosest) continue;

            closest = candidate;
            distanceToClosest = distanceToThisCandidate;
        }

        return closest;
    }

    /// <summary>Find the closest terrain feature to this one in the current <see cref="GameLocation"/> of the specified type.</summary>
    /// <param name="location">The object's current location.</param>
    /// <param name="candidates">The candidate terrain features, if already available.</param>
    /// <param name="predicate">An optional condition with which to filter out candidates.</param>
    public static T? GetClosestTerrainFeature<T>(this SObject @object, GameLocation location,
        IEnumerable<T>? candidates = null,
        Func<T, bool>? predicate = null) where T : TerrainFeature
    {
        predicate ??= _ => true;
        var candidatesArr = candidates?.ToArray() ?? location.terrainFeatures.Values.OfType<T>()
            .Where(t => predicate(t)).ToArray();
        var distanceToClosest = double.MaxValue;
        switch (candidatesArr.Length)
        {
            case 0:
                return null;
            case 1:
                return candidatesArr[0];
        }

        T? closest = null;
        foreach (var candidate in candidatesArr)
        {
            var distanceToThisCandidate = @object.DistanceTo(candidate);
            if (distanceToThisCandidate >= distanceToClosest) continue;

            closest = candidate;
            distanceToClosest = distanceToThisCandidate;
        }

        return closest;
    }
}