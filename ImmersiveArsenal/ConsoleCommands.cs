using System.Linq;

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
        helper.ConsoleCommands.Add("arsenal_debugquest",
            "Advance the local player to the final stage of Qi's Final Challenge quest.", DebugQuest);
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

    private static void DebugQuest(string command, string[] args)
    {
        if (!Context.IsWorldReady)
        {
            Log.W("You must load a save first.");
            return;
        }

        if (Game1.player.hasOrWillReceiveMail("QiChallengeComplete"))
        {
            if (!args.Any(arg => arg is "--force" or "-f"))
            {
                Log.W("Already completed the Qi Challenge questline. Use parameter '--force', '-f' to forcefully reset.");
                return;
            }
            else
            {
                Game1.player.RemoveMail("QiChallengeComplete");
            }
        }

        if (!Game1.player.hasOrWillReceiveMail("skullCave"))
        {
            Game1.player.mailReceived.Add("skullCave");
            Log.I("Added 'skullCave' to mail received.");
        }

        if (!Game1.player.hasOrWillReceiveMail("QiChallengeFirst"))
        {
            Game1.player.mailReceived.Add("QiChallengeFirst");
            Log.I("Added 'QiChallengeFirst' to mail received.");
        }

        Game1.player.addQuest(ModEntry.QiChallengeFinalQuestId);
        Log.I($"Added Qi's Final Challenge to {Game1.player.Name}'s active quests.");
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