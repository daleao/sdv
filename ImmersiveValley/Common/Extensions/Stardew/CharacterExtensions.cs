namespace DaLion.Common.Extensions.Stardew;

#region using directives

using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Locations;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using SObject = StardewValley.Object;

#endregion using directives

/// <summary>Extensions for the <see cref="Character"/> class.</summary>
public static class CharacterExtensions
{
    /// <summary>Get the tile distance between the character and a target <see cref="Building"/> in the same <see cref="GameLocation"/>.</summary>
    /// <param name="building">The target building.</param>
    public static double DistanceTo(this Character character, Building building) =>
        (character.getTileLocation() - new Vector2(building.tileX.Value, building.tileY.Value)).Length();

    /// <summary>Get the tile distance between this and another character in the same <see cref="GameLocation"/>.</summary>
    /// <param name="other">The target character.</param>
    public static double DistanceTo(this Character character, Character other) =>
        (character.getTileLocation() - other.getTileLocation()).Length();

    /// <summary>Get the tile distance between the character and a target <see cref="SObject"/> in the same <see cref="GameLocation"/>.</summary>
    /// <param name="@object">The target object.</param>
    public static double DistanceTo(this Character character, SObject @object) =>
        (character.getTileLocation() - @object.TileLocation).Length();

    /// <summary>Get the tile distance between the character and a target <see cref="TerrainFeature"/> in the same <see cref="GameLocation"/>.</summary>
    /// <param name="terrain">The target terrain feature.</param>
    public static double DistanceTo(this Character character, TerrainFeature terrain) =>
        (character.getTileLocation() - terrain.currentTileLocation).Length();

    /// <summary>Find the closest <see cref="Building"/> to this character in the current <see cref="GameLocation"/>.</summary>
    /// <typeparam name="T">A subtype of <see cref="Building"/>.</typeparam>
    /// <param name="candidates">The candidate buildings, if already available.</param>
    /// <param name="predicate">An optional condition with which to filter out candidates.</param>
    public static T? GetClosestBuilding<T>(this Character character, IEnumerable<T>? candidates = null,
        Func<T, bool>? predicate = null) where T : Building
    {
        if (character.currentLocation is not BuildableGameLocation buildable) return null;

        predicate ??= _ => true;
        var candidatesArr = candidates?.ToArray() ?? buildable.buildings.OfType<T>().Where(b => predicate(b)).ToArray();
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
            var distanceToThisCandidate = character.DistanceTo(candidate);
            if (distanceToThisCandidate >= distanceToClosest) continue;

            closest = candidate;
            distanceToClosest = distanceToThisCandidate;
        }

        return closest;
    }

    /// <summary>Find the closest <see cref="Farmer"/> to this character in the current <see cref="GameLocation"/>.</summary>
    /// <param name="candidates">The candidate farmers, if already available.</param>
    /// <param name="predicate">An optional condition with which to filter out candidates.</param>
    public static Farmer? GetClosestFarmer(this Character character, IEnumerable<Farmer>? candidates = null,
        Func<Farmer, bool>? predicate = null)
    {
        predicate ??= _ => true;
        var candidatesArr = candidates?.ToArray() ??
                            character.currentLocation.farmers.Where(f => f != character && predicate(f)).ToArray();
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
            var distanceToThisCandidate = character.DistanceTo(candidate);
            if (distanceToThisCandidate >= distanceToClosest) continue;

            closest = candidate;
            distanceToClosest = distanceToThisCandidate;
        }

        return closest;
    }

    /// <summary>Find the closest <see cref="SObject"/> to this character in the current <see cref="GameLocation"/>.</summary>
    /// <typeparam name="T">A subtype of <see cref="SObject"/>.</typeparam>
    /// <param name="candidates">The candidate objects, if already available.</param>
    /// <param name="predicate">An optional condition with which to filter out candidates.</param>
    public static T? GetClosestObject<T>(this Character character, IEnumerable<T>? candidates = null,
        Func<T, bool>? predicate = null) where T : SObject
    {
        predicate ??= _ => true;
        var candidatesArr = candidates?.ToArray() ??
                            character.currentLocation.Objects.Values.OfType<T>().Where(o => predicate(o)).ToArray();
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
            var distanceToThisCandidate = character.DistanceTo(candidate);
            if (distanceToThisCandidate >= distanceToClosest) continue;

            closest = candidate;
            distanceToClosest = distanceToThisCandidate;
        }

        return closest;
    }

    /// <summary>Find the closest <see cref="NPC"/> to this character in the current <see cref="GameLocation"/>.</summary>
    /// <typeparam name="T">A subtype of <see cref="NPC"/>.</typeparam>
    /// <param name="candidates">The candidate NPCs, if already available.</param>
    /// <param name="predicate">An optional condition with which to filter out candidates.</param>
    public static T? GetClosestNPC<T>(this Character character, IEnumerable<T>? candidates = null,
        Func<T, bool>? predicate = null) where T : NPC
    {
        predicate ??= _ => true;
        var candidatesArr = candidates?.ToArray() ?? character.currentLocation.characters.OfType<T>()
            .Where(c => c != character && predicate(c)).ToArray();
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
            var distanceToThisCandidate = character.DistanceTo(candidate);
            if (distanceToThisCandidate >= distanceToClosest) continue;

            closest = candidate;
            distanceToClosest = distanceToThisCandidate;
        }

        return closest;
    }

    /// <summary>Find the closest terrain feature to this character in the current <see cref="GameLocation"/>.</summary>
    /// <typeparam name="T">A subtype of <see cref="TerrainFeature"/>.</typeparam>
    /// <param name="candidates">The candidate terrain features, if already available.</param>
    /// <param name="predicate">An optional condition with which to filter out candidates.</param>
    public static T? GetClosestTerrainFeature<T>(this Character character, IEnumerable<T>? candidates = null,
        Func<T, bool>? predicate = null) where T : TerrainFeature
    {
        predicate ??= _ => true;
        var candidatesArr = candidates?.ToArray() ?? character.currentLocation.terrainFeatures.Values.OfType<T>()
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
            var distanceToThisCandidate = character.DistanceTo(candidate);
            if (distanceToThisCandidate >= distanceToClosest) continue;

            closest = candidate;
            distanceToClosest = distanceToThisCandidate;
        }

        return closest;
    }
}