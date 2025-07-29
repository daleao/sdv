namespace DaLion.Shared.Harmony;

#region using directives

using System.Diagnostics;
using System.Linq;
using System.Reflection;
using DaLion.Shared.Attributes;
using DaLion.Shared.Extensions.Reflection;
using HarmonyLib;
using StardewModdingAPI;

#endregion using directives

/// <summary>Instantiates and applies <see cref="IHarmonyPatcher"/> classes in the assembly by searching for <see cref="HarmonyLib"/> <see cref="Attribute"/>s using reflection.</summary>
public sealed class Harmonizer
{
    /// <inheritdoc cref="IModRegistry"/>
    private readonly IModRegistry _modRegistry;

    /// <inheritdoc cref="Stopwatch"/>
    private readonly Stopwatch _sw = new();

    /// <inheritdoc cref="Logger"/>
    private readonly Logger _log;

    private int _appliedPrefixes;
    private int _appliedPostfixes;
    private int _appliedTranspilers;
    private int _appliedFinalizers;

    /// <summary>Initializes a new instance of the <see cref="Harmonizer"/> class.</summary>
    /// <param name="modRegistry">API for fetching metadata about loaded mods.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    /// <param name="harmonyId">The unique ID of the declaring mod.</param>
    private Harmonizer(IModRegistry modRegistry, Logger logger, string harmonyId)
    {
        this._modRegistry = modRegistry;
        this.HarmonyId = harmonyId;
        this.Harmony = new Harmony(harmonyId);
        this._log = logger;
    }

    /// <inheritdoc cref="HarmonyLib.Harmony"/>
    public Harmony Harmony { get; }

    /// <summary>Gets the unique ID of the <see cref="HarmonyLib.Harmony"/> instance.</summary>
    public string HarmonyId { get; }

    /// <summary>Implicitly applies<see cref="IHarmonyPatcher"/> types in the specified <paramref name="assembly"/> using reflection.</summary>
    /// <param name="assembly">The assembly containing the types.</param>
    /// <param name="modRegistry">API for fetching metadata about loaded mods.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    /// <param name="harmonyId">The unique ID of the declaring mod.</param>
    /// <returns>The <see cref="Harmonizer"/> instance.</returns>
    public static Harmonizer ApplyAll(Assembly assembly, IModRegistry modRegistry, Logger logger, string harmonyId)
    {
        logger.D($"[Harmonizer]: Preparing to apply all patches in {assembly.GetName()}...");
        return new Harmonizer(modRegistry, logger, harmonyId).ApplyImplicitly(assembly);
    }

    /// <summary>Implicitly applies only the <see cref="IHarmonyPatcher"/> types in the specified <paramref name="assembly"/>> which are also within the specified <paramref name="namespace"/>.</summary>
    /// <param name="assembly">The assembly containing the types.</param>
    /// <param name="namespace">The desired namespace.</param>
    /// <param name="modRegistry">API for fetching metadata about loaded mods.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    /// <param name="harmonyId">The unique ID of the declaring mod. Defaults to <paramref name="namespace"/> if null.</param>
    /// <returns>The <see cref="Harmonizer"/> instance.</returns>
    public static Harmonizer ApplyFromNamespace(
        Assembly assembly,
        string @namespace,
        IModRegistry modRegistry,
        Logger logger,
        string? harmonyId = null)
    {
        logger.D($"[Harmonizer]: Preparing to apply all patches in {@namespace}...");
        return new Harmonizer(modRegistry, logger, harmonyId ?? @namespace)
            .ApplyImplicitly(assembly, t => t.Namespace?.StartsWith(@namespace) ?? false);
    }

    /// <summary>Implicitly applies only the <see cref="IHarmonyPatcher"/> types with the specified <paramref name="assembly"/>> which are also decorated with <typeparamref name="TAttribute"/>.</summary>
    /// <typeparam name="TAttribute">An <see cref="Attribute"/> type.</typeparam>
    /// <param name="assembly">The assembly containing the types.</param>
    /// <param name="modRegistry">API for fetching metadata about loaded mods.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    /// <param name="harmonyId">The unique ID of the declaring mod.</param>
    /// <returns>The <see cref="Harmonizer"/> instance.</returns>
    public static Harmonizer ApplyWithAttribute<TAttribute>(
        Assembly assembly,
        IModRegistry modRegistry,
        Logger logger,
        string harmonyId)
        where TAttribute : Attribute
    {
        logger.D($"[Harmonizer]: Gathering patches with {nameof(TAttribute)}...");
        return new Harmonizer(modRegistry, logger, harmonyId)
            .ApplyImplicitly(assembly, t => t.HasAttribute<TAttribute>());
    }

    /// <summary>Unapplies all <see cref="IHarmonyPatcher"/>s applied by this instance.</summary>
    /// <returns>Always <see langword="null"/>.</returns>
    public Harmonizer Unapply()
    {
        this.Harmony.UnpatchAll(this.HarmonyId);
        this._appliedPrefixes = 0;
        this._appliedPostfixes = 0;
        this._appliedTranspilers = 0;
        this._appliedFinalizers = 0;
        this._log.D($"[Harmonizer]: Unapplied all patches for {this.HarmonyId}.");
        return this;
    }

    /// <summary>Instantiates and applies <see cref="IHarmonyPatcher"/> classes in the specified <paramref name="assembly"/> using reflection.</summary>
    /// <param name="assembly">The assembly to search within.</param>
    /// <param name="predicate">An optional condition with which to limit the scope of applied <see cref="IHarmonyPatcher"/>es.</param>
    /// <returns>The <see cref="Harmonizer"/> instance.</returns>
    private Harmonizer ApplyImplicitly(Assembly assembly, Func<Type, bool>? predicate = null)
    {
        this.StartWatch();

        predicate ??= _ => true;
        var patchTypes = AccessTools
            .GetTypesFromAssembly(assembly)
            .Where(t => t.IsAssignableTo(typeof(IHarmonyPatcher)) && !t.IsAbstract && predicate(t))
            .ToArray();

        this._log.D($"[Harmonizer]: Found {patchTypes.Length} patch classes.");
        if (patchTypes.Length == 0)
        {
            return this;
        }

        this._log.D("[Harmonizer]: Applying patches...");
        foreach (var patchType in patchTypes)
        {
#if RELEASE
            var debugAttribute = patchType.GetCustomAttribute<DebugAttribute>();
            if (debugAttribute is not null)
            {
                continue;
            }
#endif

            var ignoreAttribute = patchType.GetCustomAttribute<ImplicitIgnoreAttribute>();
            if (ignoreAttribute is not null)
            {
                this._log.D($"[Harmonizer]: {patchType.Name} is marked to be ignored.");
                continue;
            }

            var modRequirementAttribute = patchType.GetCustomAttribute<ModRequirementAttribute>();
            if (modRequirementAttribute is not null)
            {
                if (!this._modRegistry.IsLoaded(modRequirementAttribute.UniqueId))
                {
                    this._log.D(
                        $"[Harmonizer]: The target mod {modRequirementAttribute.UniqueId} is not loaded. {patchType.Name} will be ignored.");
                    continue;
                }

                var installedVersion = this._modRegistry.Get(modRequirementAttribute.UniqueId)!.Manifest.Version;
                if (!string.IsNullOrEmpty(modRequirementAttribute.Version) &&
                    installedVersion.IsOlderThan(modRequirementAttribute.Version))
                {
                    this._log.W(
                        $"[Harmonizer]: The integration patch {patchType.Name} will be ignored because the installed version of {modRequirementAttribute.UniqueId} is older than minimum supported version." +
                        $" Please update {modRequirementAttribute.UniqueId} in order to enable integrations with {this.HarmonyId}." +
                        $"\n\tInstalled version: {this._modRegistry.Get(modRequirementAttribute.UniqueId)!.Manifest.Version}\n\tRequired version: {modRequirementAttribute.Version}");
                    continue;
                }
            }

            var modConflictAttribute = patchType.GetCustomAttribute<ModConflictAttribute>();
            if (modConflictAttribute is not null)
            {
                if (this._modRegistry.IsLoaded(modConflictAttribute.UniqueId))
                {
                    this._log.D(
                        $"[Harmonizer]: The conflicting mod {modConflictAttribute.UniqueId} is loaded. {patchType.Name} will be ignored.");
                    continue;
                }
            }

            try
            {
                var patch = (IHarmonyPatcher)patchType
                    .RequireConstructor(this.GetType(), typeof(Logger))
                    .Invoke([this, this._log]);
                if (patch.Apply(this.Harmony))
                {
                    this._log.D($"[Harmonizer]: Applied {patchType.Name} to {patch.Target!.GetFullName()}.");
                }
                else
                {
                    this._log.W(
                        $"[Harmonizer]: {patchType.Name} was partially applied or failed to apply. Some mod features may not work correctly.");
                }
            }
            catch (Exception ex)
            {
                this._log.E($"[Harmonizer]: Failed to apply {patchType.Name}.\nHarmony returned {ex}");
            }
        }

        this.StopWatch();
        this._appliedPrefixes = this.Harmony.GetAllPrefixes(p => p.owner == this.HarmonyId).Count();
        this._appliedPostfixes = this.Harmony.GetAllPostfixes(p => p.owner == this.HarmonyId).Count();
        this._appliedTranspilers = this.Harmony.GetAllTranspilers(p => p.owner == this.HarmonyId).Count();
        this._appliedFinalizers = this.Harmony.GetAllFinalizers(p => p.owner == this.HarmonyId).Count();
        this.LogStats();
        return this;
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
    private void LogStats()
    {
        var patchedMethodsCount = this.Harmony.GetPatchedMethods().Count();
        var totalApplied = this._appliedPrefixes + this._appliedPostfixes + this._appliedTranspilers +
                           this._appliedFinalizers;
        this._log.D($"[Harmonizer]: {this.HarmonyId} patching completed in {this._sw.ElapsedMilliseconds}ms." +
              $"\n\tApplied {totalApplied} patches to {patchedMethodsCount} methods, of which" +
              $"\n\t- {this._appliedPrefixes} prefixes" +
              $"\n\t- {this._appliedPostfixes} postfixes" +
              $"\n\t- {this._appliedTranspilers} transpilers" +
              $"\n\t- {this._appliedFinalizers} finalizers");
    }
}
