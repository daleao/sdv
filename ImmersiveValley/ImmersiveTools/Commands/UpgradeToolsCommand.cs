namespace DaLion.Stardew.Tools.Commands;

#region using directives

using Common;
using Common.Commands;
using StardewValley.Tools;
using System;
using System.Linq;

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
    public override string Documentation =>
        "Set the upgrade level of all upgradeable tools in the inventory." + GetUsage();

    /// <inheritdoc />
    public override void Callback(string[] args)
    {
        if (Game1.player.CurrentTool is not MeleeWeapon weapon)
        {
            Log.W("You must select a weapon first.");
            return;
        }

        if (args.Length < 1)
        {
            Log.W("You must specify a valid quality." + GetUsage());
            return;
        }

        if (!Enum.TryParse<Framework.UpgradeLevel>(args[0], true, out var upgradeLevel))
        {
            Log.W($"Invalid quality {args[0]}. Please specify a valid quality." + GetUsage());
            return;
        }

        if (upgradeLevel > Framework.UpgradeLevel.Iridium && !ModEntry.IsMoonMisadventuresLoaded)
        {
            Log.W("You must have `Moon Misadventures` mod installed to set this upgrade level.");
            return;
        }

        foreach (var item in Game1.player.Items)
            if (item is Axe or Hoe or Pickaxe or WateringCan)
                (item as Tool)!.UpgradeLevel = (int)upgradeLevel;
    }

    /// <summary>Tell the dummies how to use the console command.</summary>
    private string GetUsage()
    {
        var result = $"\n\nUsage: {Handler.EntryCommand} {Triggers.First()} <level>";
        result += "\n\nParameters:";
        result += "\n\t- <level>: one of 'copper', 'steel', 'gold', 'iridium'";
        if (ModEntry.IsMoonMisadventuresLoaded)
            result += ", 'radioactive', 'mythicite'";

        result += "\n\nExample:";
        result += $"\n\t- {Handler.EntryCommand} {Triggers.First()} iridium";
        return result;
    }
}