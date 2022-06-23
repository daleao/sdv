namespace DaLion.Stardew.Ponds.Commands;

#region using directives

using System.Linq;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Buildings;

using Common;
using Common.Commands;
using Extensions;

#endregion using directives

internal class ResetPondDataCommand : ICommand
{
    /// <inheritdoc />
    public string Trigger => "reset_data";

    /// <inheritdoc />
    public string Documentation => "Reset custom mod data of nearest pond.";

    /// <inheritdoc />
    public void Callback(string[] args)
    {
        if (!Game1.player.currentLocation.Equals(Game1.getFarm()))
        {
            Log.W("You must be at the farm to do this.");
            return;
        }

        var ponds = Game1.getFarm().buildings.OfType<FishPond>().Where(p =>
                (p.owner.Value == Game1.player.UniqueMultiplayerID || !Context.IsMultiplayer) &&
                !p.isUnderConstruction())
            .ToHashSet();
        if (!ponds.Any())
        {
            Log.W("You don't own any Fish Ponds.");
            return;
        }

        var nearest = Game1.player.GetClosestBuilding(out _, ponds);
        if (nearest is null)
        {
            Log.W("There are no ponds nearby.");
            return;
        }

        nearest.WriteData("FishQualities", null);
        nearest.WriteData("FamilyQualities", null);
        nearest.WriteData("FamilyLivingHere", null);
        nearest.WriteData("DaysEmpty", 0.ToString());
        nearest.WriteData("SeaweedLivingHere", null);
        nearest.WriteData("GreenAlgaeLivingHere", null);
        nearest.WriteData("WhiteAlgaeLivingHere", null);
        nearest.WriteData("CheckedToday", null);
        nearest.WriteData("ItemsHeld", null);
    }
}