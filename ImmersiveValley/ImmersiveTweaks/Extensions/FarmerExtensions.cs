// ReSharper disable PossibleLossOfFraction
namespace DaLion.Stardew.Tweex.Extensions;

#region using directives

using StardewValley;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using SObject = StardewValley.Object;

#endregion using directives

/// <summary>Extensions for the <see cref="Farmer"/> class.</summary>
public static class FarmerExtensions
{
    /// <summary>Get the tile distance between the farmer and a <see cref="Tree"/> in the same <see cref="GameLocation"/>.</summary>
    /// <param name="character">The target character.</param>
    public static double DistanceToTree(this Farmer farmer, Tree tree)
    {
        return (farmer.getTileLocation() - tree.currentTileLocation).Length();
    }

    /// <summary>Find the closest <see cref="Tree"/> to the farmer in the current <see cref="GameLocation"/>. </summary>
    /// <param name="distanceToClosestTree">The distance to the returned tree, or <see cref="double.MaxValue"/> if none was found.</param>
    /// <param name="candidates">The candidate trees, if already available.</param>
    /// <param name="predicate">An optional condition with which to filter out candidates (ignore candidates for which the predicate returns <see langword="true">).</param>
    public static Tree? GetClosestTree(this Farmer farmer, out double distanceToClosestTree,
        IEnumerable<Tree>? candidates = null, Func<Tree, bool>? predicate = null)
    {
        predicate ??= _ => true;
        var candidatesArr = candidates?.ToArray() ?? farmer.currentLocation?.terrainFeatures.Values.OfType<Tree>()
            .Where(c => predicate(c)).ToArray();
        distanceToClosestTree = double.MaxValue;
        if (candidatesArr is null || candidatesArr.Length == 0) return null;

        if (candidatesArr.Length == 1)
        {
            distanceToClosestTree = farmer.DistanceToTree(candidatesArr[0]);
            return candidatesArr[0];
        }

        Tree? closest = null;
        foreach (var candidate in candidatesArr)
        {
            var distanceToThisCandidate = farmer.DistanceToTree(candidate);
            if (distanceToThisCandidate >= distanceToClosestTree) continue;

            closest = candidate;
            distanceToClosestTree = distanceToThisCandidate;
        }

        return closest;
    }

    /// <summary>Get the tile distance between the farmer and a big craftable <see cref="SObject"/> in the same <see cref="GameLocation"/>.</summary>
    /// <param name="@object">The target big craftable.</param>
    public static double DistanceToBigCraftable(this Farmer farmer, SObject @object)
    {
        if (!@object.bigCraftable.Value) throw new InvalidOperationException($"{@object.Name} is not a Big Craftable.");
        return (farmer.getTileLocation() - @object.TileLocation).Length();
    }

    /// <summary>Find the closest <see cref="Tree"/> to the farmer in the current <see cref="GameLocation"/>. </summary>
    /// <param name="distanceToClosestCraftable">The distance to the returned craftable, or <see cref="double.MaxValue"/> if none was found.</param>
    /// <param name="candidates">The candidate objects, if already available.</param>
    /// <param name="predicate">An optional condition with which to filter out candidates (ignore candidates for which the predicate returns <see langword="true">).</param>
    public static SObject? GetClosestBigCraftable(this Farmer farmer, out double distanceToClosestCraftable,
        IEnumerable<SObject>? candidates = null, Func<SObject, bool>? predicate = null)
    {
        predicate ??= _ => true;
        var candidatesArr = candidates?.Where(c => c.bigCraftable.Value).ToArray() ?? farmer.currentLocation?.objects
            .Values.OfType<SObject>().Where(c => c.bigCraftable.Value && predicate(c)).ToArray();
        distanceToClosestCraftable = double.MaxValue;
        if (candidatesArr is null || candidatesArr.Length == 0) return null;

        if (candidatesArr.Length == 1)
        {
            distanceToClosestCraftable = farmer.DistanceToBigCraftable(candidatesArr[0]);
            return candidatesArr[0];
        }

        SObject? closest = null;
        foreach (var candidate in candidatesArr)
        {
            var distanceToThisCandidate = farmer.DistanceToBigCraftable(candidate);
            if (distanceToThisCandidate >= distanceToClosestCraftable) continue;

            closest = candidate;
            distanceToClosestCraftable = distanceToThisCandidate;
        }

        return closest;
    }
}