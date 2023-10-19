#region global using directives

#pragma warning disable SA1200 // Using directives should be placed correctly
global using static DaLion.Overhaul.ModEntry;
global using static DaLion.Overhaul.Modules.OverhaulModule;
#pragma warning restore SA1200 // Using directives should be placed correctly

#endregion global using directives

namespace DaLion.Overhaul;

#region using directives

using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using DaLion.Shared.Events;
using DaLion.Shared.Extensions.Collections;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Extensions.SMAPI;
using DaLion.Shared.ModData;
using DaLion.Shared.Networking;
using DaLion.Shared.Reflection;
using StardewModdingAPI.Utilities;

#endregion using directives

/// <summary>The mod entry point.</summary>
public sealed class ModEntry : Mod
{
    /// <inheritdoc cref="Stopwatch"/>
    private readonly Stopwatch _sw = new();

    /// <summary>Gets the static <see cref="ModEntry"/> instance.</summary>
    internal static ModEntry Instance { get; private set; } = null!; // set in Entry

    /// <summary>Gets or sets the <see cref="ModConfig"/> instance.</summary>
    internal static ModConfig Config { get; set; } = null!; // set in Entry

    /// <summary>Gets or sets the <see cref="ModData"/> instance.</summary>
    internal static ModData LocalData { get; set; } = null!; // set in Entry

    /// <summary>Gets the <see cref="PerScreen{T}"/> <see cref="ModState"/>.</summary>
    internal static PerScreen<ModState> PerScreenState { get; private set; } = null!; // set in Entry

    /// <summary>Gets or sets the <see cref="ModState"/> of the local player.</summary>
    internal static ModState State
    {
        get => PerScreenState.Value;
        set => PerScreenState.Value = value;
    }

    /// <summary>Gets the <see cref="Shared.Events.EventManager"/> instance.</summary>
    internal static EventManager EventManager { get; private set; } = null!; // set in Entry

    /// <summary>Gets the <see cref="Reflector"/> instance.</summary>
    internal static Reflector Reflector { get; private set; } = null!; // set in Entry

    /// <summary>Gets the <see cref="Broadcaster"/> instance.</summary>
    internal static Broadcaster Broadcaster { get; private set; } = null!; // set in Entry

    /// <summary>Gets the <see cref="IModHelper"/> API.</summary>
    internal static IModHelper ModHelper => Instance.Helper;

    /// <summary>Gets the <see cref="IManifest"/> for this mod.</summary>
    internal static IManifest Manifest => Instance.ModManifest;

    /// <summary>Gets the <see cref="ITranslationHelper"/> API.</summary>
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:Element should begin with upper-case letter", Justification = "Conflicts with Pathoschild.TranslationBuilder")]
    // ReSharper disable once InconsistentNaming
    internal static ITranslationHelper _I18n => ModHelper.Translation;

    /// <summary>The mod entry point, called after the mod is first loaded.</summary>
    /// <param name="helper">Provides simplified APIs for writing mods.</param>
    public override void Entry(IModHelper helper)
    {
        this.StartWatch();

        Instance = this;
        Log.Init(this.Monitor);

        // pseudo-DRM for low-effort theft
        if (Manifest.UniqueID != "DaLion.Overhaul")
        {
            Log.W(
                "Woops, looks like you downloaded a clandestine version of this mod! Please make sure to download from the official GitHub repo at https://github.com/daleao/modular-overhaul/releases.");
            return;
        }

        // check SpaceCore build first of all
        var spaceCoreAssembly = Assembly.Load("SpaceCore");
        if (spaceCoreAssembly.IsDebugBuild())
        {
            Log.E(
                "The installed version of SpaceCore was built in Debug mode, which is not compatible with this mood. Please ask the author to build in Release mode.");
            return;
        }

        I18n.Init(helper.Translation);
        ModDataIO.Init();

        LocalData = helper.Data.ReadJsonFile<ModData>("data.json") ?? new ModData();

        Config = helper.ReadConfig<ModConfig>();
        Config.Validate(helper);
        Config.Log();

        PerScreenState = new PerScreen<ModState>(() => new ModState());
        EventManager = new EventManager(helper.Events, helper.ModRegistry);
        Reflector = new Reflector();
        Broadcaster = new Broadcaster(helper.Multiplayer, this.ModManifest.UniqueID);
        EnumerateModules().ForEach(module => module.Activate(helper));
        this.ValidateMultiplayer();
        this.StopWatch();
        this.LogTime();
        this.LogHarmonyStats();
        EventManager.LogStats();
        Log.I("[Entry]: Version checksum: " + this.GetType().Assembly.CalculateMd5());
    }

    /// <inheritdoc />
    public override object GetApi()
    {
        return new ModApi();
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
    private void LogTime()
    {
        Log.A($"[Entry]: Initialization completed in {this._sw.ElapsedMilliseconds}ms.");
    }

    [Conditional("DEBUG")]
    private void LogHarmonyStats()
    {
        var patchedMethods = new HashSet<MethodBase>();
        var appliedPrefixes = 0;
        var appliedPostfixes = 0;
        var appliedTranspilers = 0;
        var appliedFinalizers = 0;
        foreach (var module in EnumerateModules())
        {
            if (module.Harmonizer is not { } harmonizer)
            {
                continue;
            }

            appliedPrefixes += harmonizer.AppliedPrefixes;
            appliedPostfixes += harmonizer.AppliedPostfixes;
            appliedTranspilers += harmonizer.AppliedTranspilers;
            appliedFinalizers += harmonizer.AppliedFinalizers;
            foreach (var method in harmonizer.Harmony.GetPatchedMethods())
            {
                if (method is null)
                {
                    continue;
                }

                patchedMethods.Add(method);
            }
        }

        var totalApplied = appliedPrefixes + appliedPostfixes + appliedTranspilers + appliedFinalizers;
        Log.A($"[Entry]: In total, {totalApplied} patches were applied to {patchedMethods.Count} methods, of which:" +
              $"\n\t- {appliedPrefixes} prefixes" +
              $"\n\t- {appliedPostfixes} postfixes" +
              $"\n\t- {appliedTranspilers} transpilers" +
              $"\n\t- {appliedFinalizers} finalizers");
    }
}
