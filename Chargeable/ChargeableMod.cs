global using DaLion.Chargeable.Framework;
global using DaLion.Chargeable.Framework.Extensions;
global using static DaLion.Chargeable.ChargeableMod;

namespace DaLion.Chargeable;

#region using directives

using System.Reflection;
using HarmonyLib;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;

#endregion using directives

/// <summary>The mod entry point.</summary>
public sealed class ChargeableMod : Mod
{
    /// <summary>Gets the static <see cref="ChargeableMod"/> instance.</summary>
    internal static ChargeableMod Instance { get; private set; } = null!; // set in Entry

    /// <summary>Gets or sets the <see cref="ChargeableConfig"/> instance.</summary>
    internal static ChargeableConfig Config { get; set; } = null!; // set in Entry

    /// <summary>Gets the <see cref="PerScreen{T}"/> <see cref="ChargeableState"/>.</summary>
    internal static PerScreen<ChargeableState> PerScreenState { get; private set; } = null!; // set in Entry

    /// <summary>Gets or sets the <see cref="ChargeableState"/> of the local player.</summary>
    internal static ChargeableState State
    {
        get => PerScreenState.Value;
        set => PerScreenState.Value = value;
    }

    /// <summary>Gets the <see cref="Logger"/> instance.</summary>
    internal static Logger Log { get; private set; } = null!; // set in Entry;

    /// <summary>Gets the <see cref="IModHelper"/> API.</summary>
    internal static IModHelper ModHelper => Instance.Helper;

    /// <summary>Gets the <see cref="IManifest"/> API.</summary>
    internal static IManifest Manifest => Instance.ModManifest;

    /// <summary>Gets the unique ID for this mod.</summary>
    internal static string UniqueId => Manifest.UniqueID;

    /// <summary>The mod entry point, called after the mod is first loaded.</summary>
    /// <param name="helper">Provides simplified APIs for writing mods.</param>
    public override void Entry(IModHelper helper)
    {
        Instance = this;
        Log = new Logger(this.Monitor);

        // pseudo-DRM for low-effort theft
        if (Manifest.Author != "DaLion" || UniqueId != this.GetType().Namespace)
        {
            Log.W(
                "Woops, looks like you downloaded a clandestine version of this mod! Please make sure to download from the official mod page at Nexus Mods.");
            return;
        }

        I18n.Init(helper.Translation);
        Config = helper.ReadConfig<ChargeableConfig>();
        Config.Validate(helper);
        PerScreenState = new PerScreen<ChargeableState>(() => new ChargeableState());
        helper.Events.GameLoop.GameLaunched += OnGameLaunched;
        helper.Events.GameLoop.ReturnedToTitle += OnReturnedToTitle;
        helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
        new Harmony(this.ModManifest.UniqueID).PatchAll(Assembly.GetExecutingAssembly());
    }

    private static void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
    {
        if (ChargeableConfigMenu.Instance?.IsLoaded == true)
        {
            ChargeableConfigMenu.Instance.Register();
        }
    }

    private static void OnReturnedToTitle(object? sender, ReturnedToTitleEventArgs e)
    {
        PerScreenState.ResetAllScreens();
    }

    private static void OnUpdateTicked(object? sender, UpdateTickedEventArgs e)
    {
        if (State.Shockwaves.Count == 0 || (Config.TicksBetweenWaves > 1 && !e.IsMultipleOf(Config.TicksBetweenWaves)))
        {
            return;
        }

        for (var i = State.Shockwaves.Count - 1; i >= 0; i--)
        {
            if (State.Shockwaves[i].Update(Game1.currentGameTime.TotalGameTime.TotalMilliseconds))
            {
                State.Shockwaves.RemoveAt(i);
            }
        }
    }
}
