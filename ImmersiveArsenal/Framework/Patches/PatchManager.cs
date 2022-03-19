namespace DaLion.Stardew.Arsenal.Framework;

#region using directives

using System;
using System.Linq;
using System.Reflection;
using HarmonyLib;

using Common.Extensions;
using Patches;

#endregion using directives

/// <summary>Unified entry point for applying Harmony patches.</summary>
internal static class PatchManager
{
    /// <summary>Instantiate and apply one of every <see cref="IPatch" /> class in the assembly using reflection.</summary>
    internal static void ApplyAll(string uniqueID)
    {
        var harmony = new Harmony(uniqueID);

        Log.D("[HarmonyPatcher]: Gathering patches...");
        var patches = AccessTools.GetTypesFromAssembly(Assembly.GetAssembly(typeof(IPatch)))
            .Where(t => t.IsAssignableTo(typeof(IPatch)) && !t.IsAbstract).ToList();

        Log.D($"[HarmonyPatcher]: Found {patches.Count} patch classes. Applying patches...");
        foreach (var patch in patches.Select(t => (IPatch) t.Constructor().Invoke(Array.Empty<object>())))
            patch.Apply(harmony);
    }
}