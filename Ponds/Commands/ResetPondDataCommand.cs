namespace DaLion.Ponds.Commands;

#region using directives

using DaLion.Shared.Commands;
using DaLion.Shared.Extensions.Stardew;
using StardewValley.Buildings;

#endregion using directives

/// <summary>Initializes a new instance of the <see cref="ResetPondDataCommand"/> class.</summary>
/// <param name="handler">The <see cref="CommandHandler"/> instance that handles this command.</param>
[UsedImplicitly]
internal sealed class ResetPondDataCommand(CommandHandler handler)
    : ConsoleCommand(handler)
{
    /// <inheritdoc />
    public override string[] Triggers { get; } = ["reset_data", "clear_data", "reset", "clear"];

    /// <inheritdoc />
    public override string Documentation => "Reset custom mod data of nearest pond.";

    /// <inheritdoc />
    public override bool CallbackImpl(string trigger, string[] args)
    {
        if (!Game1.player.currentLocation.Equals(Game1.getFarm()))
        {
            Log.W("You must be at the farm to do this.");
            return false;
        }

        var nearest = Game1.player.GetClosestBuilding<FishPond>(predicate: b =>
            b.IsOwnedBy(Game1.player) && !b.isUnderConstruction());
        if (nearest is null)
        {
            Log.W("There are no owned ponds nearby.");
            return true;
        }

        Data.Write(nearest, DataKeys.PondFish, null);
        Data.Write(nearest, DataKeys.DaysEmpty, 0.ToString());
        Data.Write(nearest, DataKeys.CheckedToday, null);
        Data.Write(nearest, DataKeys.ItemsHeld, null);
        Data.Write(nearest, DataKeys.MetalsHeld, null);
        var label = string.IsNullOrEmpty(nearest.fishType.Value) ? "empty" : nearest.GetFishObject().Name;
        Log.I($"The mod data for nearby {label} pond has been reset.");
        return true;
    }
}
