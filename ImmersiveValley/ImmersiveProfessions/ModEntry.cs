namespace DaLion.Stardew.Professions;

#region using directives

using System;
using Newtonsoft.Json.Linq;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using StardewValley;

using Common.Extensions.Stardew;
using Framework;
using Framework.Sounds;

#endregion using directives

/// <summary>The mod entry point.</summary>
public class ModEntry : Mod
{
    internal static PerScreen<PlayerState> PerScreenState { get; } = new(() => new());

    internal static ModEntry Instance { get; private set; }
    internal static IModHelper ModHelper => Instance.Helper;
    internal static IManifest Manifest => Instance.ModManifest;
    internal static Action<string, LogLevel> Log => Instance.Monitor.Log;

    internal static ModConfig Config { get; set; }
    internal static JObject ArsenalConfig { get; private set; }
    internal static JObject PondsConfig { get; private set; }
    internal static JObject RingsConfig { get; private set; }
    internal static JObject TweaksConfig { get; private set; }

    internal static HostState HostState { get; private set; }
    internal static PlayerState PlayerState
    {
        get => PerScreenState.Value;
        set => PerScreenState.Value = value;
    }

    internal static FrameRateCounter FpsCounter { get; private set; }
    internal static ICursorPosition DebugCursorPosition { get; set; }

    /// <summary>The mod entry point, called after the mod is first loaded.</summary>
    /// <param name="helper">Provides simplified APIs for writing mods.</param>
    public override void Entry(IModHelper helper)
    {
        Instance = this;

        // get configs
        Config = helper.ReadConfig<ModConfig>();

        ArsenalConfig = helper.ReadConfigExt("DaLion.ImmersiveArsenal", Log);
        PondsConfig = helper.ReadConfigExt("DaLion.ImmersivePonds", Log);
        RingsConfig = helper.ReadConfigExt("DaLion.ImmersiveRings", Log);
        TweaksConfig = helper.ReadConfigExt("DaLion.ImmersiveTweaks", Log);

        // initialize mod state
        if (Context.IsMainPlayer) HostState = new();

        // load sound effects
        SoundBank.LoadCollection(helper.DirectoryPath);
        
        // initialize mod events
        EventManager.Init(Helper.Events);

        // apply harmony patches
        PatchManager.ApplyAll(Manifest.UniqueID);

        // add debug commands
        helper.ConsoleCommands.Register();

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