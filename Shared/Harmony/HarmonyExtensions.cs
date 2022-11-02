namespace DaLion.Shared.Harmony;

#region using directives

using System.Collections.Generic;
using System.Linq;
using DaLion.Shared.Extensions.Reflection;
using HarmonyLib;

#endregion using directives

/// <summary>Extensions for the <see cref="Harmony"/> class.</summary>
public static class HarmonyExtensions
{
    /// <summary>Gets all patches applied to methods patched by the <paramref name="harmony"/> instance.</summary>
    /// <param name="harmony">The <see cref="Harmony"/> instance.</param>
    /// <param name="predicate">A filter condition.</param>
    /// <returns>A <see cref="IEnumerable{T}"/> of <see cref="Patch"/> instances applied by <paramref name="harmony"/>.</returns>
    public static IEnumerable<Patch> GetAllPatches(this Harmony harmony, Func<Patch, bool>? predicate = null)
    {
        predicate ??= _ => true;
        return harmony.GetPatchedMethods().SelectMany(m => m.GetAppliedPatches(predicate));
    }

    /// <summary>Gets all patches applied to methods patched by the <paramref name="harmony"/> instance that include a <see cref="HarmonyPrefix"/>.</summary>
    /// <param name="harmony">The <see cref="Harmony"/> instance.</param>
    /// <param name="predicate">A filter condition.</param>
    /// <returns>A <see cref="IEnumerable{T}"/> of <see cref="Patch"/> instances applied by <paramref name="harmony"/> that include at least one <see cref="HarmonyPrefix"/>.</returns>
    public static IEnumerable<Patch> GetAllPrefixes(this Harmony harmony, Func<Patch, bool>? predicate = null)
    {
        predicate ??= _ => true;
        return harmony.GetPatchedMethods().SelectMany(m => m.GetAppliedPrefixes(predicate));
    }

    /// <summary>Gets all patches applied to methods patched by the <paramref name="harmony"/> instance that include a <see cref="HarmonyPostfix"/>.</summary>
    /// <param name="harmony">The <see cref="Harmony"/> instance.</param>
    /// <param name="predicate">A filter condition.</param>
    /// <returns>A <see cref="IEnumerable{T}"/> of <see cref="Patch"/> instances applied by <paramref name="harmony"/> that include at least one <see cref="HarmonyPostfix"/>.</returns>
    public static IEnumerable<Patch> GetAllPostfixes(this Harmony harmony, Func<Patch, bool>? predicate = null)
    {
        predicate ??= _ => true;
        return harmony.GetPatchedMethods().SelectMany(m => m.GetAppliedPostfixes(predicate));
    }

    /// <summary>Gets all patches applied to methods patched by the <paramref name="harmony"/> instance that include a <see cref="HarmonyTranspiler"/>.</summary>
    /// <param name="harmony">The <see cref="Harmony"/> instance.</param>
    /// <param name="predicate">A filter condition.</param>
    /// <returns>A <see cref="IEnumerable{T}"/> of <see cref="Patch"/> instances applied by <paramref name="harmony"/> that include at least one <see cref="HarmonyTranspiler"/>.</returns>
    public static IEnumerable<Patch> GetAllTranspilers(this Harmony harmony, Func<Patch, bool>? predicate = null)
    {
        predicate ??= _ => true;
        return harmony.GetPatchedMethods().SelectMany(m => m.GetAppliedTranspilers(predicate));
    }

    /// <summary>Gets all patches applied to methods patched by the <paramref name="harmony"/> instance that include a <see cref="HarmonyFinalizer"/>.</summary>
    /// <param name="harmony">The <see cref="Harmony"/> instance.</param>
    /// <param name="predicate">A filter condition.</param>
    /// <returns>A <see cref="IEnumerable{T}"/> of <see cref="Patch"/> instances applied by <paramref name="harmony"/> that include at least one <see cref="HarmonyFinalizer"/>.</returns>
    public static IEnumerable<Patch> GetAllFinalizers(this Harmony harmony, Func<Patch, bool>? predicate = null)
    {
        predicate ??= _ => true;
        return harmony.GetPatchedMethods().SelectMany(m => m.GetAppliedFinalizers(predicate));
    }

    /// <summary>
    ///     Gets the patches applied to methods patched by the <paramref name="harmony"/> instance, with the specified
    ///     <paramref name="uniqueId"/>.
    /// </summary>
    /// <param name="harmony">The <see cref="Harmony"/> instance.</param>
    /// <param name="uniqueId">A unique ID to search for.</param>
    /// <returns>A <see cref="IEnumerable{T}"/> of all applied patches by <paramref name="harmony"/> for the mod with the specified <paramref name="uniqueId"/>.</returns>
    public static IEnumerable<Patch> GetPatchesById(this Harmony harmony, string uniqueId)
    {
        return harmony.GetAllPatches(p => p.owner == uniqueId);
    }
}
