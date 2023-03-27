namespace DaLion.Overhaul.Modules.Enchantments.Commands;

#region using directives

using System.Linq;
using System.Text;
using DaLion.Overhaul.Modules.Slingshots.VirtualProperties;
using DaLion.Overhaul.Modules.Weapons.VirtualProperties;
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
    public override void Callback(string trigger, string[] args)
    {
        if (args.Length == 0 || string.IsNullOrEmpty(args[0]))
        {
            Log.W("No enchantment was specified.");
            return;
        }

        var tool = Game1.player.CurrentTool;
        if (tool is not (MeleeWeapon or Slingshot))
        {
            Log.W("You must select a slingshot first.");
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

        switch (tool)
        {
            case MeleeWeapon weapon when WeaponsModule.IsEnabled:
                weapon.Invalidate();
                break;
            case Slingshot slingshot when SlingshotsModule.IsEnabled:
                slingshot.Invalidate();
                break;
        }
    }

    /// <summary>Tell the dummies how to use the console command.</summary>
    private string GetUsage()
    {
        var result = new StringBuilder($"\n\nUsage: {this.Handler.EntryCommand} {this.Triggers[0]} <enchantment>");
        result.Append("\n\nParameters:");
        result.Append("\n\t- <enchantment>: a weapon or slingshot enchantment");
        result.Append("\n\nExample:");
        result.Append($"\n\t- {this.Handler.EntryCommand} {this.Triggers[0]} vampiric");
        return result.ToString();
    }
}
