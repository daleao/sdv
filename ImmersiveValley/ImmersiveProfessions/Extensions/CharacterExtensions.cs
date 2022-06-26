namespace DaLion.Stardew.Professions.Extensions;

#region using directives

using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;

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
    /// <param name="predicate">An optional condition with which to filter out candidates (ignore candidates for which the predicate returns <see langword="true">).</param>
    public static T? GetClosestCharacter<T>(this Character character, out double distanceToClosestCharacter,
        IEnumerable<T>? candidates = null, Func<T, bool>? predicate = null) where T : Character
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

        T? closest = null;
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
    public static Farmer? GetClosestFarmer(this Character character, out double distanceToClosestFarmer,
        IEnumerable<Farmer>? candidates = null, Func<Farmer, bool>? predicate = null)
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

        Farmer? closest = null;
        foreach (var candidate in candidatesArr)
        {
            var distanceToThisCandidate = character.DistanceToCharacter(candidate);
            if (distanceToThisCandidate >= distanceToClosestFarmer) continue;

            closest = candidate;
            distanceToClosestFarmer = distanceToThisCandidate;
        }

        return closest;
    }
}