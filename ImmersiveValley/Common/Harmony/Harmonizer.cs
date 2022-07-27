namespace DaLion.Common.Harmony;

#region using directives

using Common.Attributes;
using HarmonyLib;
using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

#endregion using directives

/// <summary>Instantiates and applies <see cref="HarmonyPatch"/> classes in the assembly.</summary>
internal class Harmonizer
{
    /// <inheritdoc cref="Harmony"/>
    private readonly Harmony _Harmony;
    private readonly IModRegistry _ModRegistry;

    /// <summary>Construct an instance.</summary>
    /// <param name="uniqueID">The unique ID of the declaring mod.</param>
    /// /// <param name="modRegistry">API for fetching metadata about loaded mods.</param>
    internal Harmonizer(IModRegistry modRegistry, string uniqueID)
    {
        _Harmony = new(uniqueID);
        _ModRegistry = modRegistry;
    }

    /// <summary>Instantiate and apply one of every <see cref="IHarmonyPatch" /> class in the assembly using reflection.</summary>
    internal void ApplyAll()
    {
        var sw = new Stopwatch();
        sw.Start();

        Log.D("[Harmonizer]: Gathering patches...");
        var patchTypes = AccessTools
            .GetTypesFromAssembly(Assembly.GetAssembly(typeof(IHarmonyPatch)))
            .Where(t => t.IsAssignableTo(typeof(IHarmonyPatch)) && !t.IsAbstract)
            .ToArray();

        Log.D($"[Harmonizer]: Found {patchTypes.Length} patch classes. Applying patches...");
        foreach (var p in patchTypes)
        {
            try
            {
#if RELEASE
                var debugOnlyAttribute =
                    (DebugOnlyAttribute?)p.GetCustomAttributes(typeof(DebugOnlyAttribute), false).FirstOrDefault();
                if (debugOnlyAttribute is not null) continue;
#endif

                var deprecatedAttr =
                    (DeprecatedAttribute?)p.GetCustomAttributes(typeof(DeprecatedAttribute), false).FirstOrDefault();
                if (deprecatedAttr is not null) continue;

                var integrationAttr = (RequiresModAttribute?)p.GetCustomAttributes(typeof(RequiresModAttribute), false).FirstOrDefault();
                if (integrationAttr is not null && !_ModRegistry.IsLoaded(integrationAttr.UniqueID))
                {
                    Log.D($"[Harmonizer]: The target mod {integrationAttr.UniqueID} is not loaded. {p.Name} will be ignored.");
                    continue;
                }

                var patch = (IHarmonyPatch?)p
                    .GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null, Type.EmptyTypes, null)
                    ?.Invoke(Array.Empty<object>());
                if (patch is null) throw new MissingMethodException("Didn't find internal parameterless constructor.");

                patch.Apply(_Harmony);
                Log.D($"[Harmonizer]: Applied {p.Name} to {patch.Target!.DeclaringType}::{patch.Target.Name}.");
            }
            catch (MissingMethodException ex)
            {
                Log.W($"[Harmonizer]: {ex.Message} {p.Name} will be ignored.");
            }
            catch (Exception ex)
            {
                Log.E($"[Harmonizer]: Failed to apply {p.Name}.\nHarmony returned {ex}");
            }
        }

        sw.Stop();
        Log.D($"[Harmonizer]: Patching completed in {sw.ElapsedMilliseconds}ms.");
    }
}