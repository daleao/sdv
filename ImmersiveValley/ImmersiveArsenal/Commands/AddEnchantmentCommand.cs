namespace DaLion.Stardew.Arsenal.Commands;

#region using directives

using System.Linq;

using StardewValley;
using StardewValley.Tools;

using Common;
using Common.Commands;

#endregion using directives

internal class AddEnchantmentCommand : ICommand
{
    /// <inheritdoc />
    public string Trigger => "add_enchants";

    /// <inheritdoc />
    public string Documentation => "Add the specified enchantment to the selected weapon." + GetUsage();

    /// <inheritdoc />
    public void Callback(string[] args)
    {
        if (Game1.player.CurrentTool is not MeleeWeapon weapon)
        {
            Log.W("You must select a weapon first.");
            return;
        }

        while (args.Any())
        {
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
                Log.W($"Ignoring unknown enchantment {args[0]}.");
                return;
            }

            if (!enchantment.CanApplyTo(weapon))
            {
                Log.W($"Cannot apply {enchantment.GetDisplayName()} enchantment to {weapon.DisplayName}.");
                return;
            }

            weapon.enchantments.Add(enchantment);
            Log.I($"Applied {enchantment.GetDisplayName()} enchantment to {weapon.DisplayName}.");

            args = args.Skip(1).ToArray();
        }
    }

    /// <summary>Tell the dummies how to use the console command.</summary>
    private string GetUsage()
    {
        var result = "\n\nUsage: tool_addenchantment <enchantment>";
        result += "\n\nParameters:";
        result += "\n\t- <enchantment>: a tool enchantment";
        result += "\n\nExample:";
        result += "\n\t- tool_addenchantment powerful";
        return result;
    }
}