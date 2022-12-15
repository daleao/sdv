#region global using directives

#pragma warning disable SA1200 // Using directives should be placed correctly
global using static DaLion.Overhaul.ModEntry;
global using static DaLion.Overhaul.Modules.OverhaulModule;
#pragma warning restore SA1200 // Using directives should be placed correctly

#endregion global using directives

namespace DaLion.Overhaul;

#region using directives

using System.Collections.Generic;
using DaLion.Shared.Events;
using DaLion.Shared.Integrations;
using DaLion.Shared.ModData;
using DaLion.Shared.Networking;
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

    /// <summary>Gets the <see cref="Shared.Events.EventManager"/> instance.</summary>
    internal static EventManager EventManager { get; private set; } = null!; // set in Entry

    /// <summary>Gets the <see cref="Reflector"/> instance.</summary>
    internal static Reflector Reflector { get; private set; } = null!; // set in Entry

    /// <summary>Gets the <see cref="Broadcaster"/> instance.</summary>
    internal static Broadcaster Broadcaster { get; private set; } = null!; // set in Entry

    /// <summary>Gets the cache of <see cref="BaseIntegration"/>s.</summary>
    internal static Dictionary<string, BaseIntegration> Integrations { get; } = new();

    /// <summary>Gets the <see cref="IModHelper"/> API.</summary>
    internal static IModHelper ModHelper => Instance.Helper;

    /// <summary>Gets the <see cref="IManifest"/> for this mod.</summary>
    internal static IManifest Manifest => Instance.ModManifest;

    /// <summary>Gets the <see cref="ITranslationHelper"/> API.</summary>
    internal static ITranslationHelper I18n => ModHelper.Translation;

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
        Config.Validate(helper);

        // initialize mod state
        PerScreenState = new PerScreen<ModState>(() => new ModState());

        // initialize event manager
        EventManager = new EventManager(helper.Events, helper.ModRegistry);

        // initialize reflector
        Reflector = new Reflector();

        // initialize multiplayer broadcaster
        Broadcaster = new Broadcaster(helper.Multiplayer, this.ModManifest.UniqueID);

        // initialize modules
        Core.Activate(helper);

        if (Config.EnableArsenal)
        {
            Arsenal.Activate(helper);
        }

        if (Config.EnablePonds)
        {
            Ponds.Activate(helper);
        }

        if (Config.EnableProfessions)
        {
            Professions.Activate(helper);
        }

        if (Config.EnableRings)
        {
            Rings.Activate(helper);
        }

        if (Config.EnableTaxes)
        {
            Taxes.Activate(helper);
        }

        if (Config.EnableTools)
        {
            Tools.Activate(helper);
        }

        if (Config.EnableTweex)
        {
            Tweex.Activate(helper);
        }

        // validate multiplayer
        if (!Context.IsMultiplayer || Context.IsMainPlayer || Context.IsSplitScreen)
        {
            return;
        }

        var host = helper.Multiplayer.GetConnectedPlayer(Game1.MasterPlayer.UniqueMultiplayerID)!;
        var hostMod = host.GetMod(this.ModManifest.UniqueID);
        if (hostMod is null)
        {
            Log.W(
                "Modular Overhaul was not installed by the session host. Most features will not work properly.");
        }
        else if (!hostMod.Version.Equals(this.ModManifest.Version))
        {
            Log.W(
                $"The session host has a different version of Modular Overhaul installed. Some features may not work properly.\n\tHost version: {hostMod.Version}\n\tLocal version: {this.ModManifest.Version}");
        }
    }

    /// <inheritdoc />
    public override object GetApi()
    {
        return new ModApi();
    }
}
