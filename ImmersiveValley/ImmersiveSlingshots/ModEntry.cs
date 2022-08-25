namespace DaLion.Stardew.Slingshots;

#region using directives

using Common;
using Common.Commands;
using Common.Events;
using Common.Harmony;
using Common.Integrations.WalkOfLife;
using Framework.Events;
using Newtonsoft.Json.Linq;
using StardewModdingAPI.Utilities;

#endregion using directives

/// <summary>The mod entry point.</summary>
public class ModEntry : Mod
{
    internal static ModEntry Instance { get; private set; } = null!;
    internal static ModConfig Config { get; set; } = null!;
    internal static EventManager Events { get; private set; } = null!;
    internal static PerScreen<int> SlingshotCooldown { get; } = new(() => default);

    internal static IModHelper ModHelper => Instance.Helper;
    internal static IManifest Manifest => Instance.ModManifest;
    internal static ITranslationHelper i18n => ModHelper.Translation;

    internal static IImmersiveProfessionsAPI? ProfessionsApi { get; set; }
    internal static JObject? ArsenalConfig { get; set; }

    /// <summary>The mod entry point, called after the mod is first loaded.</summary>
    /// <param name="helper">Provides simplified APIs for writing mods.</param>
    public override void Entry(IModHelper helper)
    {
        Instance = this;

        // initialize logger
        Log.Init(Monitor);

        // get configs
        Config = helper.ReadConfig<ModConfig>();

        // enable events
        Events = new(helper.Events);
        if (Config.FaceMouseCursor) Events.Enable<SlingshotButtonPressedEvent>();

        // apply patches
        new Harmonizer(helper.ModRegistry, ModManifest.UniqueID).ApplyAll();

        // register commands
        new CommandHandler(helper.ConsoleCommands).Register("dsp", "Desperado");
    }
}