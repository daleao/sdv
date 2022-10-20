namespace DaLion.Common.Harmony;

#region using directives

using System.Diagnostics;
using System.Linq;
using System.Reflection;
using DaLion.Common.Attributes;
using DaLion.Common.Extensions.Reflection;
using HarmonyLib;

#endregion using directives

/// <summary>Instantiates and applies <see cref="IHarmonyPatch"/> classes in the assembly.</summary>
internal class Harmonizer
{
    /// <inheritdoc cref="Harmony"/>
    private readonly Harmony _harmony;

    private readonly IModRegistry _modRegistry;

    /// <summary>Initializes a new instance of the <see cref="Harmonizer"/> class.</summary>
    /// <param name="modRegistry">API for fetching metadata about loaded mods.</param>
    /// <param name="uniqueId">The unique ID of the declaring mod.</param>
    internal Harmonizer(IModRegistry modRegistry, string uniqueId)
    {
        this._harmony = new Harmony(uniqueId);
        this._modRegistry = modRegistry;
    }

    /// <summary>Instantiates and applies one of every <see cref="IHarmonyPatch"/> class in the assembly using reflection.</summary>
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
                if (deprecatedAttr is not null)
                {
                    continue;
                }

                var integrationAttr =
                    (RequiresModAttribute?)p.GetCustomAttributes(typeof(RequiresModAttribute), false).FirstOrDefault();
                if (integrationAttr is not null)
                {
                    if (!this._modRegistry.IsLoaded(integrationAttr.UniqueId))
                    {
                        Log.D(
                            $"[Harmonizer]: The target mod {integrationAttr.UniqueId} is not loaded. {p.Name} will be ignored.");
                        continue;
                    }

                    if (!string.IsNullOrEmpty(integrationAttr.Version) &&
                        this._modRegistry.Get(integrationAttr.UniqueId)!.Manifest.Version.IsOlderThan(
                            integrationAttr.Version))
                    {
                        Log.W(
                            $"[Harmonizer]: The integration patch {p.Name} will be ignored because the installed version of {integrationAttr.UniqueId} is older than minimum supported version." +
                            $" Please update {integrationAttr.UniqueId} in order to enable integrations with {this._harmony.Id}.");
                        continue;
                    }
                }

                var patch = (IHarmonyPatch?)p
                    .GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null, Type.EmptyTypes, null)
                    ?.Invoke(Array.Empty<object>());
                if (patch is null)
                {
                    ThrowHelper.ThrowMissingMethodException("Didn't find internal parameter-less constructor.");
                }

                patch.Apply(this._harmony);
                Log.D($"[Harmonizer]: Applied {p.Name} to {patch.Target!.GetFullName()}.");
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
