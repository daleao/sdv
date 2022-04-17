namespace DaLion.Stardew.Professions;

#region using directives

using System;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using StardewValley;

using Framework;
using Framework.AssetEditors;
using Framework.AssetLoaders;

#endregion using directives

/// <summary>The mod entry point.</summary>
public class ModEntry : Mod
{
    private static readonly PerScreen<PlayerState> _playerState = new(() => new());

    internal static ModConfig Config { get; set; }
    internal static HostState HostState { get; private set; }

    internal static string ArsenalModId => "DaLion.ImmersiveArsenal";
    internal static string PondsModId => "DaLion.ImmersivePonds";
    internal static string RingsModId => "DaLion.ImmersiveRings";
    internal static string TweaksModId => "DaLion.ImmersiveTweaks";
    internal static object ArsenalConfig { get; private set; }
    internal static object PondsConfig { get; private set; }
    internal static object RingsConfig { get; private set; }
    internal static object TweaksConfig { get; private set; }

    internal static PlayerState PlayerState
    {
        get => _playerState.Value;
        set => _playerState.Value = value;
    }

    internal static IModHelper ModHelper { get; private set; }
    internal static IManifest Manifest { get; private set; }
    internal static Action<string, LogLevel> Log { get; private set; }

    internal static FrameRateCounter FpsCounter { get; private set; }
    internal static ICursorPosition DebugCursorPosition { get; set; }

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

        // get add-on configs
        var arsenalInfo = helper.ModRegistry.Get(ArsenalModId);
        if (arsenalInfo is not null)
        {
            Log("Detected ImmersiveArsenal. Enabling integration...", LogLevel.Info);
            var arsenalEntry = (IMod) arsenalInfo.GetType().GetProperty("Mod")!.GetValue(arsenalInfo);
            ArsenalConfig = arsenalEntry!.Helper.ReadConfig<object>();
        }

        var pondsInfo = helper.ModRegistry.Get(PondsModId);
        if (pondsInfo is not null)
        {
            Log("Detected ImmersivePonds. Enabling integration...", LogLevel.Info);
            var pondsEntry = (IMod) pondsInfo.GetType().GetProperty("Mod")!.GetValue(pondsInfo);
            PondsConfig = pondsEntry!.Helper.ReadConfig<object>();
        }

        var ringsInfo = helper.ModRegistry.Get(RingsModId);
        if (ringsInfo is not null)
        {
            Log("Detected ImmersiveRings. Enabling integration...", LogLevel.Info);
            var ringsEntry = (IMod) ringsInfo.GetType().GetProperty("Mod")!.GetValue(ringsInfo);
            RingsConfig = ringsEntry!.Helper.ReadConfig<object>();
        }

        var tweaksInfo = helper.ModRegistry.Get(TweaksModId);
        if (tweaksInfo is not null)
        {
            Log("Detected ImmersiveTweaks. Enabling integration...", LogLevel.Info);
            var tweaksEntry = (IMod) tweaksInfo.GetType().GetProperty("Mod")!.GetValue(tweaksInfo);
            TweaksConfig = tweaksEntry!.Helper.ReadConfig<object>();
        }


        // initialize mod state
        if (Context.IsMainPlayer) HostState = new();

        // register asset editors / loaders
        helper.Content.AssetLoaders.Add(new TextureLoader());
        helper.Content.AssetEditors.Add(new FishPondDataEditor());
        helper.Content.AssetEditors.Add(new SpriteEditor());

        // load sound effects
        SoundBank.LoadCollection(helper.DirectoryPath);
        
        // initialize mod events
        EventManager.Init(Helper.Events);

        // apply harmony patches
        PatchManager.ApplyAll(Manifest.UniqueID);

        // add debug commands
        ConsoleCommands.Register(helper.ConsoleCommands);

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
        return new ModApi();
    }
}