namespace DaLion.Stardew.Ponds.Extensions;

#region using directives

using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Buildings;
using System;
using System.Collections.Generic;
using System.Linq;

#endregion using directives

/// <summary>Extensions for the <see cref="Farmer"/> class.</summary>
public static class FarmerExtensions
{
    /// <summary>Get the tile distance between the farmer and any building in the <see cref="GameLocation"/>.</summary>
    /// <param name="building">The target building.</param>
    public static double DistanceToBuilding(this Farmer farmer, Building building) =>
        (farmer.getTileLocation() - new Vector2(building.tileX.Value, building.tileY.Value)).Length();

    /// <summary>Find the closest building to this instance in the current <see cref="GameLocation"/>. </summary>
    /// <typeparam name="T">A subtype of <see cref="Building"/>.</typeparam>
    /// <param name="distanceToClosestBuilding">The distance to the returned building, or <see cref="double.MaxValue"/> if none was found.</param>
    /// <param name="candidates">The candidate buildings, if already available.</param>
    /// <param name="predicate">An optional condition with which to filter out candidates (ignore candidates for which the predicate returns <see langword="true">).</param>
    public static T? GetClosestBuilding<T>(this Farmer farmer, out double distanceToClosestBuilding,
        IEnumerable<T>? candidates = null, Func<T, bool>? predicate = null) where T : Building
    {
        predicate ??= _ => true;
        var candidatesArr = candidates?.ToArray() ?? Game1.getFarm().buildings.OfType<T>().Where(c => predicate(c)).ToArray();
        distanceToClosestBuilding = double.MaxValue;
        if (candidatesArr.Length == 0) return null;

        if (candidatesArr.Length == 1)
        {
            distanceToClosestBuilding = farmer.DistanceToBuilding(candidatesArr[0]);
            return candidatesArr[0];
        }

        T? closest = null;
        foreach (var candidate in candidatesArr)
        {
            var distanceToThisCandidate = farmer.DistanceToBuilding(candidate);
            if (distanceToThisCandidate >= distanceToClosestBuilding) continue;

            closest = candidate;
            distanceToClosestBuilding = distanceToThisCandidate;
        }

        return closest;
    }
}