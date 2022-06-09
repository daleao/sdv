namespace DaLion.Stardew.Arsenal;

#region using directives

using System;
using System.Reflection;
using HarmonyLib;
using StardewModdingAPI;

using Framework.Events;

#endregion using directives

/// <summary>The mod entry point.</summary>
public class ModEntry : Mod
{
    internal static ModEntry Instance { get; private set; }
    internal static ModConfig Config { get; set; }

    internal static IModHelper ModHelper => Instance.Helper;
    internal static IManifest Manifest => Instance.ModManifest;
    internal static ITranslationHelper i18n => ModHelper.Translation;
    internal static Action<string, LogLevel> Log => Instance.Monitor.Log;

    internal static int QiChallengeFinalQuestId => "TrulyLegendaryGalaxySword".GetHashCode();

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

        // add debug commands
        helper.ConsoleCommands.Register();
    }
}