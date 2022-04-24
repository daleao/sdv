namespace DaLion.Stardew.Professions.Framework.Extensions;

#region using directives

using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using StardewValley;

using Common.Extensions;
using Common.Stardew.Extensions;

#endregion using directives

/// <summary>Extensions for the <see cref="Character"/> class.</summary>
public static class CharacterExtensions
{
    /// <summary>Get the tile distance between the instance and any other character in the <see cref="GameLocation"/>.</summary>
    /// <param name="character">The target character.</param>
    public static double DistanceToCharacter(this Character character, Character other)
    {
        return (character.getTileLocation() - other.getTileLocation()).Length();
    }

    /// <summary>Find the closest character to this instance in the current <see cref="GameLocation"/>. </summary>
    /// <typeparam name="T">A subtype of <see cref="Character"/>.</typeparam>
    /// <param name="distanceToClosestCharacter">The distance to the returned character, or <see cref="double.MaxValue"/> if none was found.</param>
    /// <param name="candidates">The candidate characters, if already available.</param>
    /// <param name="predicate">An optional condition with which to filter out candidates (ignore candidates for which the predicate returns <c>True</c>.</param>
    [CanBeNull]
    public static T GetClosestCharacter<T>(this Character character, out double distanceToClosestCharacter,
        IEnumerable<T> candidates = null, Func<T, bool> predicate = null) where T : Character
    {
        predicate ??= _ => true;
        var candidatesArr = candidates?.ToArray() ?? character.currentLocation?.characters.OfType<T>().Where(c => predicate(c)).ToArray();
        distanceToClosestCharacter = double.MaxValue;
        if (candidatesArr is null || candidatesArr.Length == 0) return null;

        if (candidatesArr.Length == 1)
        {
            distanceToClosestCharacter = character.DistanceToCharacter(candidatesArr[0]);
            return candidatesArr[0];
        }

        T closest = null;
        foreach (var candidate in candidatesArr)
        {
            var distanceToThisCandidate = character.DistanceToCharacter(candidate);
            if (distanceToThisCandidate >= distanceToClosestCharacter) continue;

            closest = candidate;
            distanceToClosestCharacter = distanceToThisCandidate;
        }

        return closest;
    }

    /// <summary>Find the closest farmer to this instance in the current <see cref="GameLocation"/>. </summary>
    /// <param name="distanceToClosestFarmer">The distance to the returned farmer, or <see cref="double.MaxValue"/> if none was found.</param>
    /// <param name="candidates">The candidate farmers, if already available.</param>
    /// <param name="predicate">An optional condition with which to filter out candidates.</param>
    [CanBeNull]
    public static Farmer GetClosestFarmer(this Character character, out double distanceToClosestFarmer,
        IEnumerable<Farmer> candidates = null, Func<Farmer, bool> predicate = null)
    {
        predicate ??= _ => true;
        var candidatesArr = candidates?.ToArray() ?? character.currentLocation?.farmers.Where(f => predicate(f)).ToArray();
        distanceToClosestFarmer = double.MaxValue;
        if (candidatesArr is null || candidatesArr.Length == 0) return null;

        if (candidatesArr.Length == 1)
        {
            distanceToClosestFarmer = character.DistanceToCharacter(candidatesArr[0]);
            return candidatesArr[0];
        }

        Farmer closest = null;
        foreach (var candidate in candidatesArr)
        {
            var distanceToThisCandidate = character.DistanceToCharacter(candidate);
            if (distanceToThisCandidate >= distanceToClosestFarmer) continue;

            closest = candidate;
            distanceToClosestFarmer = distanceToThisCandidate;
        }

        return closest;
    }

    /// <summary>Read a string from this character's <see cref="ModDataDictionary" />.</summary>
    /// <param name="field">The field to read from.</param>
    /// <param name="defaultValue">The default value to return if the field does not exist.</param>
    public static string ReadData(this Character character, string field, string defaultValue = "")
    {
        return character.modData.Read($"{ModEntry.Manifest.UniqueID}/{field}", defaultValue);
    }

    /// <summary>Read a field from this character's <see cref="ModDataDictionary" /> as <typeparamref name="T" />.</summary>
    /// <param name="field">The field to read from.</param>
    /// <param name="defaultValue"> The default value to return if the field does not exist.</param>
    public static T ReadDataAs<T>(this Character character, string field, T defaultValue = default)
    {
        return character.modData.ReadAs($"{ModEntry.Manifest.UniqueID}/{field}", defaultValue);
    }

    /// <summary>Write to a field in this character's <see cref="ModDataDictionary" />, or remove the field if supplied with a null or empty value.</summary>
    /// <param name="field">The field to write to.</param>
    /// <param name="value">The value to write, or <c>null</c> to remove the field.</param>
    public static void WriteData(this Character character, string field, string value)
    {
        character.modData.Write($"{ModEntry.Manifest.UniqueID}/{field}", value);
        Log.D($"[ModData]: Wrote {value} to {character.Name}'s {field}.");
    }

    /// <summary>Write to a field in this character's <see cref="ModDataDictionary" />, only if it doesn't yet have a value.</summary>
    /// <param name="field">The field to write to.</param>
    /// <param name="value">The value to write, or <c>null</c> to remove the field.</param>
    public static bool WriteDataIfNotExists(this Character character, string field, string value)
    {
        if (character.modData.ContainsKey($"{ModEntry.Manifest.UniqueID}/{field}"))
        {
            Log.D($"[ModData]: The data field {field} already existed.");
            return true;
        }
        
        character.WriteData(field, value);
        return false;
    }

    /// <summary>Append a string to an existing string field in this character's <see cref="ModDataDictionary"/>, or initialize it with the given value.</summary>
    /// <param name="field">The field to update.</param
    /// <param name="value">Value to append.</param>
    public static void AppendData(this Character character, string field, string value, string separator = ",")
    {
        var current = character.ReadData(field);
        if (current.Contains(value))
        {
            Log.D($"[ModData]: {character.Name}'s {field} already contained {value}.");
        }
        character.WriteData(field, string.IsNullOrEmpty(current) ? value : current + separator + value);
        Log.D($"[ModData]: Appended {character.Name}'s {field} with {value}");
    }

    /// <summary>Increment the value of a numeric field in this character's <see cref="ModDataDictionary" /> by an arbitrary amount.</summary>
    /// <param name="field">The field to update.</param>
    /// <param name="amount">Amount to increment by.</param>
    public static void IncrementData<T>(this Character character, string field, T amount)
    {
        character.modData.Increment($"{ModEntry.Manifest.UniqueID}/{field}", amount);
        Log.D($"[ModData]: Incremented {character.Name}'s {field} by {amount}.");
    }

    /// <summary>Increment the value of a numeric field in this character's <see cref="ModDataDictionary" /> by 1.</summary>
    /// <param name="field">The field to update.</param>
    public static void IncrementData<T>(this Character character, string field)
    {
        character.modData.Increment($"{ModEntry.Manifest.UniqueID}/{field}",
            "1".Parse<T>());
        Log.D($"[ModData]: Incremented {character.Name}'s {field} by 1.");
    }
}