namespace DaLion.Overhaul.Modules.Ponds.Commands;

#region using directives

using DaLion.Shared.Commands;
using DaLion.Shared.Extensions.Stardew;
using StardewValley.Buildings;

#endregion using directives

[UsedImplicitly]
internal sealed class ResetPondDataCommand : ConsoleCommand
{
    /// <summary>Initializes a new instance of the <see cref="ResetPondDataCommand"/> class.</summary>
    /// <param name="handler">The <see cref="CommandHandler"/> instance that handles this command.</param>
    internal ResetPondDataCommand(CommandHandler handler)
        : base(handler)
    {
    }

    /// <inheritdoc />
    public override string[] Triggers { get; } = { "reset_data", "clear_data", "reset", "clear" };

    /// <inheritdoc />
    public override string Documentation => "Reset custom mod data of nearest pond.";

    /// <inheritdoc />
    public override void Callback(string trigger, string[] args)
    {
        if (!Game1.player.currentLocation.Equals(Game1.getFarm()))
        {
            Log.W("You must be at the farm to do this.");
            return;
        }

        var nearest = Game1.player.GetClosestBuilding<FishPond>(out _, predicate: b =>
            b.IsOwnedBy(Game1.player) && !b.isUnderConstruction());
        if (nearest is null)
        {
            Log.W("There are no owned ponds nearby.");
            return;
        }

        nearest.Write(DataKeys.FishQualities, null);
        nearest.Write(DataKeys.FamilyQualities, null);
        nearest.Write(DataKeys.FamilyLivingHere, null);
        nearest.Write(DataKeys.DaysEmpty, 0.ToString());
        nearest.Write(DataKeys.SeaweedLivingHere, null);
        nearest.Write(DataKeys.GreenAlgaeLivingHere, null);
        nearest.Write(DataKeys.WhiteAlgaeLivingHere, null);
        nearest.Write(DataKeys.CheckedToday, null);
        nearest.Write(DataKeys.ItemsHeld, null);
        nearest.Write(DataKeys.MetalsHeld, null);
        Log.I($"The mod data for nearby {nearest.GetFishObject().Name} pond has been reset.");
    }
}
