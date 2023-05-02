#region global using directives

#pragma warning disable SA1200 // Using directives should be placed correctly
global using static DaLion.Alchemy.ModEntry;
#pragma warning restore SA1200 // Using directives should be placed correctly

#endregion global using directives

namespace DaLion.Alchemy;

#region using directives
using DaLion.Shared.Commands;
using DaLion.Shared.Events;
using DaLion.Shared.Harmony;
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

        Log.Init(this.Monitor);
        ModDataIO.Init(this.ModManifest.UniqueID);

        Config = helper.ReadConfig<ModConfig>();
        if (!Config.Validate())
        {
            helper.WriteConfig(Config);
        }

        // initialize mod state
        PerScreenState = new PerScreen<ModState>(() => new ModState());

        // initialize reflection
        Reflector = new Reflector();

        // initialize multiplayer broadcaster
        Broadcaster = new Broadcaster(helper.Multiplayer, this.ModManifest.UniqueID);

        // hook events
        EventManager = new EventManager(helper.Events, helper.ModRegistry);
        EventManager.ManageNamespace("DaLion.Alchemy.Events");

        // apply harmony patches
        Harmonizer.ApplyFromNamespace(helper.ModRegistry, "DaLion.Alchemy.Patchers");

        // register console commands
        CommandHandler.HandleFromNamespace(helper.ConsoleCommands, "DaLion.Alchemy.Commands", "Trials of the Grasses", "alch");

        // validate multiplayer
        if (Context.IsMultiplayer && !Context.IsMainPlayer && !Context.IsSplitScreen)
        {
            var host = helper.Multiplayer.GetConnectedPlayer(Game1.MasterPlayer.UniqueMultiplayerID)!;
            var hostMod = host.GetMod(this.ModManifest.UniqueID);
            if (hostMod is null)
            {
                Log.W(
                    "DaLion.Alchemy was not installed by the session host. Most features will not work properly.");
            }
            else if (!hostMod.Version.Equals(this.ModManifest.Version))
            {
                Log.W(
                    $"The session host has a different version of Modular Overhaul installed. Some features may not work properly.\n\tHost version: {hostMod.Version}\n\tLocal version: {this.ModManifest.Version}");
            }
        }
    }

    /// <inheritdoc />
    public override object GetApi()
    {
        return new ModApi();
    }
}
