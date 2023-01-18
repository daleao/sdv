namespace DaLion.Overhaul.Modules.Ponds.Commands;

#region using directives

using DaLion.Shared.Commands;
using DaLion.Shared.Extensions.Stardew;
using StardewValley.Buildings;

#endregion using directives

[UsedImplicitly]
internal sealed class UpdateMaxOccupancyCommand : ConsoleCommand
{
    /// <summary>Initializes a new instance of the <see cref="UpdateMaxOccupancyCommand"/> class.</summary>
    /// <param name="handler">The <see cref="CommandHandler"/> instance that handles this command.</param>
    internal UpdateMaxOccupancyCommand(CommandHandler handler)
        : base(handler)
    {
    }

    /// <inheritdoc />
    public override string[] Triggers { get; } = { "update_pond_occupancy", "update_occupancy", "update", "occupancy" };

    /// <inheritdoc />
    public override string Documentation => "Update the maximum population of all owned fish ponds.";

    /// <inheritdoc />
    public override void Callback(string trigger, string[] args)
    {
        if (args.Length > 0)
        {
            Log.W("Additional arguments will be ignored.");
        }

        var count = 0;
        foreach (var building in Game1.getFarm().buildings)
        {
            if (building is not FishPond pond || !pond.IsOwnedBy(Game1.player) ||
                pond.isUnderConstruction())
            {
                continue;
            }

            pond.UpdateMaximumOccupancy();
            count++;
        }

        if (count > 0)
        {
            Log.I($"Maximum occupancy updated for {count} Fish Ponds.");
        }
        else
        {
            Log.W("You don't own any Fish Ponds.");
        }
    }
}
