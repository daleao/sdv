namespace DaLion.Stardew.Professions;

#region using directives

using System.Collections.Generic;
using JetBrains.Annotations;
using Newtonsoft.Json.Linq;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using StardewValley;

using Common;
using Common.Classes;
using Common.Commands;
using Common.Data;
using Common.Harmony;
using Common.Integrations;
using Framework;

#endregion using directives

/// <summary>The mod entry point.</summary>
public class ModEntry : Mod
{

    internal static ModEntry Instance { get; private set; }
    internal static ModConfig Config { get; set; }
    internal static ProfessionEventManager EventManager { get; private set; }
    internal static MultiplayerBroadcaster Broadcaster { get; private set; }
    internal static HostState HostState { get; private set; }
    internal static PerScreen<PlayerState> PerScreenState { get; private set; }
    internal static PlayerState PlayerState
    {
        get => PerScreenState.Value;
        set => PerScreenState.Value = value;
    }

    [CanBeNull] internal static JObject ArsenalConfig { get; set; }
    [CanBeNull] internal static JObject PondsConfig { get; set; }
    [CanBeNull] internal static JObject RingsConfig { get; set; }
    [CanBeNull] internal static JObject TaxesConfig { get; set; }
    [CanBeNull] internal static JObject TweaksConfig { get; set; }
    [CanBeNull] internal static JObject SVEConfig { get; set; }
    [CanBeNull] internal static ISpaceCoreAPI SpaceCoreApi { get; set; }
    [CanBeNull] internal static ICookingSkillAPI CookingSkillApi { get; set; }
    [CanBeNull] internal static ILuckSkillAPI LuckSkillApi { get; set; }

    /// <remarks><see cref="ISkill"/> is used instead of <see cref="CustomSkill"/> because the dictionary must also cache <see cref="LuckSkill"/> which does not use SpaceCore.</remarks>
    internal static Dictionary<string, ISkill> CustomSkills { get; set; } = new();
    internal static Dictionary<int, CustomProfession> CustomProfessions { get; set; } = new();

    internal static IModHelper ModHelper => Instance.Helper;
    internal static IManifest Manifest => Instance.ModManifest;
    internal static ITranslationHelper i18n => ModHelper.Translation;


    internal static FrameRateCounter FpsCounter { get; private set; }
    internal static ICursorPosition DebugCursorPosition { get; set; }

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
        new HarmonyPatcher(Manifest.UniqueID).ApplyAll();

        // initialize multiplayer broadcaster
        Broadcaster = new(helper.Multiplayer, ModManifest.UniqueID);

        // initialize mod state
        PerScreenState = new(() => new());
        if (Context.IsMainPlayer) HostState = new();

        // register commands
        new CommandHandler(helper.ConsoleCommands).Register("wol", ModManifest.UniqueID);

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