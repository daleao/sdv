namespace DaLion.Stardew.Professions.Commands;

#region using directives

using Common;
using Common.Commands;
using Common.Extensions.Collections;
using JetBrains.Annotations;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Buildings;
using System.Linq;

#endregion using directives

[UsedImplicitly]
internal sealed class UpdateMaxOccupancyCommand : ConsoleCommand
{
    /// <summary>Construct an instance.</summary>
    /// <param name="handler">The <see cref="CommandHandler"/> instance that handles this command.</param>
    internal UpdateMaxOccupancyCommand(CommandHandler handler)
        : base(handler) { }

    /// <inheritdoc />
    public override string[] Triggers { get; } = { "update_pond_occupancy", "update_occupancy", "update", "occupancy" };

    /// <inheritdoc />
    public override string Documentation => "Update the maximum population of all owned fish ponds.";

    /// <inheritdoc />
    public override void Callback(string[] args)
    {
        if (args.Length > 0)
            Log.W("Additional arguments will be ignored.");

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