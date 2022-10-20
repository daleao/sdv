namespace DaLion.Stardew.Professions;

#region using directives

using System.Diagnostics.CodeAnalysis;
using DaLion.Common.Commands;
using DaLion.Common.Harmony;
using DaLion.Common.Integrations.LoveOfCooking;
using DaLion.Common.Integrations.LuckSkill;
using DaLion.Common.Integrations.SpaceCore;
using DaLion.Common.ModData;
using DaLion.Common.Multiplayer;
using DaLion.Stardew.Professions.Framework;
using DaLion.Stardew.Professions.Framework.Events;
using Newtonsoft.Json.Linq;
using StardewModdingAPI.Utilities;

#endregion using directives

/// <summary>The mod entry point.</summary>
public sealed class ModEntry : Mod
{
    /// <summary>Gets the static <see cref="ModEntry"/> instance.</summary>
    internal static ModEntry Instance { get; private set; } = null!;

    /// <summary>Gets or sets the <see cref="ModConfig"/> instance.</summary>
    internal static ModConfig Config { get; set; } = null!;

    /// <summary>Gets the <see cref="ProfessionEventManager"/> instance.</summary>
    internal static ProfessionEventManager Events { get; private set; } = null!;

    /// <summary>Gets the <see cref="PerScreen{T}"/> <see cref="ModState"/>.</summary>
    internal static PerScreen<ModState> PerScreenState { get; private set; } = null!;

    /// <summary>Gets or sets the <see cref="ModState"/> of the local player.</summary>
    internal static ModState State
    {
        get => PerScreenState.Value;
        set => PerScreenState.Value = value;
    }

    internal static Broadcaster Broadcaster { get; private set; } = null!;

    internal static JObject? ArsenalConfig { get; set; }

    internal static JObject? PondsConfig { get; set; }

    internal static JObject? RingsConfig { get; set; }

    internal static JObject? SlingshotsConfig { get; set; }

    internal static JObject? TaxesConfig { get; set; }

    internal static JObject? TweaksConfig { get; set; }

    internal static JObject? SveConfig { get; set; }

    internal static Lazy<HudPointer> Pointer { get; } = new(() => new HudPointer());

    /// <summary>Gets the <see cref="IModHelper"/> API.</summary>
    internal static IModHelper ModHelper => Instance.Helper;

    /// <summary>Gets the <see cref="IManifest"/> for this mod.</summary>
    internal static IManifest Manifest => Instance.ModManifest;

    /// <summary>Gets the <see cref="ITranslationHelper"/> API.</summary>
    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Preference.")]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:Element should begin with upper-case letter", Justification = "Preference.")]
    internal static ITranslationHelper i18n => ModHelper.Translation;

    /// <summary>Gets or sets the <see cref="ISpaceCoreApi"/>.</summary>
    internal static ISpaceCoreApi? SpaceCoreApi { get; set; }

    /// <summary>Gets or sets the <see cref="ILuckSkillApi"/>.</summary>
    internal static ILuckSkillApi? LuckSkillApi { get; set; }

    /// <summary>Gets or sets the <see cref="ICookingSkillApi"/>.</summary>
    internal static ICookingSkillApi? CookingSkillApi { get; set; }

    internal static FrameRateCounter? FpsCounter { get; private set; }

    internal static ICursorPosition? DebugCursorPosition { get; set; }

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

        // initialize mod events
        Events = new ProfessionEventManager(this.Helper.Events);

        // initialize mod state
        PerScreenState = new PerScreen<ModState>(() => new ModState());

        // initialize multiplayer broadcaster
        Broadcaster = new Broadcaster(helper.Multiplayer, this.ModManifest.UniqueID);

        // apply harmony patches
        new Harmonizer(helper.ModRegistry, Manifest.UniqueID).ApplyAll();

        // register commands
        new CommandHandler(helper.ConsoleCommands).Register("wol", "Walk Of Life");

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
        // start FPS counter
        FpsCounter = new FrameRateCounter(GameRunner.instance);
        helper.Reflection.GetMethod(FpsCounter, "LoadContent").Invoke();
#endif
    }

    /// <inheritdoc />
    public override object GetApi()
    {
        return new ModApi();
    }
}
