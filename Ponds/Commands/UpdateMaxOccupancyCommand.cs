namespace DaLion.Ponds.Commands;

#region using directives

using DaLion.Shared.Commands;
using StardewValley.Buildings;

#endregion using directives

/// <summary>Initializes a new instance of the <see cref="UpdateMaxOccupancyCommand"/> class.</summary>
/// <param name="handler">The <see cref="CommandHandler"/> instance that handles this command.</param>
[UsedImplicitly]
internal sealed class UpdateMaxOccupancyCommand(CommandHandler handler)
    : ConsoleCommand(handler)
{
    /// <inheritdoc />
    public override string[] Triggers { get; } = ["update_pond_occupancy", "update_occupancy", "update", "occupancy"];

    /// <inheritdoc />
    public override string Documentation => "Update the maximum population of all owned fish ponds.";

    /// <inheritdoc />
    public override bool CallbackImpl(string trigger, string[] args)
    {
        if (args.Length > 0)
        {
            Log.W("Additional arguments will be ignored.");
        }

        var count = 0;
        Utility.ForEachBuilding(b =>
        {
            if (b is not FishPond pond)
            {
                return true;
            }

            pond.UpdateMaximumOccupancy();
            count++;
            return true;
        });

        if (count > 0)
        {
            Log.I($"Maximum occupancy updated for {count} Fish Ponds.");
        }
        else
        {
            Log.W("You don't own any Fish Ponds.");
        }

        return true;
    }
}
