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
    internal static ModEntry Instance { get; private set; }
    internal static ToolConfig Config { get; set; }

    internal static IModHelper ModHelper => Instance.Helper;
    internal static IManifest Manifest => Instance.ModManifest;
    internal static Action<string, LogLevel> Log => Instance.Monitor.Log;

    internal static bool HasMoonMod { get; private set; }

    internal static PerScreen<Shockwave> Shockwave { get; } = new(() => null);

    /// <summary>The mod entry point, called after the mod is first loaded.</summary>
    /// <param name="helper">Provides simplified APIs for writing mods.</param>
    public override void Entry(IModHelper helper)
    {
        Instance = this;

        // check for Moon Misadventures mod
        HasMoonMod = helper.ModRegistry.IsLoaded("spacechase0.MoonMisadventures");

        // get and verify configs
        Config = Helper.ReadConfig<ToolConfig>();
        VerifyConfigs();

        // hook events
        IEvent.HookAll();

        // apply harmony patches
        new Harmony(ModManifest.UniqueID).PatchAll(Assembly.GetExecutingAssembly());

        // add debug commands
        helper.ConsoleCommands.Register();
    }

    #region private methods

    /// <summary>Check for and fix invalid mod settings.</summary>
    private void VerifyConfigs()
    {
        Log("Verifying tool configs...", LogLevel.Trace);

        if (Config.AxeConfig.RadiusAtEachPowerLevel.Length < 5)
        {
            Log("Missing values in AxeConfig.RadiusAtEachPowerLevel. The default values will be restored.",
                LogLevel.Warn);
            Config.AxeConfig.RadiusAtEachPowerLevel = new[] {1, 2, 3, 4, 5};
            if (HasMoonMod) Config.AxeConfig.RadiusAtEachPowerLevel.AddRangeToArray(new[] {6, 7});
        }
        else if (Config.AxeConfig.RadiusAtEachPowerLevel.Any(i => i < 0))
        {
            Log(
                "Illegal negative value for shockwave radius in AxeConfig.RadiusAtEachPowerLevel. Those values will be replaced with ones.",
                LogLevel.Warn);
            Config.AxeConfig.RadiusAtEachPowerLevel =
                Config.AxeConfig.RadiusAtEachPowerLevel.Select(i => i < 0 ? 0 : i).ToArray();
        }

        if (Config.PickaxeConfig.RadiusAtEachPowerLevel.Length < 5)
        {
            Log("Missing values PickaxeConfig.RadiusAtEachPowerLevel. The default values will be restored.",
                LogLevel.Warn);
            Config.PickaxeConfig.RadiusAtEachPowerLevel = new[] {1, 2, 3, 4, 5};
            if (HasMoonMod) Config.PickaxeConfig.RadiusAtEachPowerLevel.AddRangeToArray(new[] { 6, 7 });
        }
        else if (Config.PickaxeConfig.RadiusAtEachPowerLevel.Any(i => i < 0))
        {
            Log(
                "Illegal negative value for shockwave radius in PickaxeConfig.RadiusAtEachPowerLevel. Those values will be replaced with zero.",
                LogLevel.Warn);
            Config.PickaxeConfig.RadiusAtEachPowerLevel =
                Config.PickaxeConfig.RadiusAtEachPowerLevel.Select(i => i < 0 ? 0 : i).ToArray();
        }

        if (Config.HoeConfig.AffectedTiles.Length < 5 || Config.HoeConfig.AffectedTiles.Any(row => row.Length != 2))
        {
            Log("Incorrect or missing values in HoeConfig.AffectedTiles. The default values will be restored.",
                LogLevel.Warn);
            Config.HoeConfig.AffectedTiles = new[]
                {
                    new[] {3, 0},
                    new[] {5, 0},
                    new[] {3, 1},
                    new[] {6, 1},
                    new[] {5, 2}
                };
            if (HasMoonMod)
                Config.HoeConfig.AffectedTiles.AddRangeToArray(new[]
                {
                    new[] {7, 3},
                    new[] {9, 4}
                });
        }
        else if (Config.HoeConfig.AffectedTiles.Any(row => row.Any(i => i < 0)))
        {
            Log(
                "Illegal negative value for affected tile radius or length in HoeConfig.AffectedTiles. Those values will be replaced with zero.",
                LogLevel.Warn);
            foreach (var row in Config.HoeConfig.AffectedTiles)
                for (var i = 0; i < 2; ++i)
                    if (row[i] < 0) row[i] = 0;
        }

        if (Config.WateringCanConfig.AffectedTiles.Length < 5 || Config.WateringCanConfig.AffectedTiles.Any(row => row.Length != 2))
        {
            Log("Incorrect or missing values in WateringCanConfig.AffectedTiles. The default values will be restored.",
                LogLevel.Warn);
            Config.WateringCanConfig.AffectedTiles = new[]
            {
                new[] {3, 0},
                new[] {5, 0},
                new[] {3, 1},
                new[] {6, 1},
                new[] {5, 2}
            };
            if (HasMoonMod)
                Config.WateringCanConfig.AffectedTiles.AddRangeToArray(new[]
                {
                    new[] {7, 3},
                    new[] {9, 4}
                });
        }
        else if (Config.WateringCanConfig.AffectedTiles.Any(row => row.Any(i => i < 0)))
        {
            Log(
                "Illegal negative value for affected tile radius or length in WateringCanConfig.AffectedTiles. Those values will be replaced with zero.",
                LogLevel.Warn);
            foreach (var row in Config.WateringCanConfig.AffectedTiles)
                for (var i = 0; i < 2; ++i)
                    if (row[i] < 0) row[i] = 0;
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

            switch (Config.AxeConfig.RadiusAtEachPowerLevel.Length)
            {
                case < 7:
                    Log("Adding default radius values for higher Axe upgrades.", LogLevel.Info);
                    Config.AxeConfig.RadiusAtEachPowerLevel =
                        Config.AxeConfig.RadiusAtEachPowerLevel.AddRangeToArray(new[] {6, 7});
                    break;

                case > 7:
                    Log("Too many values in AxeConfig.RadiusAtEachPowerLevel. Additional values will be removed.",
                        LogLevel.Warn);
                    Config.AxeConfig.RadiusAtEachPowerLevel = Config.AxeConfig.RadiusAtEachPowerLevel.Take(7).ToArray();
                    break;
            }

            switch (Config.PickaxeConfig.RadiusAtEachPowerLevel.Length)
            {
                case < 7:
                    Log("Adding default radius values for higher Pickaxe upgrades.", LogLevel.Info);
                    Config.PickaxeConfig.RadiusAtEachPowerLevel =
                        Config.PickaxeConfig.RadiusAtEachPowerLevel.AddRangeToArray(new[] {6, 7});
                    break;

                case > 7:
                    Log("Too many values in PickaxeConfig.RadiusAtEachPowerLevel. Additional values will be removed.",
                        LogLevel.Warn);
                    Config.PickaxeConfig.RadiusAtEachPowerLevel =
                        Config.PickaxeConfig.RadiusAtEachPowerLevel.Take(7).ToArray();
                    break;
            }

            switch (Config.HoeConfig.AffectedTiles.Length)
            {
                case < 7:
                    Log("Adding default length and radius values for higher Hoe upgrades.", LogLevel.Info);
                    Config.HoeConfig.AffectedTiles = Config.HoeConfig.AffectedTiles.AddRangeToArray(new[]
                    {
                        new[] {7, 3},
                        new[] {9, 4}
                    });
                    break;

                case > 7:
                    Log("Too many values in HoeConfig.AffectedTiles. Additional values will be removed.",
                        LogLevel.Warn);
                    Config.HoeConfig.AffectedTiles =
                        Config.HoeConfig.AffectedTiles.Take(7).ToArray();
                    break;
            }

            switch (Config.WateringCanConfig.AffectedTiles.Length)
            {
                case < 7:
                    Log("Adding default length and radius values for higher Watering Can upgrades.", LogLevel.Info);
                    Config.WateringCanConfig.AffectedTiles = Config.WateringCanConfig.AffectedTiles.AddRangeToArray(
                        new[]
                        {
                            new[] {7, 3},
                            new[] {9, 4}
                        });
                    break;

                case > 7:
                    Log("Too many values in WateringCanConfig.AffectedTiles. Additional values will be removed.",
                        LogLevel.Warn);
                    Config.WateringCanConfig.AffectedTiles =
                        Config.WateringCanConfig.AffectedTiles.Take(7).ToArray();
                    break;
            }
        }
        else
        {
            if (Config.AxeConfig.RadiusAtEachPowerLevel.Length > 5)
            {
                Log("Too many values in AxeConfig.RadiusAtEachPowerLevel. Additional values will be removed.",
                    LogLevel.Warn);
                Config.AxeConfig.RadiusAtEachPowerLevel = Config.AxeConfig.RadiusAtEachPowerLevel.Take(5).ToArray();
            }

            if (Config.PickaxeConfig.RadiusAtEachPowerLevel.Length > 5)
            {
                Log("Too many values in PickaxeConfig.RadiusAtEachPowerLevel. Additional values will be removed.",
                    LogLevel.Warn);
                Config.PickaxeConfig.RadiusAtEachPowerLevel =
                    Config.PickaxeConfig.RadiusAtEachPowerLevel.Take(5).ToArray();
            }

            if (Config.HoeConfig.AffectedTiles.Length > 5)
            {
                Log("Too many values in HoeConfig.AffectedTiles. Additional values will be removed.",
                    LogLevel.Warn);
                Config.HoeConfig.AffectedTiles =
                    Config.HoeConfig.AffectedTiles.Take(7).ToArray();
            }

            if (Config.WateringCanConfig.AffectedTiles.Length > 5)
            {
                Log("Too many values in WateringCanConfig.AffectedTiles. Additional values will be removed.",
                    LogLevel.Warn);
                Config.WateringCanConfig.AffectedTiles =
                    Config.WateringCanConfig.AffectedTiles.Take(7).ToArray();
            }
        }

        Helper.WriteConfig(Config);
    }

    #endregion private methods
}