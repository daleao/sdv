namespace DaLion.Stardew.Tweaks;

#region using directives

using System;
using StardewModdingAPI;

using Framework;
using Framework.AssetEditors;

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
        helper.Content.AssetEditors.Add(new WeaponsEditor());

        // apply harmony patches
        PatchManager.ApplyAll(Manifest.UniqueID);

        // add debug commands
        ConsoleCommands.Register(helper);
    }
}