namespace DaLion.Overhaul.Modules.Tools.Commands;

#region using directives

using System.Diagnostics.CodeAnalysis;
using System.Linq;
using DaLion.Shared.Commands;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class RemoveEnchantmentsCommand : ConsoleCommand
{
    /// <summary>Initializes a new instance of the <see cref="RemoveEnchantmentsCommand"/> class.</summary>
    /// <param name="handler">The <see cref="CommandHandler"/> instance that handles this command.</param>
    internal RemoveEnchantmentsCommand(CommandHandler handler)
        : base(handler)
    {
    }

    /// <inheritdoc />
    public override string[] Triggers { get; } = { "remove_enchants", "remove", "disenchant" };

    /// <inheritdoc />
    public override string Documentation =>
        "Remove the specified enchantments from the selected weapon or slingshot." + this.GetUsage();

    /// <inheritdoc />
    [SuppressMessage("StyleCop.CSharp.SpacingRules", "SA1012:Opening braces should be spaced correctly", Justification = "Paradoxical.")]
    public override void Callback(string trigger, string[] args)
    {
        if (args.Length == 0 || string.IsNullOrEmpty(args[0]))
        {
            Log.W("No enchantment was specified.");
            return;
        }

        if (Game1.player.CurrentTool is not ({ } tool and (Axe or Hoe or Pickaxe or WateringCan or FishingRod or MeleeWeapon)) ||
            (Game1.player.CurrentTool is MeleeWeapon weapon && !weapon.isScythe()))
        {
            Log.W("You must select a tool first.");
            return;
        }

        while (args.Length > 0)
        {
            var enchantment = tool.enchantments.FirstOrDefault(e =>
                e.GetType().Name.ToLowerInvariant().Contains(args[0].ToLowerInvariant()));

            if (enchantment is null)
            {
                Log.W($"The {tool.DisplayName} does not have a {args[0]} enchantment.");
                args = args.Skip(1).ToArray();
                continue;
            }

            tool.RemoveEnchantment(enchantment);
            Log.I($"Removed {enchantment.GetDisplayName()} enchantment from {tool.DisplayName}.");

            args = args.Skip(1).ToArray();
        }
    }

    /// <summary>Tell the dummies how to use the console command.</summary>
    private string GetUsage()
    {
        var result = $"\n\nUsage: {this.Handler.EntryCommand} {this.Triggers.First()} <enchantment>";
        result += "\n\nParameters:";
        result += "\n\t- <enchantment>: a tool enchantment";
        result += "\n\nExample:";
        result += $"\n\t- {this.Handler.EntryCommand} {this.Triggers.First()} vampiric";
        return result;
    }
}
