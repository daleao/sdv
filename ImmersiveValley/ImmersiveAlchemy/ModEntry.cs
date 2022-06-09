namespace DaLion.Stardew.Alchemy;

#region using directives

using System;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using StardewValley;

using Common.Classes;
using Framework;

#endregion using directives

/// <summary>The mod entry point.</summary>
public class ModEntry : Mod
{
    internal static PerScreen<PlayerState> PerScreenState { get; } = new(() => new());

    internal static ModEntry Instance { get; private set; }
    internal static ModConfig Config { get; set; }
    internal static PlayerState PlayerState
    {
        get => PerScreenState.Value;
        set => PerScreenState.Value = value;
    }
    internal static Broadcaster Broadcaster { get; private set; }

    internal static IModHelper ModHelper => Instance.Helper;
    internal static IManifest Manifest => Instance.ModManifest;
    internal static ITranslationHelper i18n => ModHelper.Translation;
    internal static Action<string, LogLevel> Log => Instance.Monitor.Log;

    internal static bool LoadedBackpackMod { get; private set; }

    internal static FrameRateCounter FpsCounter { get; private set; }
    internal static ICursorPosition DebugCursorPosition { get; set; }

    /// <summary>The mod entry point, called after the mod is first loaded.</summary>
    /// <param name="helper">Provides simplified APIs for writing mods.</param>
    public override void Entry(IModHelper helper)
    {
        Instance = this;

        // get configs
        Config = helper.ReadConfig<ModConfig>();

        // load content packs
        SubstanceManager.Init(helper.ContentPacks);

        // initialize mod events
        EventManager.Init(Helper.Events);

        // apply harmony patches
        HarmonyPatcher.ApplyAll(Manifest.UniqueID);

        // add debug commands
        helper.ConsoleCommands.Register();

        // instantiate broadcaster
        Broadcaster = new(helper.Multiplayer, Manifest.UniqueID);

        // validate multiplayer
        if (Context.IsMultiplayer && !Context.IsMainPlayer && !Context.IsSplitScreen)
        {
            var host = helper.Multiplayer.GetConnectedPlayer(Game1.MasterPlayer.UniqueMultiplayerID)!;
            var hostMod = host.GetMod(ModManifest.UniqueID);
            if (hostMod is null)
                Log("[Entry] The session host does not have this mod installed. Some features will not work properly.",
                    LogLevel.Warn);
            else if (!hostMod.Version.Equals(ModManifest.Version))
                Log(
                    $"[Entry] The session host has a different mod version. Some features may not work properly.\n\tHost version: {hostMod.Version}\n\tLocal version: {ModManifest.Version}",
                    LogLevel.Warn);
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
    public override object GetApi()
    {
        return new ModAPI();
    }
}