namespace DaLion.Overhaul.Modules.Tools.Commands;

#region using directives

using System.Diagnostics.CodeAnalysis;
using System.Linq;
using DaLion.Overhaul.Modules.Tools.Integrations;
using DaLion.Shared.Commands;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class UpgradeToolsCommand : ConsoleCommand
{
    /// <summary>Initializes a new instance of the <see cref="UpgradeToolsCommand"/> class.</summary>
    /// <param name="handler">The <see cref="CommandHandler"/> instance that handles this command.</param>
    internal UpgradeToolsCommand(CommandHandler handler)
        : base(handler)
    {
    }

    /// <inheritdoc />
    public override string[] Triggers { get; } = { "upgrade_tools", "set_upgrade", "set", "upgrade" };

    /// <inheritdoc />
    public override string Documentation => "Set the upgrade level of the currently held tool." + this.GetUsage();

    /// <inheritdoc />
    [SuppressMessage("StyleCop.CSharp.SpacingRules", "SA1012:Opening braces should be spaced correctly", Justification = "Paradoxical.")]
    public override void Callback(string[] args)
    {
        if (Game1.player.CurrentTool is not ({ } tool and (Axe or Hoe or Pickaxe or WateringCan or FishingRod)))
        {
            Log.W("You must select a tool first.");
            return;
        }

        if (args.Length < 1)
        {
            Log.W("You must specify a valid upgrade level." + this.GetUsage());
            return;
        }

        if (!Enum.TryParse<UpgradeLevel>(args[0], true, out var upgradeLevel))
        {
            Log.W($"Invalid upgrade level {args[0]}." + this.GetUsage());
            return;
        }

        switch (upgradeLevel)
        {
            case UpgradeLevel.Enchanted:
                Log.W("To add enchantments use the `add_enchantment` command instead.");
                return;
            case > UpgradeLevel.Iridium when MoonMisadventuresIntegration.Instance?.IsLoaded == true:
                Log.W("You must have `Moon Misadventures` mod installed to set this upgrade level.");
                return;
        }

        tool.UpgradeLevel = (int)upgradeLevel;
    }

    /// <summary>Tell the dummies how to use the console command.</summary>
    private string GetUsage()
    {
        var result = $"\n\nUsage: {this.Handler.EntryCommand} {this.Triggers.FirstOrDefault()} <level>";
        result += "\n\nParameters:";
        result += "\n\t- <level>: one of 'copper', 'steel', 'gold', 'iridium'";
        if (MoonMisadventuresIntegration.Instance?.IsLoaded == true)
        {
            result += ", 'radioactive', 'mythicite'";
        }

        result += "\n\nExample:";
        result += $"\n\t- {this.Handler.EntryCommand} {this.Triggers.First()} iridium";
        return result;
    }
}
