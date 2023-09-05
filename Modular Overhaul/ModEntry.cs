#region global using directives

#pragma warning disable SA1200 // Using directives should be placed correctly
global using static DaLion.Overhaul.ModEntry;
global using static DaLion.Overhaul.Modules.OverhaulModule;
#pragma warning restore SA1200 // Using directives should be placed correctly

#endregion global using directives

namespace DaLion.Overhaul;

#region using directives

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
    internal static ModData Data { get; set; } = null!; // set in Entry

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

        // check SpaceCore build first of all
        var spaceCoreAssembly = Assembly.Load("SpaceCore");
        if (spaceCoreAssembly.IsDebugBuild())
        {
            Log.E(
                "The installed version of SpaceCore was built in Debug mode, which is not compatible with this mood. Please ask the author to build in Release mode.");
            return;
        }

        ModDataIO.Init(this.ModManifest.UniqueID);
        I18n.Init(helper.Translation);

        Data = helper.Data.ReadJsonFile<ModData>("data.json") ?? new ModData();

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
        this.LogStats();
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
    private void LogStats()
    {
        Log.A($"[Entry]: Initialization completed in {this._sw.ElapsedMilliseconds}ms.");
    }
}
