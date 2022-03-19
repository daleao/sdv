namespace DaLion.Stardew.Rings;

#region using directives

using System;
using System.Reflection;
using HarmonyLib;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using Framework.AssetEditors;

using Framework.AssetLoaders;
using Integrations;

#endregion using directives

/// <summary>The mod entry point.</summary>
public class ModEntry : Mod
{
    internal static ModConfig Config { get; set; }
    internal static IModHelper ModHelper { get; private set; }
    internal static IManifest Manifest { get; private set; }
    internal static Action<string, LogLevel> Log { get; private set; }

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

        // register asset editors / loaders
        helper.Content.AssetEditors.Add(new CraftingRecipesEditor());
        helper.Content.AssetEditors.Add(new ObjectInformationEditor());
        helper.Content.AssetEditors.Add(new SpringObjectsEditor());
        helper.Content.AssetLoaders.Add(new AssetLoader());

        // register events
        helper.Events.GameLoop.GameLaunched += OnGameLaunched;

        // apply harmony patches
        new Harmony(ModManifest.UniqueID).PatchAll(Assembly.GetExecutingAssembly());

        // add debug commands
        ConsoleCommands.Register(helper);
    }

    /// <summary>Raised after the game is launched, right before the first update tick.</summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event data.</param>
    private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
    {
        // add Generic Mod Config Menu integration
        new GenericModConfigMenuIntegrationForImmersiveRings(
            getConfig: () => Config,
            reset: () =>
            {
                Config = new();
                ModHelper.WriteConfig(Config);
            },
            saveAndApply: () => { ModHelper.WriteConfig(Config); },
            log: Log,
            modRegistry: ModHelper.ModRegistry,
            manifest: Manifest
        ).Register();
    }
}