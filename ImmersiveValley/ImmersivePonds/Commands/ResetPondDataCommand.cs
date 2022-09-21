namespace DaLion.Stardew.Ponds.Commands;

#region using directives

using DaLion.Common;
using DaLion.Common.Commands;
using DaLion.Common.Extensions.Stardew;
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
    public override void Callback(string[] args)
    {
        if (!Game1.player.currentLocation.Equals(Game1.getFarm()))
        {
            Log.W("You must be at the farm to do this.");
            return;
        }

        var nearest = Game1.player.GetClosestBuilding<FishPond>(predicate: b =>
            (b.owner.Value == Game1.player.UniqueMultiplayerID || !Context.IsMultiplayer) && !b.isUnderConstruction());
        if (nearest is null)
        {
            Log.W("There are no owned ponds nearby.");
            return;
        }

        nearest.Write("FishQualities", null);
        nearest.Write("FamilyQualities", null);
        nearest.Write("FamilyLivingHere", null);
        nearest.Write("DaysEmpty", 0.ToString());
        nearest.Write("SeaweedLivingHere", null);
        nearest.Write("GreenAlgaeLivingHere", null);
        nearest.Write("WhiteAlgaeLivingHere", null);
        nearest.Write("CheckedToday", null);
        nearest.Write("ItemsHeld", null);
    }
}
