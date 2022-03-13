namespace DaLion.Stardew.Tweaks;

#region using directives

using StardewModdingAPI;
using StardewValley;

#endregion using directives

internal static class ConsoleCommands
{
    internal static void Register(IModHelper helper)
    {
        if (!ModEntry.ModHelper.ModRegistry.IsLoaded("DaLion.ImmersiveTools"))
            helper.ConsoleCommands.Add("tool_addenchantment",
                "Add the specified enchantment to the player's current tool." + GetAddEnchantmentUsage(),
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

        var tool = Game1.player.CurrentTool;
        if (tool is null)
        {
            Log.W("You must select a tool first.");
            return;
        }

        BaseEnchantment enchantment = args[0].ToLower() switch
        {
            // weapon enchants
            "artful" => new ArchaeologistEnchantment(),
            "bugkiller" => new BugKillerEnchantment(),
            "crusader" => new CrusaderEnchantment(),
            "vampiric" => new VampiricEnchantment(),
            "haymaker" => new HaymakerEnchantment(),
            "magic" or "starburst" => new MagicEnchantment(), // not implemented
            // tool enchants
            "auto-hook" or "autohook" => new AutoHookEnchantment(),
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
            Log.W($"Unknown enchantment type {args[0]}. Please enter a valid enchantment.");
            return;
        }

        if (!enchantment.CanApplyTo(tool))
        {
            Log.W($"Cannot apply {enchantment.GetDisplayName()} enchantment to {tool.DisplayName}.");
            return;
        }

        tool.enchantments.Add(enchantment);
        Log.I($"Applied {enchantment.GetDisplayName()} enchantment to {tool.DisplayName}.");
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