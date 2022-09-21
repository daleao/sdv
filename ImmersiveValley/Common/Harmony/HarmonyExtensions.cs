namespace DaLion.Common.Harmony;

#region using directives

using System;
using System.Collections.Generic;
using System.Linq;
using DaLion.Common.Extensions.Reflection;
using HarmonyLib;

#endregion using directives

/// <summary>Extensions for the <see cref="Harmony"/> class.</summary>
public static class HarmonyExtensions
{
    /// <summary>Gets all patches applied to methods patched by the <paramref name="harmony"/> instance.</summary>
    /// <param name="harmony">The <see cref="Harmony"/> instance.</param>
    /// <param name="predicate">A filter condition.</param>
    /// <returns>A <see cref="IEnumerable{T}"/> of all applied patches by <paramref name="harmony"/>.</returns>
    public static IEnumerable<Patch> GetAllPatches(this Harmony harmony, Func<Patch, bool>? predicate = null)
    {
        predicate ??= _ => true;
        return harmony.GetPatchedMethods().SelectMany(m => m.GetAppliedPatches(predicate));
    }

    /// <summary>
    ///     Gets the patches applied to methods patched by the <paramref name="harmony"/> instance, with the specified
    ///     <paramref name="uniqueId"/>.
    /// </summary>
    /// <param name="harmony">The <see cref="Harmony"/> instance.</param>
    /// <param name="uniqueId">A unique ID to search for.</param>
    /// <returns>A <see cref="IEnumerable{T}"/> of all applied patches by <paramref name="harmony"/> for the mod with the specified <paramref name="uniqueId"/>.</returns>
    public static IEnumerable<Patch> GetAllPatchesById(this Harmony harmony, string uniqueId)
    {
        return harmony.GetAllPatches(p => p.owner == uniqueId);
    }
}
