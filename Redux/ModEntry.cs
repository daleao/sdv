namespace DaLion.Redux;

#region using directives

using System.Diagnostics.CodeAnalysis;
using DaLion.Shared.Events;
using DaLion.Shared.ModData;
using DaLion.Shared.Multiplayer;
using DaLion.Shared.Reflection;
using StardewModdingAPI.Utilities;

#endregion using directives

/// <summary>The mod entry point.</summary>
public sealed class ModEntry : Mod
{
    /// <summary>Gets the static <see cref="ModEntry"/> instance.</summary>
    internal static ModEntry Instance { get; private set; } = null!; // set in Entry

    /// <summary>Gets or sets the <see cref="ModConfig"/> instance.</summary>
    internal static ModConfig Config { get; set; } = null!; // set in Entry

    /// <summary>Gets the <see cref="PerScreen{T}"/> <see cref="ModState"/>.</summary>
    internal static PerScreen<ModState> PerScreenState { get; private set; } = null!; // set in Entry

    /// <summary>Gets or sets the <see cref="ModState"/> of the local player.</summary>
    internal static ModState State
    {
        get => PerScreenState.Value;
        set => PerScreenState.Value = value;
    }

    /// <summary>Gets the <see cref="EventManager"/> instance.</summary>
    internal static EventManager Events { get; private set; } = null!; // set in Entry

    /// <summary>Gets the <see cref="Reflector"/> instance.</summary>
    internal static Reflector Reflector { get; private set; } = null!; // set in Entry

    /// <summary>Gets the <see cref="Broadcaster"/> instance.</summary>
    internal static Broadcaster Broadcaster { get; private set; } = null!; // set in Entry

    /// <summary>Gets the <see cref="IModHelper"/> API.</summary>
    internal static IModHelper ModHelper => Instance.Helper;

    /// <summary>Gets the <see cref="IManifest"/> for this mod.</summary>
    internal static IManifest Manifest => Instance.ModManifest;

    /// <summary>Gets the <see cref="ITranslationHelper"/> API.</summary>
    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Preference.")]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:Element should begin with upper-case letter", Justification = "Preference.")]
    internal static ITranslationHelper i18n => ModHelper.Translation;

    /// <summary>The mod entry point, called after the mod is first loaded.</summary>
    /// <param name="helper">Provides simplified APIs for writing mods.</param>
    public override void Entry(IModHelper helper)
    {
        Instance = this;

        // initialize logger
        Log.Init(this.Monitor);

        // initialize data
        ModDataIO.Init(helper.Multiplayer, this.ModManifest.UniqueID);

        // get configs
        Config = helper.ReadConfig<ModConfig>();

        // initialize mod state
        PerScreenState = new PerScreen<ModState>(() => new ModState());

        // initialize event manager
        Events = new EventManager(helper.Events, helper.ModRegistry);

        // initialize reflector
        Reflector = new Reflector();

        // initialize multiplayer broadcaster
        Broadcaster = new Broadcaster(helper.Multiplayer, this.ModManifest.UniqueID);

        // initialize modules
        ReduxModule.Core.Initialize(helper);

        if (Config.EnableArsenal)
        {
            ReduxModule.Arsenal.Initialize(helper);
        }

        if (Config.EnablePonds)
        {
            ReduxModule.Ponds.Initialize(helper);
        }

        if (Config.EnableProfessions)
        {
            ReduxModule.Professions.Initialize(helper);
        }

        if (Config.EnableRings)
        {
            ReduxModule.Rings.Initialize(helper);
            Integrations.IsBetterRingsLoaded = helper.ModRegistry.IsLoaded("BBR.BetterRings");
        }

        if (Config.EnableTaxes)
        {
            ReduxModule.Taxes.Initialize(helper);
        }

        if (Config.EnableTools)
        {
            ReduxModule.Tools.Initialize(helper);
            Integrations.IsMoonMisadventuresLoaded = helper.ModRegistry.IsLoaded("spacechase0.MoonMisadventures");
        }

        if (Config.EnableTweex)
        {
            ReduxModule.Tweex.Initialize(helper);
        }

        // validate multiplayer
        if (Context.IsMultiplayer && !Context.IsMainPlayer && !Context.IsSplitScreen)
        {
            var host = helper.Multiplayer.GetConnectedPlayer(Game1.MasterPlayer.UniqueMultiplayerID)!;
            var hostMod = host.GetMod(this.ModManifest.UniqueID);
            if (hostMod is null)
            {
                Log.W(
                    "[Entry] The session host does not have this mod installed. Most features will not work properly.");
            }
            else if (!hostMod.Version.Equals(this.ModManifest.Version))
            {
                Log.W(
                    $"[Entry] The session host has a different mod version. Some features may not work properly.\n\tHost version: {hostMod.Version}\n\tLocal version: {this.ModManifest.Version}");
            }
        }

#if DEBUG
        ReduxModule.Debug.Initialize(helper);

        // start FPS counter
        Globals.FpsCounter = new FrameRateCounter(GameRunner.instance);
        helper.Reflection.GetMethod(Globals.FpsCounter, "LoadContent").Invoke();
#endif
    }

    /// <inheritdoc />
    public override object GetApi()
    {
        return new ModApi();
    }
}
