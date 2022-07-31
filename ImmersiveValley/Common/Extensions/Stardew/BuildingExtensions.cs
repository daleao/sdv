namespace DaLion.Common.Extensions.Stardew;

#region using directives

using Microsoft.Xna.Framework;
using ModData;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;
using System.Linq;

#endregion using directives

/// <summary>Extensions for the <see cref="Building"/> class.</summary>
public static class BuildingExtensions
{
    /// <summary>Determine whether the building can accommodate no further occupants.</summary>
    public static bool IsFull(this Building building) =>
        building.currentOccupants.Value >= building.maxOccupants.Value;

    /// <summary>Get the <see cref="Building"/> instance that owns this building.</summary>
    public static Farmer GetOwner(this Building building) =>
        Game1.getFarmerMaybeOffline(building.owner.Value) ?? Game1.MasterPlayer;

    /// <summary>Get the tile distance between this and another building in the same <see cref="GameLocation"/>.</summary>
    /// <param name="@object">The target object.</param>
    public static double DistanceTo(this Building building, Building other) =>
        (new Vector2(building.tileX.Value, building.tileY.Value) - new Vector2(other.tileX.Value, other.tileY.Value))
        .Length();

    /// <summary>Get the tile distance between the building and a target <see cref="Character"/> in the same <see cref="GameLocation"/>.</summary>
    /// <param name="character">The target character.</param>
    public static double DistanceTo(this Building building, Character character) =>
        (new Vector2(building.tileX.Value, building.tileY.Value) - character.getTileLocation()).Length();

    /// <summary>Get the tile distance between the building and a target <see cref="SObject"/> in the same <see cref="GameLocation"/>.</summary>
    /// <param name="@object">The target object.</param>
    public static double DistanceTo(this Building building, SObject @object) =>
        (new Vector2(building.tileX.Value, building.tileY.Value) - @object.TileLocation).Length();

    /// <summary>Get the tile distance between the building and a target <see cref="TerrainFeature"/> in the same <see cref="GameLocation"/>.</summary>
    /// <param name="terrain">The target terrain feature.</param>
    public static double DistanceTo(this Building building, TerrainFeature terrain) =>
        (new Vector2(building.tileX.Value, building.tileY.Value) - terrain.currentTileLocation).Length();

    /// <summary>Find the closest building to this one in the current <see cref="GameLocation"/> of the specified type.</summary>
    /// <param name="candidates">The candidate buildings, if already available.</param>
    /// <param name="predicate">An optional condition with which to filter out candidates.</param>
    public static T? GetClosestBuilding<T>(this Building building, IEnumerable<T>? candidates = null,
        Func<T, bool>? predicate = null) where T : Building
    {
        predicate ??= _ => true;
        var candidatesArr = candidates?.ToArray() ??
                            Game1.getFarm().buildings.OfType<T>().Where(t => predicate(t)).ToArray();
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
            var distanceToThisCandidate = building.DistanceTo(candidate);
            if (distanceToThisCandidate >= distanceToClosest) continue;

            closest = candidate;
            distanceToClosest = distanceToThisCandidate;
        }

        return closest;
    }

    /// <summary>Find the closest <see cref="Building"/> to this building in the current <see cref="GameLocation"/>.</summary>
    /// <param name="candidates">The candidate buildings, if already available.</param>
    /// <param name="predicate">An optional condition with which to filter out candidates.</param>
    public static Building? GetClosestBuilding(this Building building, IEnumerable<Building>? candidates = null,
        Func<Building, bool>? predicate = null)
    {
        predicate ??= _ => true;
        var candidatesArr = candidates?.ToArray() ?? Game1.getFarm().buildings.Where(f => predicate(f)).ToArray();
        var distanceToClosest = double.MaxValue;
        switch (candidatesArr.Length)
        {
            case 0:
                return null;
            case 1:
                return candidatesArr[0];
        }

        Building? closest = null;
        foreach (var candidate in candidatesArr)
        {
            var distanceToThisCandidate = building.DistanceTo(candidate);
            if (distanceToThisCandidate >= distanceToClosest) continue;

            closest = candidate;
            distanceToClosest = distanceToThisCandidate;
        }

        return closest;
    }

    /// <summary>Find the closest <see cref="SObject"/> to this building in the current <see cref="GameLocation"/> of the specified type.</summary>
    /// <param name="candidates">The candidate objects, if already available.</param>
    /// <param name="predicate">An optional condition with which to filter out candidates.</param>
    public static T? GetClosestObject<T>(this Building building, IEnumerable<T>? candidates = null,
        Func<T, bool>? predicate = null) where T : SObject
    {
        predicate ??= _ => true;
        var candidatesArr = candidates?.ToArray() ??
                            Game1.getFarm().Objects.Values.OfType<T>().Where(o => predicate(o)).ToArray();
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
            var distanceToThisCandidate = building.DistanceTo(candidate);
            if (distanceToThisCandidate >= distanceToClosest) continue;

            closest = candidate;
            distanceToClosest = distanceToThisCandidate;
        }

        return closest;
    }

    /// <summary>Find the closest <see cref="NPC"/> to this building in the current <see cref="GameLocation"/>.</summary>
    /// <param name="candidates">The candidate NPCs, if already available.</param>
    /// <param name="predicate">An optional condition with which to filter out candidates.</param>
    public static T? GetClosestNPC<T>(this Building building, IEnumerable<T>? candidates = null,
        Func<T, bool>? predicate = null) where T : NPC
    {
        predicate ??= _ => true;
        var candidatesArr = candidates?.ToArray() ??
                            Game1.getFarm().characters.OfType<T>().Where(t => predicate(t)).ToArray();
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
            var distanceToThisCandidate = building.DistanceTo(candidate);
            if (distanceToThisCandidate >= distanceToClosest) continue;

            closest = candidate;
            distanceToClosest = distanceToThisCandidate;
        }

        return closest;
    }

    /// <summary>Find the closest <see cref="TerrainFeature"/> to this building in the current <see cref="GameLocation"/> of the specified type.</summary>
    /// <param name="candidates">The candidate terrain features, if already available.</param>
    /// <param name="predicate">An optional condition with which to filter out candidates.</param>
    public static T? GetClosestTerrainFeature<T>(this Building building, IEnumerable<T>? candidates = null,
        Func<T, bool>? predicate = null) where T : TerrainFeature
    {
        predicate ??= _ => true;
        var candidatesArr = candidates?.ToArray() ??
                            Game1.getFarm().terrainFeatures.Values.OfType<T>().Where(t => predicate(t)).ToArray();
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
            var distanceToThisCandidate = building.DistanceTo(candidate);
            if (distanceToThisCandidate >= distanceToClosest) continue;

            closest = candidate;
            distanceToClosest = distanceToThisCandidate;
        }

        return closest;
    }

    /// <inheritdoc cref="ModDataIO.Read"/>
    public static string Read(this Building building, string field, string defaultValue = "", string modId = "") =>
        ModDataIO.Read(building, field, defaultValue, modId);

    /// <inheritdoc cref="ModDataIO.Read{T}"/>
    public static T Read<T>(this Building building, string field, T defaultValue = default, string modId = "") where T : struct =>
        ModDataIO.Read(building, field, defaultValue, modId);

    /// <inheritdoc cref="ModDataIO.Write"/>
    public static void Write(this Building building, string field, string? value) =>
        ModDataIO.Write(building, field, value);

    /// <inheritdoc cref="ModDataIO.WriteIfNotExists"/>
    public static void WriteIfNotExists(this Building building, string field, string? value) =>
        ModDataIO.WriteIfNotExists(building, field, value);

    /// <inheritdoc cref="ModDataIO.Append"/>
    public static void Append(this Building building, string field, string value, string separator = ",") =>
        ModDataIO.Append(building, field, value, separator);

    /// <inheritdoc cref="ModDataIO.Increment{T}"/>
    public static void Increment<T>(this Building building, string field, T amount) where T : struct =>
        ModDataIO.Increment(building, field, amount);

    /// <summary>Increment the value of a numeric field in the building's <see cref="ModDataDictionary" /> by 1.</summary>
    /// <param name="field">The field to update.</param>
    public static void Increment(this Building building, string field) =>
        ModDataIO.Increment(building, field, 1);
}