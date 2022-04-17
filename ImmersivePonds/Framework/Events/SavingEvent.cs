namespace DaLion.Stardew.Ponds.Framework.Events;

#region using directives

using System.Collections.Generic;
using System.Linq;
using StardewValley.Buildings;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

using Common.Extensions;
using Extensions;

#endregion using directives

internal class SavingEvent : IEvent
{
    /// <inheritdoc />
    public void Hook()
    {
        ModEntry.ModHelper.Events.GameLoop.Saving += OnSaving;
    }

    /// <inheritdoc />
    public void Unhook()
    {
        ModEntry.ModHelper.Events.GameLoop.Saving -= OnSaving;
    }

    /// <summary>Raised before the game writes data to save file.</summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    public void OnSaving(object sender, SavingEventArgs e)
    {
        if (!Context.IsMainPlayer) return;

        var fishQualitiesDict = new Dictionary<int, string>();
        var familyQualitiesDict = new Dictionary<int, string>();
        var familyOccupantsDict = new Dictionary<int, int>();
        var daysEmptyDict = new Dictionary<int, int>();
        var seaweedOccupantsDict = new Dictionary<int, int>();
        var greenAlgaeOccupantsDict = new Dictionary<int, int>();
        var whiteAlgaeOccupantsDict = new Dictionary<int, int>();
        var itemsHeldDict = new Dictionary<int, string>();
        foreach (var pond in Game1.getFarm().buildings.OfType<FishPond>().Where(p => !p.isUnderConstruction()))
        {
            var pondId = pond.GetCenterTile().ToString().GetDeterministicHashCode();
            
            var fishQualities = pond.ReadData("FishQualities");
            if (!string.IsNullOrEmpty(fishQualities)) fishQualitiesDict[pondId] = fishQualities;

            var familyQualities = pond.ReadData("FamilyQualities");
            if (!string.IsNullOrEmpty(familyQualities)) familyQualitiesDict[pondId] = familyQualities;

            var familyLivingHere = pond.ReadDataAs<int>("FamilyLivingHere");
            if (familyLivingHere > 0) familyOccupantsDict[pondId] = familyLivingHere;

            var daysEmpty = pond.ReadDataAs<int>("DaysEmpty");
            if (daysEmpty > 0) daysEmptyDict[pondId] = daysEmpty;
            
            var seaweedLivingHere = pond.ReadDataAs<int>("SeaweedLivingHere");
            if (seaweedLivingHere > 0) seaweedOccupantsDict[pondId] = seaweedLivingHere;
            
            var greenAlgaeLivingHere = pond.ReadDataAs<int>("GreenAlgaeLivingHere");
            if (greenAlgaeLivingHere > 0) greenAlgaeOccupantsDict[pondId] = greenAlgaeLivingHere;
            
            var whiteAlgaeLivingHere = pond.ReadDataAs<int>("WhiteAlgaeLivingHere");
            if (whiteAlgaeLivingHere > 0) whiteAlgaeOccupantsDict[pondId] = whiteAlgaeLivingHere;

            var itemsHeld = pond.ReadData("ItemsHeld");
            if (!string.IsNullOrEmpty(itemsHeld)) itemsHeldDict[pondId] = itemsHeld;
        }

        Game1.player.WriteData(DataField.FishQualitiesDict, fishQualitiesDict.Stringify(">", "/"));
        Game1.player.WriteData(DataField.FamilyQualitiesDict, familyQualitiesDict.Stringify(">", "/"));
        Game1.player.WriteData(DataField.FamilyOccupantsDict, familyOccupantsDict.Stringify());
        Game1.player.WriteData(DataField.DaysEmptyDict, daysEmptyDict.Stringify());
        Game1.player.WriteData(DataField.SeaweedOccupantsDict, seaweedOccupantsDict.Stringify());
        Game1.player.WriteData(DataField.GreenAlgaeOccupantsDict, greenAlgaeOccupantsDict.Stringify());
        Game1.player.WriteData(DataField.WhiteAlgaeOccupantsDict, whiteAlgaeOccupantsDict.Stringify());
        Game1.player.WriteData(DataField.HeldItemsDict, itemsHeldDict.Stringify(">", "/"));
    }
}