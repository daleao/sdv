namespace DaLion.Stardew.Professions;

#region using directives

using Common;
using Common.Commands;
using Common.Harmony;
using Common.Integrations.LoveOfCooking;
using Common.Integrations.LuckSkill;
using Common.Integrations.SpaceCore;
using Common.ModData;
using Common.Multiplayer;
using Framework;
using Framework.Events;
using Newtonsoft.Json.Linq;
using StardewModdingAPI.Utilities;
using System;
using System.Collections.Generic;

#endregion using directives

/// <summary>The mod entry point.</summary>
public class ModEntry : Mod
{
    internal static ModEntry Instance { get; private set; } = null!;
    internal static ModConfig Config { get; set; } = null!;
    internal static ProfessionEventManager EventManager { get; private set; } = null!;
    internal static PerScreen<ModState> PerScreenState { get; private set; } = null!;
    internal static ModState State
    {
        get => PerScreenState.Value;
        set => PerScreenState.Value = value;
    }
    internal static Broadcaster Broadcaster { get; private set; } = null!;

    internal static JObject? ArsenalConfig { get; set; }
    internal static JObject? PondsConfig { get; set; }
    internal static JObject? RingsConfig { get; set; }
    internal static JObject? TaxesConfig { get; set; }
    internal static JObject? TweaksConfig { get; set; }
    internal static JObject? SVEConfig { get; set; }
    internal static ISpaceCoreAPI? SpaceCoreApi { get; set; }
    internal static ILuckSkillAPI? LuckSkillApi { get; set; }
    internal static ICookingSkillAPI? CookingSkillApi { get; set; }

    /// <remarks><see cref="ISkill"/> is used instead of <see cref="CustomSkill"/> because the dictionary must also cache <see cref="LuckSkill"/> which does not use SpaceCore.</remarks>
    internal static Dictionary<string, ISkill> CustomSkills { get; set; } = new();
    internal static Dictionary<int, CustomProfession> CustomProfessions { get; set; } = new();
    internal static Lazy<HudPointer> Pointer { get; } = new(() => new());

    internal static IModHelper ModHelper => Instance.Helper;
    internal static IManifest Manifest => Instance.ModManifest;
    internal static ITranslationHelper i18n => ModHelper.Translation;

#if DEBUG
    internal static FrameRateCounter? FpsCounter { get; private set; }
    internal static ICursorPosition? DebugCursorPosition { get; set; }
#endif

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
        new Harmonizer(helper.ModRegistry, Manifest.UniqueID).ApplyAll();

        // initialize mod state
        PerScreenState = new(() => new());

        // initialize multiplayer broadcaster
        Broadcaster = new(helper.Multiplayer, ModManifest.UniqueID);

        // register commands
        new CommandHandler(helper.ConsoleCommands).Register("wol", "Walk Of Life");

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
    public override object GetApi() => new ModAPI();
}