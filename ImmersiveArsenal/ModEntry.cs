using System.Reflection;
using DaLion.Stardew.Arsenal.Framework.Events;
using HarmonyLib;

namespace DaLion.Stardew.Arsenal;

#region using directives

using System;
using StardewModdingAPI;
using StardewModdingAPI.Events;

using Framework;
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
        helper.Content.AssetEditors.Add(new WeaponsEditor());

        // register events
        new ButtonPressedEvent().Hook();
        new GameLaunchedEvent().Hook();
        new UpdateTickedEvent().Hook();

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
        new GenericModConfigMenuIntegrationForImmersiveArsenal(
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