namespace DaLion.Stardew.Tools;

#region using directives

using Common;
using Common.Commands;
using Common.Events;
using Common.Harmony;
using Configs;
using Framework.Effects;
using HarmonyLib;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using System.Linq;

#endregion using directives

/// <summary>The mod entry point.</summary>
public class ModEntry : Mod
{
    internal static ModEntry Instance { get; private set; } = null!;
    internal static ToolConfig Config { get; set; } = null!;

    internal static IModHelper ModHelper => Instance.Helper;
    internal static IManifest Manifest => Instance.ModManifest;

    internal static PerScreen<Shockwave?> Shockwave { get; } = new(() => null);

    internal static bool HasLoadedMoonMisadventures { get; private set; }

    /// <summary>The mod entry point, called after the mod is first loaded.</summary>
    /// <param name="helper">Provides simplified APIs for writing mods.</param>
    public override void Entry(IModHelper helper)
    {
        Instance = this;

        // initialize logger
        Log.Init(Monitor);

        // check for Moon Misadventures before verifying configs
        HasLoadedMoonMisadventures = helper.ModRegistry.IsLoaded("spacechase0.MoonMisadventures");
        
        // get and verify configs
        Config = Helper.ReadConfig<ToolConfig>();
        VerifyConfigs();

        // hook events
        new EventManager(helper.Events).HookAll();

        // apply patches
        new Harmonizer(ModManifest.UniqueID).ApplyAll();

        // register commands
        new CommandHandler(helper.ConsoleCommands).Register("itools", "Power Tools");
    }

    #region private methods

    /// <summary>Check for and fix invalid mod settings.</summary>
    private void VerifyConfigs()
    {
        Log.T("Verifying tool configs...");

        if (Config.AxeConfig.RadiusAtEachPowerLevel.Length < 5)
        {
            Log.W("Missing values in AxeConfig.RadiusAtEachPowerLevel. The default values will be restored.");
            Config.AxeConfig.RadiusAtEachPowerLevel = new[] { 1, 2, 3, 4, 5 };
            if (HasLoadedMoonMisadventures) Config.AxeConfig.RadiusAtEachPowerLevel.AddRangeToArray(new[] { 6, 7 });
        }
        else if (Config.AxeConfig.RadiusAtEachPowerLevel.Any(i => i < 0))
        {
            Log.W(
                "Illegal negative value for shockwave radius in AxeConfig.RadiusAtEachPowerLevel. Those values will be replaced with ones.");
            Config.AxeConfig.RadiusAtEachPowerLevel =
                Config.AxeConfig.RadiusAtEachPowerLevel.Select(i => i < 0 ? 0 : i).ToArray();
        }

        if (Config.PickaxeConfig.RadiusAtEachPowerLevel.Length < 5)
        {
            Log.W("Missing values PickaxeConfig.RadiusAtEachPowerLevel. The default values will be restored.");
            Config.PickaxeConfig.RadiusAtEachPowerLevel = new[] { 1, 2, 3, 4, 5 };
            if (HasLoadedMoonMisadventures) Config.PickaxeConfig.RadiusAtEachPowerLevel.AddRangeToArray(new[] { 6, 7 });
        }
        else if (Config.PickaxeConfig.RadiusAtEachPowerLevel.Any(i => i < 0))
        {
            Log.W(
                "Illegal negative value for shockwave radius in PickaxeConfig.RadiusAtEachPowerLevel. Those values will be replaced with zero.");
            Config.PickaxeConfig.RadiusAtEachPowerLevel =
                Config.PickaxeConfig.RadiusAtEachPowerLevel.Select(i => i < 0 ? 0 : i).ToArray();
        }

        if (Config.HoeConfig.AffectedTiles.Length < 5 || Config.HoeConfig.AffectedTiles.Any(row => row.Length != 2))
        {
            Log.W("Incorrect or missing values in HoeConfig.AffectedTiles. The default values will be restored.");
            Config.HoeConfig.AffectedTiles = new[]
                {
                    new[] {3, 0},
                    new[] {5, 0},
                    new[] {3, 1},
                    new[] {6, 1},
                    new[] {5, 2}
                };
            if (HasLoadedMoonMisadventures)
                Config.HoeConfig.AffectedTiles.AddRangeToArray(new[]
                {
                    new[] {7, 3},
                    new[] {9, 4}
                });
        }
        else if (Config.HoeConfig.AffectedTiles.Any(row => row.Any(i => i < 0)))
        {
            Log.W(
                "Illegal negative value for affected tile radius or length in HoeConfig.AffectedTiles. Those values will be replaced with zero.");
            foreach (var row in Config.HoeConfig.AffectedTiles)
                for (var i = 0; i < 2; ++i)
                    if (row[i] < 0) row[i] = 0;
        }

        if (Config.WateringCanConfig.AffectedTiles.Length < 5 || Config.WateringCanConfig.AffectedTiles.Any(row => row.Length != 2))
        {
            Log.W("Incorrect or missing values in WateringCanConfig.AffectedTiles. The default values will be restored.");
            Config.WateringCanConfig.AffectedTiles = new[]
            {
                new[] {3, 0},
                new[] {5, 0},
                new[] {3, 1},
                new[] {6, 1},
                new[] {5, 2}
            };
            if (HasLoadedMoonMisadventures)
                Config.WateringCanConfig.AffectedTiles.AddRangeToArray(new[]
                {
                    new[] {7, 3},
                    new[] {9, 4}
                });
        }
        else if (Config.WateringCanConfig.AffectedTiles.Any(row => row.Any(i => i < 0)))
        {
            Log.W(
                "Illegal negative value for affected tile radius or length in WateringCanConfig.AffectedTiles. Those values will be replaced with zero.");
            foreach (var row in Config.WateringCanConfig.AffectedTiles)
                for (var i = 0; i < 2; ++i)
                    if (row[i] < 0) row[i] = 0;
        }

        if (Config.RequireModkey && !Config.Modkey.IsBound)
        {
            Log.W(
                "'RequireModkey' setting is set to true, but no Modkey is bound. Default keybind will be restored. To disable the Modkey, set this value to false.");
            Config.Modkey = KeybindList.ForSingle(SButton.LeftShift);
        }

        if (Config.StaminaCostMultiplier < 0)
            Log.W("'StaminaCostMultiplier' is set to a negative value. This may cause game-breaking bugs.");

        if (Config.TicksBetweenWaves > 100)
        {
            Log.W(
                "The value of 'TicksBetweenWaves' is excessively large. This is probably a mistake. The default value will be restored.");
            Config.TicksBetweenWaves = 4;
        }

        if (HasLoadedMoonMisadventures)
        {
            Log.I("Moon Misadventures detected.");

            switch (Config.AxeConfig.RadiusAtEachPowerLevel.Length)
            {
                case < 7:
                    Log.I("Adding default radius values for higher Axe upgrades.");
                    Config.AxeConfig.RadiusAtEachPowerLevel =
                        Config.AxeConfig.RadiusAtEachPowerLevel.AddRangeToArray(new[] { 6, 7 });
                    break;

                case > 7:
                    Log.W("Too many values in AxeConfig.RadiusAtEachPowerLevel. Additional values will be removed.");
                    Config.AxeConfig.RadiusAtEachPowerLevel = Config.AxeConfig.RadiusAtEachPowerLevel.Take(7).ToArray();
                    break;
            }

            switch (Config.PickaxeConfig.RadiusAtEachPowerLevel.Length)
            {
                case < 7:
                    Log.I("Adding default radius values for higher Pickaxe upgrades.");
                    Config.PickaxeConfig.RadiusAtEachPowerLevel =
                        Config.PickaxeConfig.RadiusAtEachPowerLevel.AddRangeToArray(new[] { 6, 7 });
                    break;

                case > 7:
                    Log.W("Too many values in PickaxeConfig.RadiusAtEachPowerLevel. Additional values will be removed.");
                    Config.PickaxeConfig.RadiusAtEachPowerLevel =
                        Config.PickaxeConfig.RadiusAtEachPowerLevel.Take(7).ToArray();
                    break;
            }

            switch (Config.HoeConfig.AffectedTiles.Length)
            {
                case < 7:
                    Log.I("Adding default length and radius values for higher Hoe upgrades.");
                    Config.HoeConfig.AffectedTiles = Config.HoeConfig.AffectedTiles.AddRangeToArray(new[]
                    {
                        new[] {7, 3},
                        new[] {9, 4}
                    });
                    break;

                case > 7:
                    Log.W("Too many values in HoeConfig.AffectedTiles. Additional values will be removed.");
                    Config.HoeConfig.AffectedTiles =
                        Config.HoeConfig.AffectedTiles.Take(7).ToArray();
                    break;
            }

            switch (Config.WateringCanConfig.AffectedTiles.Length)
            {
                case < 7:
                    Log.I("Adding default length and radius values for higher Watering Can upgrades.");
                    Config.WateringCanConfig.AffectedTiles = Config.WateringCanConfig.AffectedTiles.AddRangeToArray(
                        new[]
                        {
                            new[] {7, 3},
                            new[] {9, 4}
                        });
                    break;

                case > 7:
                    Log.W("Too many values in WateringCanConfig.AffectedTiles. Additional values will be removed.");
                    Config.WateringCanConfig.AffectedTiles =
                        Config.WateringCanConfig.AffectedTiles.Take(7).ToArray();
                    break;
            }
        }
        else
        {
            if (Config.AxeConfig.RadiusAtEachPowerLevel.Length > 5)
            {
                Log.W("Too many values in AxeConfig.RadiusAtEachPowerLevel. Additional values will be removed.");
                Config.AxeConfig.RadiusAtEachPowerLevel = Config.AxeConfig.RadiusAtEachPowerLevel.Take(5).ToArray();
            }

            if (Config.PickaxeConfig.RadiusAtEachPowerLevel.Length > 5)
            {
                Log.W("Too many values in PickaxeConfig.RadiusAtEachPowerLevel. Additional values will be removed.");
                Config.PickaxeConfig.RadiusAtEachPowerLevel =
                    Config.PickaxeConfig.RadiusAtEachPowerLevel.Take(5).ToArray();
            }

            if (Config.HoeConfig.AffectedTiles.Length > 5)
            {
                Log.W("Too many values in HoeConfig.AffectedTiles. Additional values will be removed.");
                Config.HoeConfig.AffectedTiles =
                    Config.HoeConfig.AffectedTiles.Take(7).ToArray();
            }

            if (Config.WateringCanConfig.AffectedTiles.Length > 5)
            {
                Log.W("Too many values in WateringCanConfig.AffectedTiles. Additional values will be removed.");
                Config.WateringCanConfig.AffectedTiles =
                    Config.WateringCanConfig.AffectedTiles.Take(7).ToArray();
            }
        }

        Helper.WriteConfig(Config);
    }

    #endregion private methods
}