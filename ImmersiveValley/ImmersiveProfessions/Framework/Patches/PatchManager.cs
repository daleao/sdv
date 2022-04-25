namespace DaLion.Stardew.Professions.Framework;

#region using directives

using System;
using System.Linq;
using System.Reflection;
using HarmonyLib;

using Common.Extensions.Reflection;
using Patches;

#endregion using directives

/// <summary>Unified entry point for applying Harmony patches.</summary>
internal static class PatchManager
{
    internal static uint TotalPrefixCount { get; set; }
    internal static uint TotalPostfixCount { get; set; }
    internal static uint TotalTranspilerCount { get; set; }
    //internal static uint TotalReversePatchCount { get; set; }
    internal static uint AppliedPrefixCount { get; set; }
    internal static uint AppliedPostfixCount { get; set; }
    internal static uint AppliedTranspilerCount { get; set; }
    //internal static uint AppliedReversePatchCount { get; set; }
    internal static uint IgnoredPrefixCount { get; set; }
    internal static uint IgnoredPostfixCount { get; set; }
    internal static uint IgnoredTranspilerCount { get; set; }
    //internal static uint IgnoredReversePatchCount { get; set; }
    internal static uint FailedPrefixCount { get; set; }
    internal static uint FailedPostfixCount { get; set; }
    internal static uint FailedTranspilerCount { get; set; }
    //internal static uint FailedReversePatchCount { get; set; }

    /// <summary>Instantiate and apply one of every <see cref="IPatch" /> class in the assembly using reflection.</summary>
    internal static void ApplyAll(string uniqueID)
    {
        var harmony = new Harmony(uniqueID);

        Log.D("[HarmonyPatcher]: Gathering patches...");
        var patches = AccessTools.GetTypesFromAssembly(Assembly.GetAssembly(typeof(IPatch)))
            .Where(t => t.IsAssignableTo(typeof(IPatch)) && !t.IsAbstract).ToList();

        Log.D($"[HarmonyPatcher]: Found {patches.Count} patch classes. Applying patches...");
        foreach (var patch in patches.Select(t => (IPatch) t.RequireConstructor().Invoke(Array.Empty<object>())))
            patch.Apply(harmony);

        var message = $"[HarmonyPatcher]: Done.\n\t- Applied {AppliedPrefixCount}/{TotalPrefixCount} prefixes.";
        if (AppliedPrefixCount < TotalPrefixCount)
            message += $" {IgnoredPrefixCount} ignored. {FailedPrefixCount} failed.";

        message += $"\n\t- Applied {AppliedPostfixCount}/{TotalPostfixCount} postfixes.";
        if (AppliedPostfixCount < TotalPostfixCount)
            message += $" {IgnoredPostfixCount} ignored. {FailedPostfixCount} failed.";

        message += $"\n\t- Applied {AppliedTranspilerCount}/{TotalTranspilerCount} transpilers.";
        if (AppliedTranspilerCount < TotalTranspilerCount)
            message += $" {IgnoredTranspilerCount} ignored. {FailedTranspilerCount} failed.";

        //message += $"\nApplied {AppliedReversePatchCount}/{TotalReversePatchCount} reverse patches.";
        //if (AppliedReversePatchCount < TotalReversePatchCount)
        //    message += $" {IgnoredReversePatchCount} ignored. {FailedReversePatchCount} failed.";

        Log.D(message);
    }
}