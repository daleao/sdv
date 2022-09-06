namespace DaLion.Common.Extensions.Reflection;

#region using directives

using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

#endregion using directives

/// <summary>Extensions for the <see cref="MethodBase"/> class.</summary>
public static class MethodBaseExtensions
{
    /// <summary>The full name of the method, including the declaring type.</summary>
    public static string GetFullName(this MethodBase method) => $"{method.DeclaringType}::{method.Name}";

    /// <summary>Get all the patches applied to this method and that satisfy a given predicate.</summary>
    /// <param name="predicate">Filter conditions.</param>
    public static IEnumerable<Patch> GetAppliedPatches(this MethodBase method, Func<Patch, bool>? predicate = null)
    {
        predicate ??= _ => true;
        var patches = Harmony.GetPatchInfo(method);
        if (patches is null) yield break;

        foreach (var patch in patches.Prefixes.Where(predicate)) yield return patch;
        foreach (var patch in patches.Postfixes.Where(predicate)) yield return patch;
        foreach (var patch in patches.Transpilers.Where(predicate)) yield return patch;
        foreach (var patch in patches.Finalizers.Where(predicate)) yield return patch;
    }

    /// <summary>Get all the transpilers applied to this method and that satisfy a given predicate.</summary>
    /// <param name="predicate">Filter conditions.</param>
    public static IEnumerable<Patch> GetAppliedTranspilers(this MethodBase method, Func<Patch, bool>? predicate = null)
    {
        predicate ??= _ => true;
        var patches = Harmony.GetPatchInfo(method);
        if (patches is null) yield break;

        foreach (var patch in patches.Transpilers.Where(predicate)) yield return patch;
    }

    /// <summary>Get the patches applied to this method with the specified unique ID.</summary>
    /// <param name="uniqueID">A unique ID to search for.</param>
    public static IEnumerable<Patch> GetAppliedPatchesById(this MethodBase method, string uniqueID)
        => method.GetAppliedPatches(p => p.owner == uniqueID);
}