namespace DaLion.Stardew.Tweex;

#region using directives

using Common;
using Common.Commands;
using Common.Events;
using Common.Harmony;
using Common.Integrations.WalkOfLife;
using Common.ModData;
using StardewModdingAPI;

#endregion using directives

/// <summary>The mod entry point.</summary>
public class ModEntry : Mod
{
    internal static ModEntry Instance { get; private set; } = null!;
    internal static ModConfig Config { get; set; } = null!;

    internal static IModHelper ModHelper => Instance.Helper;
    internal static IManifest Manifest => Instance.ModManifest;

    internal static IImmersiveProfessionsAPI? ProfessionsAPI { get; set; }

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

        // enable events
        new EventManager(helper.Events).EnableAll();

        // apply patches
        new Harmonizer(helper.ModRegistry, ModManifest.UniqueID).ApplyAll();

        // register commands
        new CommandHandler(helper.ConsoleCommands).Register("qol", "Quality Of Life");
    }
}