using StardewModdingAPI;
using StardewValley;
using StardewValley.Tools;

namespace DaLion.Stardew.Tools
{
    internal static class ConsoleCommands
    {
        internal static void Register(IModHelper helper)
        {
            helper.ConsoleCommands.Add("player_upgradetools",
                "Set the upgrade level of all upgradeable tools in the player's inventory." + GetUpgradeToolsUsage(),
                UpgradeTools);

            helper.ConsoleCommands.Add("tool_addenchantment",
                "Add the specified enchantment to the player's current tool." + GetAddEnchantmentUsage(),
                AddEnchantment);
        }

        #region command handlers

        /// <summary>Set the upgrade level of all upgradeable tools in the player's inventory.</summary>
        /// <param name="command">The console command.</param>
        /// <param name="args">The supplied arguments.</param>
        private static void UpgradeTools(string command, string[] args)
        {
            if (!Context.IsWorldReady)
            {
                ModEntry.Log("You must load a save first.", LogLevel.Warn);
                return;
            }

            if (args.Length < 1)
            {
                ModEntry.Log("Missing argument." + GetUpgradeToolsUsage(), LogLevel.Warn);
                return;
            }

            var upgradeLevel = args[0].ToLower() switch
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
                if (int.TryParse(args[0], out var i) && i <= 5)
                {
                    upgradeLevel = i;
                }
                else
                {
                    ModEntry.Log("Invalid argument." + GetUpgradeToolsUsage(), LogLevel.Warn);
                    return;
                }
            }

            if (upgradeLevel == 5 && !ModEntry.HasToolMod)
            {
                ModEntry.Log("You must have either 'Prismatic Tools' or 'Radioactive Tools' installed to set this upgrade level.",
                    LogLevel.Warn);
                return;
            }

            foreach (var item in Game1.player.Items)
                if (item is Axe or Hoe or Pickaxe or WateringCan)
                    (item as Tool).UpgradeLevel = upgradeLevel;

            ModEntry.Log($"Upgraded all tools to {args[0]}", LogLevel.Info);
        }

        /// <summary>Add the specified enchantment to the player's current tool.</summary>
        /// <param name="command">The console command.</param>
        /// <param name="args">The supplied arguments.</param>
        private static void AddEnchantment(string command, string[] args)
        {
            if (!Context.IsWorldReady)
            {
                ModEntry.Log("You must load a save first.", LogLevel.Warn);
                return;
            }

            var tool = Game1.player.CurrentTool;
            if (tool is null)
            {
                ModEntry.Log("You must select a tool first.", LogLevel.Warn);
                return;
            }

            BaseEnchantment enchantment = args[0].ToLower() switch
            {
                "auto-hook" => new AutoHookEnchantment(),
                "archaeologist" => new ArchaeologistEnchantment(),
                "bottomless" => new BottomlessEnchantment(),
                "efficient" => new EfficientToolEnchantment(),
                "generous" => new GenerousEnchantment(),
                "master" => new MasterEnchantment(),
                "powerful" => new PowerfulEnchantment(),
                "preserving" => new PreservingEnchantment(),
                "reaching" => new ReachingToolEnchantment(),
                "shaving" => new ShavingEnchantment(),
                "swift" => new SwiftToolEnchantment(),
                _ => null
            };

            if (enchantment is null)
            {
                ModEntry.Log($"Unknown enchantment type {args[0]}. Please enter a valid tool enchantment.", LogLevel.Warn);
                return;
            }

            if (!enchantment.CanApplyTo(tool))
            {
                ModEntry.Log($"Cannot apply {enchantment.GetDisplayName()} Enchantment to {tool.DisplayName}.", LogLevel.Warn);
                return;
            }

            tool.enchantments.Add(enchantment);
            ModEntry.Log($"Added {enchantment.GetDisplayName()} Enchantment to {tool.DisplayName}.", LogLevel.Info);
        }

        #endregion command handlers

        #region private methods

        /// <summary>Tell the dummies how to use the console command.</summary>
        private static string GetUpgradeToolsUsage()
        {
            var result = "\n\nUsage: player_upgradetools <level>";
            result += "\n\nParameters:";
            result += "\n\t- <level>: one of 'copper', 'steel', 'gold', 'iridium'";
            if (ModEntry.ModHelper.ModRegistry.IsLoaded("stokastic.PrismaticTools"))
                result += ", 'prismatic'";
            else if (ModEntry.ModHelper.ModRegistry.IsLoaded("kakashigr.RadioactiveTools")) result += ", 'radioactive'";

            result += "\n\nExample:";
            result += "\n\t- player_upgradetools iridium";
            return result;
        }

        /// <summary>Tell the dummies how to use the console command.</summary>
        private static string GetAddEnchantmentUsage()
        {
            var result = "\n\nUsage: tool_addenchantment <enchantment>";
            result += "\n\nParameters:";
            result += "\n\t- <enchantment>: a tool enchantment";
            result += "\n\nExample:";
            result += "\n\t- tool_addenchantment powerful";
            return result;
        }

        #endregion private methods
    }
}
