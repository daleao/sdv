namespace DaLion.Stardew.Tools.Commands;

#region using directives

using Common;
using Common.Commands;
using Framework;
using LinqFasterer;
using StardewValley.Tools;
using System;

#endregion using directives

[UsedImplicitly]
internal sealed class UpgradeToolsCommand : ConsoleCommand
{
    /// <summary>Construct an instance.</summary>
    /// <param name="handler">The <see cref="CommandHandler"/> instance that handles this command.</param>
    internal UpgradeToolsCommand(CommandHandler handler)
        : base(handler) { }

    /// <inheritdoc />
    public override string[] Triggers { get; } = { "upgrade_tools", "set_upgrade", "set", "upgrade" };

    /// <inheritdoc />
    public override string Documentation => "Set the upgrade level of the currently held tool." + GetUsage();

    /// <inheritdoc />
    public override void Callback(string[] args)
    {
        if (Game1.player.CurrentTool is not ({ } tool and (Axe or Hoe or Pickaxe or WateringCan or FishingRod)))
        {
            Log.W("You must select a tool first.");
            return;
        }

        if (args.Length < 1)
        {
            Log.W("You must specify a valid upgrade level." + GetUsage());
            return;
        }

        if (!Enum.TryParse<Framework.UpgradeLevel>(args[0], true, out var upgradeLevel))
        {
            Log.W($"Invalid upgrade level {args[0]}. Please specify a valid quality." + GetUsage());
            return;
        }

        switch (upgradeLevel)
        {
            case UpgradeLevel.Enchanted:
                Log.W("To add enchantments please use the `add_enchantment` command instead.");
                return;
            case > UpgradeLevel.Iridium when !ModEntry.IsMoonMisadventuresLoaded:
                Log.W("You must have `Moon Misadventures` mod installed to set this upgrade level.");
                return;
        }

        tool.UpgradeLevel = (int)upgradeLevel;
    }

    /// <summary>Tell the dummies how to use the console command.</summary>
    private string GetUsage()
    {
        var result = $"\n\nUsage: {Handler.EntryCommand} {Triggers.FirstOrDefaultF()} <level>";
        result += "\n\nParameters:";
        result += "\n\t- <level>: one of 'copper', 'steel', 'gold', 'iridium'";
        if (ModEntry.IsMoonMisadventuresLoaded)
            result += ", 'radioactive', 'mythicite'";

        result += "\n\nExample:";
        result += $"\n\t- {Handler.EntryCommand} {Triggers.FirstF()} iridium";
        return result;
    }
}