namespace DaLion.Stardew.Arsenal;

#region using directives

using System;
using System.Reflection;
using HarmonyLib;
using StardewModdingAPI;

using Framework.Events;
using Framework.AssetEditors;

#endregion using directives

/// <summary>The mod entry point.</summary>
public class ModEntry : Mod
{
    internal static ModConfig Config { get; set; }

    internal static IModHelper ModHelper { get; private set; }
    internal static IManifest Manifest { get; private set; }
    internal static Action<string, LogLevel> Log { get; private set; }

    internal static int QiChallengeFinalQuestId => "TrulyLegendaryGalaxySword".GetHashCode();

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
        helper.Content.AssetEditors.Add(new RebalancedArsenalEditor());
        helper.Content.AssetEditors.Add(new TrulyLegendaryGalaxyEditor());

        // register events
        IEvent.HookAll();

        // apply harmony patches
        new Harmony(ModManifest.UniqueID).PatchAll(Assembly.GetExecutingAssembly());

        // add debug commands
        helper.ConsoleCommands.Register();
    }
}