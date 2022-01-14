namespace DaLion.Stardew.Tools;

#region using directives

using System;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;

using Configs;
using Framework.Effects;
using Framework.Events;

#endregion using directives

/// <summary>The mod entry point.</summary>
public class ModEntry : Mod
{
    internal static ToolConfig Config { get; set; }
    internal static bool HasMoonMod { get; private set; }

    internal static IModHelper ModHelper { get; private set; }
    internal static IManifest Manifest { get; private set; }
    internal static Action<string, LogLevel> Log { get; private set; }

    internal static PerScreen<Shockwave> Shockwave { get; } = new(() => null);

    /// <summary>The mod entry point, called after the mod is first loaded.</summary>
    /// <param name="helper">Provides simplified APIs for writing mods.</param>
    public override void Entry(IModHelper helper)
    {
        // store references to helper, mod manifest and logger
        ModHelper = helper;
        Manifest = ModManifest;
        Log = Monitor.Log;

        // check for Moon Misadventures mod
        HasMoonMod = helper.ModRegistry.IsLoaded("spacechase0.MoonMisadventures");

        // get and verify configs
        Config = Helper.ReadConfig<ToolConfig>();
        VerifyConfigs();

        // hook events
        new GameLaunchedEvent().Hook();
        new UpdateTickedEvent().Hook();

        // apply harmony patches
        var harmony = new Harmony(ModManifest.UniqueID);
        harmony.PatchAll(Assembly.GetExecutingAssembly());

        // add debug commands
        ConsoleCommands.Register(helper);
    }

    #region private methods

    /// <summary>Check for and fix invalid mod settings.</summary>
    private void VerifyConfigs()
    {
        if (Config.AxeConfig.RadiusAtEachPowerLevel.Count < 4)
        {
            Log("Missing values in AxeConfig.RadiusAtEachPowerLevel. The default values will be restored.",
                LogLevel.Warn);
            Config.AxeConfig.RadiusAtEachPowerLevel = new() {1, 2, 3, 4};
        }
        else if (Config.AxeConfig.RadiusAtEachPowerLevel.Any(i => i <= 0))
        {
            Log(
                "Illegal negative value for shockwave radius in AxeConfig.RadiusAtEachPowerLevel. Those values will be replaced with ones.",
                LogLevel.Warn);
            Config.AxeConfig.RadiusAtEachPowerLevel =
                Config.AxeConfig.RadiusAtEachPowerLevel.Select(i => i <= 0 ? 1 : i).ToList();
        }

        if (Config.PickaxeConfig.RadiusAtEachPowerLevel.Count < 4)
        {
            Log("Missing values PickaxeConfig.RadiusAtEachPowerLevel. The default values will be restored.",
                LogLevel.Warn);
            Config.PickaxeConfig.RadiusAtEachPowerLevel = new() {1, 2, 3, 4};
        }
        else if (Config.PickaxeConfig.RadiusAtEachPowerLevel.Any(i => i <= 0))
        {
            Log(
                "Illegal negative value for shockwave radius in PickaxeConfig.RadiusAtEachPowerLevel. Those values will be replaced with zero.",
                LogLevel.Warn);
            Config.PickaxeConfig.RadiusAtEachPowerLevel =
                Config.PickaxeConfig.RadiusAtEachPowerLevel.Select(i => i <= 0 ? 1 : i).ToList();
        }

        if (Config.RequireModkey && !Config.Modkey.IsBound)
        {
            Log(
                "'RequireModkey' setting is set to true, but no Modkey is bound. Default keybind will be restored. To disable the Modkey, set this value to false.",
                LogLevel.Warn);
            Config.Modkey = KeybindList.ForSingle(SButton.LeftShift);
        }

        if (Config.StaminaCostMultiplier < 0)
            Log("'StaminaCostMultiplier' is set to a negative value. This may cause game-breaking bugs.",
                LogLevel.Warn);

        if (Config.TicksBetweenWaves > 100)
        {
            Log(
                "The value of 'TicksBetweenWaves' is excessively large. This is probably a mistake. The default value will be restored.",
                LogLevel.Warn);
            Config.TicksBetweenWaves = 4;
        }

        if (HasMoonMod)
        {
            Log("Moon Misadventures detected.", LogLevel.Info);

            switch (Config.AxeConfig.RadiusAtEachPowerLevel.Count)
            {
                case < 6:
                    Log("Setting default radius values for higher Axe upgrades.", LogLevel.Info);
                    Config.AxeConfig.RadiusAtEachPowerLevel.AddRange(new[] {5, 6});
                    break;

                case > 6:
                    Log("Too many values in AxeConfig.RadiusAtEachPowerLevel. Additional values will be removed.",
                        LogLevel.Warn);
                    Config.AxeConfig.RadiusAtEachPowerLevel = Config.AxeConfig.RadiusAtEachPowerLevel.Take(6).ToList();
                    break;
            }

            switch (Config.PickaxeConfig.RadiusAtEachPowerLevel.Count)
            {
                case < 6:
                    Log("Setting default radius values for higher Pickaxe upgrades.", LogLevel.Info);
                    Config.PickaxeConfig.RadiusAtEachPowerLevel.AddRange(new[] {5, 6});
                    break;

                case > 6:
                    Log("Too many values in PickaxeConfig.RadiusAtEachPowerLevel. Additional values will be removed.",
                        LogLevel.Warn);
                    Config.PickaxeConfig.RadiusAtEachPowerLevel =
                        Config.PickaxeConfig.RadiusAtEachPowerLevel.Take(6).ToList();
                    break;
            }
        }
        else
        {
            if (Config.AxeConfig.RadiusAtEachPowerLevel.Count > 4)
            {
                Log("Too many values in AxeConfig.RadiusAtEachPowerLevel. Additional values will be removed.",
                    LogLevel.Warn);
                Config.AxeConfig.RadiusAtEachPowerLevel = Config.AxeConfig.RadiusAtEachPowerLevel.Take(4).ToList();
            }

            if (Config.PickaxeConfig.RadiusAtEachPowerLevel.Count > 4)
            {
                Log("Too many values in PickaxeConfig.RadiusAtEachPowerLevel. Additional values will be removed.",
                    LogLevel.Warn);
                Config.PickaxeConfig.RadiusAtEachPowerLevel =
                    Config.PickaxeConfig.RadiusAtEachPowerLevel.Take(4).ToList();
            }
        }

        Helper.WriteConfig(Config);
    }

    #endregion private methods
}