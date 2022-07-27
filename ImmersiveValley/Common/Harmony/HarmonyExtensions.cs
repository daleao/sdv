namespace DaLion.Common.Harmony;

#region using directives

using Extensions.Reflection;
using HarmonyLib;
using System;
using System.Collections.Generic;

#endregion using directives

public static class HarmonyExtensions
{
    /// <summary>Get all patches applied to methods patched by the harmony instance.</summary>
    /// <param name="predicate">Filter condition.</param>
    public static IEnumerable<Patch> GetPatches(this Harmony harmony, Func<Patch, bool>? predicate = null)
    {
        predicate ??= _ => true;

        var enumerable = new List<Patch>();
        foreach (var method in harmony.GetPatchedMethods()) enumerable.AddRange(method.GetAppliedPatches(predicate));
        return enumerable;
    }

    /// <summary>Get the patches applied to methods patched by the harmony instance, with the specified unique ID.</summary>
    /// <param name="uniqueID">A unique ID to search for.</param>
    public static IEnumerable<Patch> GetPatchesById(this Harmony harmony, string uniqueID)
        => harmony.GetPatches(p => p.owner == uniqueID);
}