namespace DaLion.Shared.Harmony;

#region using directives

using System.Diagnostics;
using System.Linq;
using System.Reflection;
using DaLion.Shared.Attributes;
using DaLion.Shared.Extensions.Reflection;
using HarmonyLib;

#endregion using directives

/// <summary>Instantiates and applies <see cref="IHarmonyPatcher"/> classes in the assembly.</summary>
internal sealed class Harmonizer
{
    /// <inheritdoc cref="IModRegistry"/>
    private readonly IModRegistry _modRegistry;

    /// <inheritdoc cref="Stopwatch"/>
    private readonly Stopwatch _sw = new();

    /// <summary>Initializes a new instance of the <see cref="Harmonizer"/> class.</summary>
    /// <param name="modRegistry">API for fetching metadata about loaded mods.</param>
    /// <param name="uniqueId">The unique ID of the declaring module.</param>
    internal Harmonizer(IModRegistry modRegistry, string uniqueId)
    {
        this._modRegistry = modRegistry;
        this.UniqueId = uniqueId;
        this.Harmony = new Harmony(uniqueId);
    }

    /// <inheritdoc cref="HarmonyLib.Harmony"/>
    internal Harmony Harmony { get; }

    /// <summary>Gets the unique ID of the <see cref="HarmonyLib.Harmony"/> instance.</summary>
    internal string UniqueId { get; }

    /// <summary>Implicitly applies<see cref="IHarmonyPatcher"/> types in the assembly using reflection.</summary>
    /// <param name="modRegistry">API for fetching metadata about loaded mods.</param>
    /// <param name="uniqueId">The unique ID of the declaring module.</param>
    internal static void ApplyAll(IModRegistry modRegistry, string uniqueId)
    {
        Log.D("[Harmonizer]: Gathering all patches...");
        new Harmonizer(modRegistry, uniqueId).ApplyImplicitly();
    }

    /// <summary>Implicitly applies<see cref="IHarmonyPatcher"/> types in the specified namespace.</summary>
    /// <param name="modRegistry">API for fetching metadata about loaded mods.</param>
    /// <param name="uniqueId">The unique ID of the declaring module.</param>
    /// <param name="namespace">The desired namespace.</param>
    internal static void ApplyFromNamespace(IModRegistry modRegistry, string uniqueId, string? @namespace = null)
    {
        @namespace ??= uniqueId;
        Log.D($"[Harmonizer]: Gathering patches in {@namespace}...");
        new Harmonizer(modRegistry, uniqueId).ApplyImplicitly(t => t.Namespace?.StartsWith(@namespace) == true);
    }

    /// <summary>Implicitly applies<see cref="IHarmonyPatcher"/> types with the specified attribute.</summary>
    /// <param name="modRegistry">API for fetching metadata about loaded mods.</param>
    /// <param name="uniqueId">The unique ID of the declaring module.</param>
    /// <typeparam name="TAttribute">An <see cref="Attribute"/> type.</typeparam>
    internal static void ApplyWithAttribute<TAttribute>(IModRegistry modRegistry, string uniqueId)
        where TAttribute : Attribute
    {
        Log.D($"[Harmonizer]: Gathering patches with {nameof(TAttribute)}...");
        new Harmonizer(modRegistry, uniqueId).ApplyImplicitly(t => t.GetCustomAttribute<TAttribute>() is not null);
    }

    /// <summary>Instantiates and applies <see cref="IHarmonyPatcher"/> classes using reflection.</summary>
    /// <param name="predicate">An optional condition with which to limit the scope of applied <see cref="IHarmonyPatcher"/>es.</param>
    private void ApplyImplicitly(Func<Type, bool>? predicate = null)
    {
        this.StartWatch();

        predicate ??= t => true;
        var patchTypes = AccessTools
            .GetTypesFromAssembly(Assembly.GetAssembly(typeof(IHarmonyPatcher)))
            .Where(t => t.IsAssignableTo(typeof(IHarmonyPatcher)) && !t.IsAbstract && predicate(t))
            .ToArray();

        Log.D($"[Harmonizer]: Found {patchTypes.Length} patch classes. Applying patches...");
        foreach (var p in patchTypes)
        {
#if RELEASE
            var debugAttribute = p.GetCustomAttribute<DebugAttribute>();
            if (debugAttribute is not null)
            {
                continue;
            }
#endif

            var deprecatedAttr = p.GetCustomAttribute<ImplicitIgnoreAttribute>();
            if (deprecatedAttr is not null)
            {
                continue;
            }

            var integrationAttr = p.GetCustomAttribute<RequiresModAttribute>();
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

            try
            {
                var patch = (IHarmonyPatcher?)p
                    .GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null, Type.EmptyTypes, null)
                    ?.Invoke(Array.Empty<object>());
                if (patch is null)
                {
                    ThrowHelper.ThrowMissingMethodException("Didn't find internal parameter-less constructor.");
                }

                if (patch.Apply(this.Harmony))
                {
                    Log.D($"[Harmonizer]: Applied {p.Name} to {patch.Target.GetFullName()}.");
                }
                else
                {
                    Log.W($"[Harmonizer]: {p.Name} was not applied.");
                }
            }
            catch (Exception ex)
            {
                Log.E($"[Harmonizer]: Failed to apply {p.Name}.\nHarmony returned {ex}");
            }
        }

        this.StopWatch();
        this.PrintSummary();
    }

    [Conditional("DEBUG")]
    private void StartWatch()
    {
        this._sw.Start();
    }

    [Conditional("DEBUG")]
    private void StopWatch()
    {
        this._sw.Stop();
    }

    [Conditional("DEBUG")]
    private void PrintSummary()
    {
        var methodsPatched = this.Harmony.GetPatchedMethods().Count();
        var appliedPrefixes = this.Harmony.GetAllPrefixes(p => p.owner == this.UniqueId).Count();
        var appliedPostfixes = this.Harmony.GetAllPostfixes(p => p.owner == this.UniqueId).Count();
        var appliedTranspilers = this.Harmony.GetAllTranspilers(p => p.owner == this.UniqueId).Count();
        var appliedFinalizers = this.Harmony.GetAllFinalizers(p => p.owner == this.UniqueId).Count();
        var totalApplied = appliedPrefixes + appliedPostfixes + appliedTranspilers + appliedFinalizers;
        Log.A($"[Harmonizer]: {this.UniqueId} patching completed in {this._sw.ElapsedMilliseconds}ms." +
              $"\nApplied {totalApplied} patches to {methodsPatched} methods, of which" +
              $"\n\t- {appliedPrefixes} prefixes" +
              $"\n\t- {appliedPostfixes} postfixes" +
              $"\n\t- {appliedTranspilers} transpilers" +
              $"\n\t- {appliedFinalizers} finalizers");
    }
}
