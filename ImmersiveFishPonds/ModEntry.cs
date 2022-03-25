namespace DaLion.Stardew.FishPonds;

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

        // register asset editors
        helper.Content.AssetEditors.Add(new FishPondDataEditor());

        // hook events
        new SaveLoadedEvent().Hook();
        new SavingEvent().Hook();

        // apply harmony patches
        var harmony = new Harmony(ModManifest.UniqueID);
        harmony.PatchAll(Assembly.GetExecutingAssembly());
        if (helper.ModRegistry.IsLoaded("TehPers.FishingOverhaul"))
            TehsFishingOverhaulPatches.Apply(harmony);

        // add debug commands
        ConsoleCommands.Register(helper.ConsoleCommands);
    }
}