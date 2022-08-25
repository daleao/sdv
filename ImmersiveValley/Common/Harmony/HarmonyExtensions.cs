namespace DaLion.Common.Harmony;

#region using directives

using Extensions.Reflection;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;

#endregion using directives

public static class HarmonyExtensions
{
    /// <summary>Get all patches applied to methods patched by the harmony instance.</summary>
    /// <param name="predicate">Filter condition.</param>
    public static IEnumerable<Patch> GetAllPatches(this Harmony harmony, Func<Patch, bool>? predicate = null)
    {
        predicate ??= _ => true;
        return harmony.GetPatchedMethods().SelectMany(m => m.GetAppliedPatches(predicate));
    }

    /// <summary>Get the patches applied to methods patched by the harmony instance, with the specified unique ID.</summary>
    /// <param name="uniqueID">A unique ID to search for.</param>
    public static IEnumerable<Patch> GetAllPatchesById(this Harmony harmony, string uniqueID)
        => harmony.GetAllPatches(p => p.owner == uniqueID);
}