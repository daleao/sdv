namespace DaLion.Stardew.Ponds;

#region using directives

using System;
using System.Reflection;
using HarmonyLib;
using StardewModdingAPI;

using Framework.Events;
using Framework.Patches.Integrations;

#endregion using directives

/// <summary>The mod entry point.</summary>
public class ModEntry : Mod
{
    internal static ModEntry Instance { get; private set; }
    internal static IModHelper ModHelper => Instance.Helper;
    internal static IManifest Manifest => Instance.ModManifest;
    internal static Action<string, LogLevel> Log => Instance.Monitor.Log;

    internal static ModConfig Config { get; set; }

    /// <summary>The mod entry point, called after the mod is first loaded.</summary>
    /// <param name="helper">Provides simplified APIs for writing mods.</param>
    public override void Entry(IModHelper helper)
    {
        Instance = this;

        // get configs
        Config = helper.ReadConfig<ModConfig>();

        // hook events
        IEvent.HookAll();

        // apply harmony patches
        var harmony = new Harmony(ModManifest.UniqueID);
        harmony.PatchAll(Assembly.GetExecutingAssembly());
        
        if (helper.ModRegistry.IsLoaded("Pathoschild.Automate"))
            AutomatePatches.Apply(harmony);

        if (helper.ModRegistry.IsLoaded("TehPers.FishingOverhaul"))
            TehsFishingOverhaulPatches.Apply(harmony);

        // add debug commands
        helper.ConsoleCommands.Register();
    }
}