namespace DaLion.Stardew.Ponds.Commands;

#region using directives

using System.Linq;
using DaLion.Common.Commands;
using DaLion.Common.Extensions.Collections;
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
    public override void Callback(string[] args)
    {
        if (args.Length > 0)
        {
            Log.W("Additional arguments will be ignored.");
        }

        var ponds = Game1.getFarm().buildings.OfType<FishPond>().Where(p =>
                (p.owner.Value == Game1.player.UniqueMultiplayerID || !Context.IsMultiplayer) &&
                !p.isUnderConstruction())
            .ToHashSet();
        if (ponds.Count <= 0)
        {
            Log.W("You don't own any Fish Ponds.");
            return;
        }

        ponds.ForEach(p => p.UpdateMaximumOccupancy());
    }
}
