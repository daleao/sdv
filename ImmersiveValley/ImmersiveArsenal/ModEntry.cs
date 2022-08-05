namespace DaLion.Stardew.Arsenal;

#region using directives

using Common;
using Common.Commands;
using Common.Harmony;
using Common.Integrations.DynamicGameAssets;
using Common.Integrations.WalkOfLife;
using Framework.Events;
using StardewModdingAPI.Utilities;

#endregion using directives

/// <summary>The mod entry point.</summary>
public class ModEntry : Mod
{
    internal static ModEntry Instance { get; private set; } = null!;
    internal static ModConfig Config { get; set; } = null!;
    internal static ArsenalEventManager EventManager { get; private set; } = null!;
    internal static PerScreen<ModState> PerScreenState { get; private set; } = null!;
    internal static ModState State
    {
        get => PerScreenState.Value;
        set => PerScreenState.Value = value;
    }

    internal static IModHelper ModHelper => Instance.Helper;
    internal static IManifest Manifest => Instance.ModManifest;
    internal static ITranslationHelper i18n => ModHelper.Translation;

    internal static IDynamicGameAssetsAPI? DynamicGameAssetsApi { get; set; }
    internal static IImmersiveProfessionsAPI? ProfessionsApi { get; set; }
    internal static bool IsImmersiveRingsLoaded { get; private set; }

    /// <summary>The mod entry point, called after the mod is first loaded.</summary>
    /// <param name="helper">Provides simplified APIs for writing mods.</param>
    public override void Entry(IModHelper helper)
    {
        Instance = this;

        // initialize logger
        Log.Init(Monitor);

        // check for loaded mod integrations
        IsImmersiveRingsLoaded = helper.ModRegistry.IsLoaded("DaLion.ImmersiveRings");

        // get configs
        Config = helper.ReadConfig<ModConfig>();

        // enable events
        EventManager = new(helper.Events);
        EventManager.EnableForLocalPlayer();

        // apply patches
        new Harmonizer(helper.ModRegistry, ModManifest.UniqueID).ApplyAll();

        // initialize mod state
        PerScreenState = new(() => new());

        // register commands
        new CommandHandler(helper.ConsoleCommands).Register("ars", "Arsenal");
    }
}