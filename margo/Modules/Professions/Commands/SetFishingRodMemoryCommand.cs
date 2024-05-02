namespace DaLion.Overhaul.Modules.Professions.Commands;

#region using directives

using DaLion.Shared.Commands;
using DaLion.Shared.Extensions.Stardew;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class SetFishingRodMemoryCommand : ConsoleCommand
{
    /// <summary>Initializes a new instance of the <see cref="SetFishingRodMemoryCommand"/> class.</summary>
    /// <param name="handler">The <see cref="CommandHandler"/> instance that handles this command.</param>
    internal SetFishingRodMemoryCommand(CommandHandler handler)
        : base(handler)
    {
    }

    /// <inheritdoc />
    public override string[] Triggers { get; } = { "set_memory" };

    /// <inheritdoc />
    public override string Documentation => "Set a value for the fishing rod's memory.";

    /// <inheritdoc />
    public override void Callback(string trigger, string[] args)
    {
        if (args.Length == 0 || string.IsNullOrEmpty(args[0]))
        {
            Log.W("You must specify a tackle index.");
            return;
        }

        if (Game1.player.CurrentTool is not FishingRod { UpgradeLevel: > 2 } rod)
        {
            Log.W("You must equip an Iridium Rod to use this command.");
            return;
        }

        if (!int.TryParse(args[0], out var index) ||
            index is not (686 or 687 or 691 or 692 or 693 or 694 or 695 or 856 or 877))
        {
            Log.W("You must specify a valid tackle index.");
            return;
        }

        rod.Write(DataKeys.LastTackleUsed, index.ToString());
        rod.Write(DataKeys.LastTackleUses, FishingRod.maxTackleUses.ToString());
    }
}
