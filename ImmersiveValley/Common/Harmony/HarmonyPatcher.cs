namespace DaLion.Common.Harmony;

#region using directives

using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using HarmonyLib;

using Extensions.Reflection;

#endregion using directives

/// <summary>Applies Harmony patches and logs statistics.</summary>
internal class HarmonyPatcher
{
    /// <inheritdoc cref="Harmony"/>
    private readonly Harmony _Harmony;

    /// <summary>Construct an instance.</summary>
    /// <param name="uniqueID">The unique mod ID.</param>
    public HarmonyPatcher(string uniqueID)
    {
        _Harmony = new(uniqueID);
    }

    /// <summary>Instantiate and apply one of every <see cref="IPatch" /> class in the assembly using reflection.</summary>
    internal void ApplyAll()
    {
        var sw = new Stopwatch();
        sw.Start();

        Log.D("[HarmonyPatcher]: Gathering patches...");
        var patchTypes = AccessTools.GetTypesFromAssembly(Assembly.GetAssembly(typeof(IPatch)))
            .Where(t => t.IsAssignableTo(typeof(IPatch)) && !t.IsAbstract).ToList();

        Log.D($"[HarmonyPatcher]: Found {patchTypes.Count} patch classes. Applying patches...");
        foreach (var p in patchTypes)
        {
            try
            {
                var patch = (IPatch) p.GetConstructor(Type.EmptyTypes)!.Invoke(Array.Empty<object>());
                patch.Apply(_Harmony);
                Log.D($"[HarmonyPatcher]: Applied {p.Name} to {patch.Target.DeclaringType}::{patch.Target.Name}.");
            }
            catch (MissingMethodException ex)
            {
                Log.D($"[HarmonyPatcher]: {ex.Message}. The {p.Name} will be ignored.");
            }
            catch (Exception ex)
            {
                Log.E($"[HarmonyPatcher]: Failed to apply {p.Name}.\nHarmony returned {ex}");
            }
        }

        sw.Stop();
        Log.D($"[HarmonyPatcher]: Patching completed in {sw.ElapsedMilliseconds}ms.");
    }
}