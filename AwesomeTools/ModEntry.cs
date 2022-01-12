using System;
using HarmonyLib;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Tools;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TheLion.Stardew.Tools.Framework.Effects;
using TheLion.Stardew.Tools.Framework.Events;

namespace TheLion.Stardew.Tools;

/// <summary>The mod entry point.</summary>
public class ModEntry : Mod
{
    internal static Configs.ToolConfig Config { get; set; }

    internal static AxeEffect AxeFx { get; private set; }
    internal static PickaxeEffect PickaxeFx { get; private set; }

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

        // get and verify configs
        Config = Helper.ReadConfig<Configs.ToolConfig>();
        VerifyConfigs();

        // instantiate awesome tool effects
        AxeFx = new AxeEffect(Config.AxeConfig, Helper.ModRegistry);
        PickaxeFx = new PickaxeEffect(Config.PickaxeConfig, Helper.ModRegistry);

        // hook events
        new GameLaunchedEvent().Hook();
        new UpdateTickedEvent().Hook();

        // create and patch Harmony instance
        var harmony = new Harmony(ModManifest.UniqueID);
        harmony.PatchAll(Assembly.GetExecutingAssembly());

        // add commands for debugging (or cheating)
        Helper.ConsoleCommands.Add("player_settoolsupgrade", "Set the upgrade level of all upgradeable tools in the player's inventory." + GetCommandUsage(), SetToolsUpgrade);
    }

    /// <summary>Check for and fix invalid mod settings.</summary>
    private void VerifyConfigs()
    {
        if (Config.AxeConfig.RadiusAtEachPowerLevel.Count < 4)
        {
            Log("Missing values in AxeConfig.RadiusAtEachPowerLevel. The default values will be restored.", LogLevel.Warn);
            Config.AxeConfig.RadiusAtEachPowerLevel = new List<int>() { 1, 2, 3, 4 };
        }
        else if (Config.AxeConfig.RadiusAtEachPowerLevel.Any(i => i < 0))
        {
            Log("Illegal negative value for shockwave radius in AxeConfig.RadiusAtEachPowerLevel. Those values will be replaced with zero.", LogLevel.Warn);
            Config.AxeConfig.RadiusAtEachPowerLevel = Config.AxeConfig.RadiusAtEachPowerLevel.Select(x => x < 0 ? 0 : x).ToList();
        }

        if (Config.PickaxeConfig.RadiusAtEachPowerLevel.Count < 4)
        {
            Log("Missing values PickaxeConfig.RadiusAtEachPowerLevel. The default values will be restored.", LogLevel.Warn);
            Config.PickaxeConfig.RadiusAtEachPowerLevel = new List<int>() { 1, 2, 3, 4 };
        }
        else if (Config.PickaxeConfig.RadiusAtEachPowerLevel.Any(i => i < 0))
        {
            Log("Illegal negative value for shockwave radius in PickaxeConfig.RadiusAtEachPowerLevel. Those values will be replaced with zero.", LogLevel.Warn);
            Config.PickaxeConfig.RadiusAtEachPowerLevel = Config.PickaxeConfig.RadiusAtEachPowerLevel.Select(x => x < 0 ? 0 : x).ToList();
        }

        if (Config.RequireModkey && !Config.Modkey.IsBound)
        {
            Log("'RequireModkey' setting is set to true, but no Modkey is bound. Default keybind will be restored. To disable the Modkey, set this value to false.", LogLevel.Warn);
            Config.Modkey = KeybindList.ForSingle(SButton.LeftShift);
        }

        if (Config.StaminaCostMultiplier < 0)
        {
            Log("'StaminaCostMultiplier' is set to a negative value. This may cause game-breaking bugs.", LogLevel.Warn);
        }

        if (Config.ShockwaveDelay > 100)
        {
            Log("The value of 'ShockwaveDelay' is excessively large. This is probably a mistake. The default value will be restored.", LogLevel.Warn);
            Config.ShockwaveDelay = 4;
        }

        if (Framework.Utility.HasHigherLevelToolMod(Helper.ModRegistry))
        {
            Log("Prismatic or Radioactive Tools detected.", LogLevel.Info);

            switch (Config.AxeConfig.RadiusAtEachPowerLevel.Count)
            {
                case < 5:
                    Log("Adding default fifth radius value to Axe configurations.", LogLevel.Info);
                    Config.AxeConfig.RadiusAtEachPowerLevel.Add(5);
                    break;

                case > 5:
                    Log("Too many values in AxeConfig.RadiusAtEachPowerLevel. Additional values will be removed.", LogLevel.Warn);
                    Config.AxeConfig.RadiusAtEachPowerLevel = Config.AxeConfig.RadiusAtEachPowerLevel.Take(5).ToList();
                    break;
            }

            switch (Config.PickaxeConfig.RadiusAtEachPowerLevel.Count)
            {
                case < 5:
                    Log("Adding default fifth radius value to Pickaxe configurations.", LogLevel.Info);
                    Config.PickaxeConfig.RadiusAtEachPowerLevel.Add(5);
                    break;

                case > 5:
                    Log("Too many values in PickaxeConfig.RadiusAtEachPowerLevel. Additional values will be removed.", LogLevel.Warn);
                    Config.PickaxeConfig.RadiusAtEachPowerLevel = Config.PickaxeConfig.RadiusAtEachPowerLevel.Take(5).ToList();
                    break;
            }
        }
        else
        {
            if (Config.AxeConfig.RadiusAtEachPowerLevel.Count > 4)
            {
                Log("Too many values in AxeConfig.RadiusAtEachPowerLevel. Additional values will be removed.", LogLevel.Warn);
                Config.AxeConfig.RadiusAtEachPowerLevel = Config.AxeConfig.RadiusAtEachPowerLevel.Take(4).ToList();
            }

            if (Config.PickaxeConfig.RadiusAtEachPowerLevel.Count > 4)
            {
                Log("Too many values in PickaxeConfig.RadiusAtEachPowerLevel. Additional values will be removed.", LogLevel.Warn);
                Config.PickaxeConfig.RadiusAtEachPowerLevel = Config.PickaxeConfig.RadiusAtEachPowerLevel.Take(4).ToList();
            }
        }

        Helper.WriteConfig(Config);
    }

    /// <summary>Set the upgrade level of all upgradeable tools in the player's inventory.</summary>
    /// <param name="command">The console command.</param>
    /// <param name="args">The supplied arguments.</param>
    private void SetToolsUpgrade(string command, string[] args)
    {
        if (args.Length < 1)
        {
            Log("Missing argument." + GetCommandUsage(), LogLevel.Info);
            return;
        }

        int upgradeLevel = args[0] switch
        {
            "copper" => 1,
            "steel" => 2,
            "gold" => 3,
            "iridium" => 4,
            "prismatic" => 5,
            "radioactive" => 5,
            _ => -1
        };

        if (upgradeLevel < 0)
        {
            if (int.TryParse(args[0], out int i) && i <= 5)
            {
                upgradeLevel = i;
            }
            else
            {
                Log("Invalid argument." + GetCommandUsage(), LogLevel.Info);
                return;
            }
        }

        if (upgradeLevel == 5 && !Framework.Utility.HasHigherLevelToolMod(Helper.ModRegistry))
        {
            Log("You must have either 'Prismatic Tools' or 'Radioactive Tools' installed to set this upgrade level.", LogLevel.Warn);
            return;
        }

        foreach (Item item in Game1.player.Items)
        {
            if (item is Axe or Hoe or Pickaxe or WateringCan)
            {
                (item as Tool).UpgradeLevel = upgradeLevel;
            }
        }
    }

    /// <summary>Tell the dummies how to use the console command.</summary>
    private string GetCommandUsage()
    {
        string result = "\n\nUsage: player_upgradetools < level >\n - level: one of 'copper', 'steel', 'gold', 'iridium'";
        if (Helper.ModRegistry.IsLoaded("stokastic.PrismaticTools"))
        {
            result += ", 'prismatic'";
        }
        else if (Helper.ModRegistry.IsLoaded("kakashigr.RadioactiveTools"))
        {
            result += ", 'radioactive'";
        }

        return result;
    }
}