namespace DaLion.Stardew.Tweaks;

#region using directives

using System;
using System.Reflection;
using HarmonyLib;
using StardewModdingAPI;

using Framework;
using Framework.Events;
using Framework.Patches.Integrations;

#endregion using directives

/// <summary>The mod entry point.</summary>
public class ModEntry : Mod
{
    internal static ModConfig Config { get; set; }
    internal static object ProfessionsConfig { get; set; }

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

        var professionsInfo = helper.ModRegistry.Get("DaLion.ImmersiveProfessions");
        if (professionsInfo is not null)
        {
            Log("Detected ImmersiveProfessions. Enabling integration...", LogLevel.Info);
            var professionsEntry = (IMod) professionsInfo.GetType().GetProperty("Mod")!.GetValue(professionsInfo);
            ProfessionsConfig = professionsEntry!.Helper.ReadConfig<object>();
        }


        // register asset editors / loaders
        helper.Content.AssetLoaders.Add(new AssetLoader());

        // hook events
        new GameLaunchedEvent().Hook();
        new ModMessageReceivedEvent().Hook();

        // apply harmony patches
        var harmony = new Harmony(Manifest.UniqueID);
        harmony.PatchAll(Assembly.GetExecutingAssembly());

        if (helper.ModRegistry.IsLoaded("Pathoschild.Automate"))
            AutomatePatches.Apply(harmony);

        if (helper.ModRegistry.IsLoaded("cat.betterartisangoodicons"))
            BetterArtisanGoodIconsPatches.Apply(harmony);
    }
}