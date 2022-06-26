namespace DaLion.Stardew.Alchemy;

#region using directives

using Common;
using Common.Data;
using Common.Harmony;
using Framework;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using StardewValley;

#endregion using directives

/// <summary>The mod entry point.</summary>
public class ModEntry : Mod
{
    internal static ModEntry Instance { get; private set; } = null!;
    internal static ModConfig Config { get; set; } = null!;
    internal static AlchemyEventManager EventManager { get; private set; } = null!;
    internal static PerScreen<PlayerState> PerScreenState { get; private set; } = null!;
    internal static PlayerState PlayerState
    {
        get => PerScreenState.Value;
        set => PerScreenState.Value = value;
    }

    internal static IModHelper ModHelper => Instance.Helper;
    internal static IManifest Manifest => Instance.ModManifest;
    internal static ITranslationHelper i18n => ModHelper.Translation;

    internal static bool LoadedBackpackMod { get; private set; }

    internal static FrameRateCounter FpsCounter { get; private set; } = null!;
    internal static ICursorPosition DebugCursorPosition { get; set; } = null!;

    /// <summary>The mod entry point, called after the mod is first loaded.</summary>
    /// <param name="helper">Provides simplified APIs for writing mods.</param>
    public override void Entry(IModHelper helper)
    {
        Instance = this;

        // initialize logger
        Log.Init(Monitor);

        // initialize data
        ModDataIO.Init(helper.Multiplayer, ModManifest.UniqueID);

        // get configs
        Config = helper.ReadConfig<ModConfig>();

        // initialize mod events
        EventManager = new(Helper.Events);

        // apply harmony patches
        new Harmonizer(Manifest.UniqueID).ApplyAll();

        // initialize mod state
        PerScreenState = new(() => new());

        // load content packs
        SubstanceManager.Init(helper.ContentPacks);

        // register commands
        helper.ConsoleCommands.Register();

        // validate multiplayer
        if (Context.IsMultiplayer && !Context.IsMainPlayer && !Context.IsSplitScreen)
        {
            var host = helper.Multiplayer.GetConnectedPlayer(Game1.MasterPlayer.UniqueMultiplayerID)!;
            var hostMod = host.GetMod(ModManifest.UniqueID);
            if (hostMod is null)
                Log.W("[Entry] The session host does not have this mod installed. Some features will not work properly.");
            else if (!hostMod.Version.Equals(ModManifest.Version))
                Log.W(
                    $"[Entry] The session host has a different mod version. Some features may not work properly.\n\tHost version: {hostMod.Version}\n\tLocal version: {ModManifest.Version}");
        }

        // check for Larger Backpack mod
        LoadedBackpackMod = helper.ModRegistry.IsLoaded("spacechase0.BiggerBackpack");

#if DEBUG
        // start FPS counter
        FpsCounter = new(GameRunner.instance);
        helper.Reflection.GetMethod(FpsCounter, "LoadContent").Invoke();
#endif
    }

    /// <inheritdoc />
    public override object GetApi() => new ModAPI();
}