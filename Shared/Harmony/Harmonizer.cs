namespace DaLion.Shared.Harmony;

#region using directives

using System.Diagnostics;
using System.Linq;
using System.Reflection;
using DaLion.Shared.Attributes;
using DaLion.Shared.Extensions.Reflection;
using HarmonyLib;

#endregion using directives

/// <summary>Instantiates and applies <see cref="IHarmonyPatch"/> classes in the assembly.</summary>
internal sealed class Harmonizer
{
    /// <inheritdoc cref="IModRegistry"/>
    private readonly IModRegistry _modRegistry;

    /// <summary>Initializes a new instance of the <see cref="Harmonizer"/> class.</summary>
    /// <param name="modRegistry">API for fetching metadata about loaded mods.</param>
    /// <param name="uniqueId">The unique ID of the declaring module.</param>
    internal Harmonizer(IModRegistry modRegistry, string uniqueId)
    {
        this._modRegistry = modRegistry;
        this.Harmony = new Harmony(uniqueId);
        this.UniqueId = uniqueId;
    }

    /// <inheritdoc cref="HarmonyLib.Harmony"/>
    internal Harmony Harmony { get; }

    /// <summary>Gets the unique ID of the <see cref="HarmonyLib.Harmony"/> instance.</summary>
    internal string UniqueId { get; }

    /// <summary>Instantiates and applies one of every <see cref="IHarmonyPatch"/> class in the assembly using reflection.</summary>
    /// <param name="scoped">Indicates whether to limit the scope of applied <see cref="IHarmonyPatch"/>es to the namespace equal to the unique ID of the <see cref="HarmonyLib.Harmony"/> instance.</param>
    internal void ApplyAll(bool scoped = false)
    {
        var sw = new Stopwatch();
        sw.Start();

        var where = scoped ? "in " + this.UniqueId : string.Empty;
        Log.D($"[Harmonizer]: Gathering patches {where}...");
        var patchTypes = AccessTools
            .GetTypesFromAssembly(Assembly.GetAssembly(typeof(IHarmonyPatch)))
            .Where(t => t.IsAssignableTo(typeof(IHarmonyPatch)) && !t.IsAbstract &&
                        (!scoped || t.Namespace?.StartsWith(this.UniqueId) == true))
            .ToArray();

        Log.D($"[Harmonizer]: Found {patchTypes.Length} patch classes. Applying patches...");
        foreach (var p in patchTypes)
        {
            try
            {
#if RELEASE
                var debugAttribute = p.GetCustomAttribute<DebugAttribute>();
                if (debugAttribute is not null) continue;
#endif

                var deprecatedAttr = p.GetCustomAttribute<DeprecatedAttribute>();
                if (deprecatedAttr is not null)
                {
                    continue;
                }

                var integrationAttr = p.GetCustomAttribute<IntegrationAttribute>();
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
                            $" Please update {integrationAttr.UniqueId} in order to enable integrations with {this.UniqueId}.");
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

                patch.Apply(this.Harmony);
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
