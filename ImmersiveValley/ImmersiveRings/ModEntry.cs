namespace DaLion.Stardew.Rings;

#region using directives

using StardewModdingAPI;

using Common;
using Common.Events;
using Common.Harmony;

#endregion using directives

/// <summary>The mod entry point.</summary>
public class ModEntry : Mod
{
    internal static ModEntry Instance { get; private set; }
    internal static ModConfig Config { get; set; }

    internal static IModHelper ModHelper => Instance.Helper;
    internal static IManifest Manifest => Instance.ModManifest;
    internal static ITranslationHelper i18n => ModHelper.Translation;

    internal static bool HasLoadedBetterRings { get; private set; }

    /// <summary>The mod entry point, called after the mod is first loaded.</summary>
    /// <param name="helper">Provides simplified APIs for writing mods.</param>
    public override void Entry(IModHelper helper)
    {
        Instance = this;

        // initialize logger
        Log.Init(Monitor);

        // get configs
        Config = helper.ReadConfig<ModConfig>();

        // hook events
        new EventManager(helper.Events).HookAll();

        // apply patches
        new HarmonyPatcher(ModManifest.UniqueID).ApplyAll();

        // check for Better Rings mod
        HasLoadedBetterRings = helper.ModRegistry.IsLoaded("BBR.BetterRings");
    }
}