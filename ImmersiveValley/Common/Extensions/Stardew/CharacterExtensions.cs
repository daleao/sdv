namespace DaLion.Common.Extensions.Stardew;

#region using directives

using Microsoft.Xna.Framework;
using ModData;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Locations;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;
using System.Linq;

#endregion using directives

/// <summary>Extensions for the <see cref="Character"/> class.</summary>
public static class CharacterExtensions
{
    /// <summary>Get the tile distance between the character and a target <see cref="Character"/> in the same <see cref="GameLocation"/>.</summary>
    /// <param name="character">The target character.</param>
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

    /// <summary>Find the closest <see cref="Character"/> to this character in the current <see cref="GameLocation"/>.</summary>
    /// <typeparam name="T">A subtype of <see cref="Character"/>.</typeparam>
    /// <param name="candidates">The candidate characters, if already available.</param>
    /// <param name="predicate">An optional condition with which to filter out candidates.</param>
    public static T? GetClosestCharacter<T>(this Character character, IEnumerable<T>? candidates = null,
        Func<T, bool>? predicate = null) where T : Character
    {
        if (character.currentLocation is not BuildableGameLocation buildable) return null;

        predicate ??= _ => true;
        var candidatesArr = candidates?.ToArray() ?? buildable.characters.OfType<T>().Where(b => predicate(b)).ToArray();
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

    /// <inheritdoc cref="ModDataIO.Read"/>
    public static string Read(this Character character, string field, string defaultValue = "", string modId = "") =>
        ModDataIO.Read(character, field, defaultValue, modId);

    /// <inheritdoc cref="ModDataIO.Read{T}"/>
    public static T Read<T>(this Character character, string field, T defaultValue = default, string modId = "") where T : struct =>
        ModDataIO.Read(character, field, defaultValue, modId);

    /// <inheritdoc cref="ModDataIO.Write"/>
    public static void Write(this Character character, string field, string? value) =>
        ModDataIO.Write(character, field, value);

    /// <inheritdoc cref="ModDataIO.WriteIfNotExists"/>
    public static void WriteIfNotExists(this Character character, string field, string? value) =>
        ModDataIO.WriteIfNotExists(character, field, value);

    /// <inheritdoc cref="ModDataIO.Append"/>
    public static void Append(this Character character, string field, string value, string separator = ",") =>
        ModDataIO.Append(character, field, value, separator);

    /// <inheritdoc cref="ModDataIO.Increment{T}"/>
    public static void Increment<T>(this Character character, string field, T amount) where T : struct =>
        ModDataIO.Increment(character, field, amount);

    /// <summary>Increment the value of a numeric field in the character's <see cref="ModDataDictionary" /> by 1.</summary>
    /// <param name="field">The field to update.</param>
    public static void Increment(this Character character, string field) =>
        ModDataIO.Increment(character, field, 1);
}