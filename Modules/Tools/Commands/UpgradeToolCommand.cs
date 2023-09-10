namespace DaLion.Overhaul.Modules.Tools.Commands;

#region using directives

using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using DaLion.Overhaul.Modules.Tools.Integrations;
using DaLion.Shared.Commands;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class UpgradeToolCommand : ConsoleCommand
{
    /// <summary>Initializes a new instance of the <see cref="UpgradeToolCommand"/> class.</summary>
    /// <param name="handler">The <see cref="CommandHandler"/> instance that handles this command.</param>
    internal UpgradeToolCommand(CommandHandler handler)
        : base(handler)
    {
    }

    /// <inheritdoc />
    public override string[] Triggers { get; } = { "set_upgrade", "set", "upgrade" };

    /// <inheritdoc />
    public override string Documentation => "Set the upgrade level of the currently held tool." + this.GetUsage();

    /// <inheritdoc />
    [SuppressMessage("StyleCop.CSharp.SpacingRules", "SA1012:Opening braces should be spaced correctly", Justification = "Paradoxical.")]
    public override void Callback(string trigger, string[] args)
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

        if (!UpgradeLevelExtensions.TryParse(args[0], true, out var upgradeLevel))
        {
            Log.W($"Invalid upgrade level {args[0]}." + this.GetUsage());
            return;
        }

        switch (upgradeLevel)
        {
            case UpgradeLevel.Radioactive when !ToolsModule.Config.EnableForgeUpgrading && MoonMisadventuresIntegration.Instance?.IsLoaded == false:
                Log.W("You must enable `ForgeUpgrading` option to set this upgrade level.");
                return;
            case UpgradeLevel.Mythicite when MoonMisadventuresIntegration.Instance?.IsLoaded == false:
                Log.W("You must install `Moon Misadventures` mod to set this upgrade level.");
                return;
            case UpgradeLevel.Enchanted:
                Log.W("To add enchantments use the `ench` entry command instead.");
                return;
            case > UpgradeLevel.Gold when tool is FishingRod:
                Log.W("This tool cannot be upgraded to that level");
                return;
        }

        tool.UpgradeLevel = (int)upgradeLevel;
    }

    /// <summary>Tell the dummies how to use the console command.</summary>
    private string GetUsage()
    {
        var result = new StringBuilder($"\n\nUsage: {this.Handler.EntryCommand} {this.Triggers.FirstOrDefault()} <level>");
        result.Append("\n\nParameters:");
        result.Append("\n\t- <level>: one of 'copper', 'steel', 'gold', 'iridium'");

        if (ToolsModule.Config.EnableForgeUpgrading || MoonMisadventuresIntegration.Instance?.IsLoaded == true)
        {
            result.Append(", 'radioactive'");
        }

        if (MoonMisadventuresIntegration.Instance?.IsLoaded == true)
        {
            result.Append(", 'mythicite'");
        }

        result.Append("\n\nExample:");
        result.Append($"\n\t- {this.Handler.EntryCommand} {this.Triggers[0]} iridium");
        return result.ToString();
    }
}
