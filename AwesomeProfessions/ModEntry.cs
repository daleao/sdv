using System;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using StardewValley;
using DaLion.Stardew.Professions.Framework;
using DaLion.Stardew.Professions.Framework.AssetEditors;
using DaLion.Stardew.Professions.Framework.Sounds;

namespace DaLion.Stardew.Professions;

/// <summary>The mod entry point.</summary>
public class ModEntry : Mod
{
    internal static ModConfig Config { get; set; }
    internal static PerScreen<ModState> State { get; private set; }
    internal static EventManager EventManager { get; private set; }
    internal static SoundBox SoundBox { get; set; }

    internal static IModHelper ModHelper { get; private set; }
    internal static IManifest Manifest { get; private set; }
    internal static Action<string, LogLevel> Log { get; private set; }

    internal static FrameRateCounter FpsCounter { get; private set; }
    internal static LogLevel DefaultLogLevel { get; private set; } = LogLevel.Trace;

    /// <summary>The mod entry point, called after the mod is first loaded.</summary>
    /// <param name="helper">Provides simplified APIs for writing mods.</param>
    public override void Entry(IModHelper helper)
    {
        // store references to helper, mod manifest and logger
        ModHelper = helper;
        Manifest = ModManifest;
        Log = Monitor.Log;

        // get configs
        Config = helper.ReadConfig<ModConfig>();

        // initialize per-screen state
        State = new(() => new());

        // initialize events
        EventManager = new(helper.Events);

        // apply harmony patches
        new HarmonyPatcher(Manifest.UniqueID).ApplyAll();

        // get mod assets
        helper.Content.AssetEditors.Add(new IconEditor()); // sprite assets
        SoundBox = new(helper.DirectoryPath); // sound assets

        // add debug commands
        ConsoleCommands.Register();

        if (Context.IsMultiplayer && !Context.IsMainPlayer && !Context.IsSplitScreen)
        {
            var host = helper.Multiplayer.GetConnectedPlayer(Game1.MasterPlayer.UniqueMultiplayerID);
            var hostMod = host.GetMod(ModManifest.UniqueID);
            if (hostMod is null)
                Log("[Entry] The session host does not have this mod installed. Some features will not work properly.",
                    LogLevel.Warn);
            else if (!hostMod.Version.Equals(ModManifest.Version))
                Log(
                    $"[Entry] The session host has a different mod version. Some features may not work properly.\n\tHost version: {hostMod.Version}\n\tLocal version: {ModManifest.Version}",
                    LogLevel.Warn);
        }

        if (!Config.EnableDebug) return;

        DefaultLogLevel = LogLevel.Debug;

        // start FPS counter
        FpsCounter = new(GameRunner.instance);
        helper.Reflection.GetMethod(FpsCounter, "LoadContent").Invoke();
    }
}