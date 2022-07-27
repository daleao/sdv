namespace DaLion.Stardew.Arsenal;

#region using directives

using Common;
using Common.Commands;
using Common.Events;
using Common.Harmony;
using Common.Integrations.DynamicGameAssets;
using Framework.Events;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;

#endregion using directives

/// <summary>The mod entry point.</summary>
public class ModEntry : Mod
{
    internal static ModEntry Instance { get; private set; } = null!;
    internal static ModConfig Config { get; set; } = null!;
    internal static EventManager Manager { get; private set; } = null!;

    internal static IModHelper ModHelper => Instance.Helper;
    internal static IManifest Manifest => Instance.ModManifest;
    internal static ITranslationHelper i18n => ModHelper.Translation;

    internal static PerScreen<int> SlingshotCooldown { get; } = new(() => 0);
    internal static IDynamicGameAssetsAPI? DynamicGameAssetsApi { get; set; }
    internal static bool IsImmersiveProfessionsLoaded { get; private set; }
    internal static bool IsImmersiveRingsLoaded { get; private set; }

    /// <summary>The mod entry point, called after the mod is first loaded.</summary>
    /// <param name="helper">Provides simplified APIs for writing mods.</param>
    public override void Entry(IModHelper helper)
    {
        Instance = this;

        // initialize logger
        Log.Init(Monitor);

        // check for Immersive mods
        IsImmersiveProfessionsLoaded = helper.ModRegistry.IsLoaded("DaLion.ImmersiveProfessions");
        IsImmersiveRingsLoaded = helper.ModRegistry.IsLoaded("DaLion.ImmersiveRings");

        // get configs
        Config = helper.ReadConfig<ModConfig>();

        // enable events
        Manager = new(helper.Events);
        Manager.EnableAll(typeof(SlingshotSpecialUpdateTickedEvent), typeof(StabbySwordSpecialUpdateTickingEvent));

        // apply patches
        new Harmonizer(helper.ModRegistry, ModManifest.UniqueID).ApplyAll();

        // register commands
        new CommandHandler(helper.ConsoleCommands).Register("ars", "Arsenal");
    }
}