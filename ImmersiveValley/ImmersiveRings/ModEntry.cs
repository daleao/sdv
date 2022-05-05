namespace DaLion.Stardew.Rings;

#region using directives

using System;
using System.Reflection;
using HarmonyLib;
using StardewModdingAPI;
using StardewModdingAPI.Events;

using Framework.Events;
using Integrations;

#endregion using directives

/// <summary>The mod entry point.</summary>
public class ModEntry : Mod
{
    internal static ModEntry Instance { get; private set; }
    internal static ModConfig Config { get; set; }

    internal static IModHelper ModHelper => Instance.Helper;
    internal static IManifest Manifest => Instance.ModManifest;
    internal static Action<string, LogLevel> Log => Instance.Monitor.Log;

    /// <summary>The mod entry point, called after the mod is first loaded.</summary>
    /// <param name="helper">Provides simplified APIs for writing mods.</param>
    public override void Entry(IModHelper helper)
    {
        Instance = this;

        // get configs
        Config = helper.ReadConfig<ModConfig>();

        // register events
        IEvent.HookAll();

        // apply harmony patches
        new Harmony(ModManifest.UniqueID).PatchAll(Assembly.GetExecutingAssembly());
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