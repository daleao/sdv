namespace DaLion.Stardew.Arsenal;

#region using directives

using StardewModdingAPI;
using StardewValley;
using StardewValley.Tools;

#endregion using directives

internal static class ConsoleCommands
{
    internal static void Register(IModHelper helper)
    {
        helper.ConsoleCommands.Add("weapon_addenchantment",
            "Add the specified enchantment to the player's current weapon." + GetAddEnchantmentUsage(),
            AddEnchantment);
    }

    #region command handlers

    /// <summary>Add the specified enchantment to the player's current tool.</summary>
    /// <param name="command">The console command.</param>
    /// <param name="args">The supplied arguments.</param>
    private static void AddEnchantment(string command, string[] args)
    {
        if (!Context.IsWorldReady)
        {
            Log.W("You must load a save first.");
            return;
        }

        if (Game1.player.CurrentTool is not MeleeWeapon weapon)
        {
            Log.W("You must select a weapon first.");
            return;
        }

        BaseEnchantment enchantment = args[0].ToLower() switch
        {
            "artful" => new ArchaeologistEnchantment(),
            "bugkiller" => new BugKillerEnchantment(),
            "crusader" => new CrusaderEnchantment(),
            "vampiric" => new VampiricEnchantment(),
            "haymaker" => new HaymakerEnchantment(),
            "magic" or "starburst" => new MagicEnchantment(), // not implemented
            _ => null
        };

        if (enchantment is null)
        {
            Log.W($"Unknown weapon enchantment {args[0]}. Please enter a valid enchantment.");
            return;
        }

        if (!enchantment.CanApplyTo(weapon))
        {
            Log.W($"Cannot apply {enchantment.GetDisplayName()} enchantment to {weapon.DisplayName}.");
            return;
        }

        weapon.enchantments.Add(enchantment);
        Log.I($"Applied {enchantment.GetDisplayName()} enchantment to {weapon.DisplayName}.");
    }

    #endregion command handlers

    #region private methods

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