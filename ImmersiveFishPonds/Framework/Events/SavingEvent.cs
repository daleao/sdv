namespace DaLion.Stardew.FishPonds.Framework.Events;

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

        var pondQualityDict = new Dictionary<int, int>();
        var familyQualityDict = new Dictionary<int, int>();
        var familyCountDict = new Dictionary<int, int>();
        var daysEmptyDict = new Dictionary<int, int>();
        var seaweedCountDict = new Dictionary<int, int>();
        var greenAlgaeCountDict = new Dictionary<int, int>();
        var whiteAlgaeCountDict = new Dictionary<int, int>();
        foreach (var pond in Game1.getFarm().buildings.OfType<FishPond>().Where(p => !p.isUnderConstruction()))
        {
            var qualityRating = pond.ReadDataAs<int>("QualityRating");
            var familyQualityRating = pond.ReadDataAs<int>("FamilyQualityRating");
            var familyCount = pond.ReadDataAs<int>("FamilyCount");
            var daysEmpty = pond.ReadDataAs<int>("DaysEmpty");
            var seaweedCount = pond.ReadDataAs<int>("SeaweedCount");
            var greenAlgaeCount = pond.ReadDataAs<int>("GreenAlgaeCount");
            var whiteAlgaeCount = pond.ReadDataAs<int>("WhiteAlgaeCount");
            var pondId = pond.GetCenterTile().ToString().GetDeterministicHashCode();
            pondQualityDict[pondId] = qualityRating;
            if (familyQualityRating > 0) familyQualityDict[pondId] = familyQualityRating;
            if (familyCount > 0) familyCountDict[pondId] = familyCount;
            if (daysEmpty > 0) daysEmptyDict[pondId] = daysEmpty;
            if (seaweedCount > 0) seaweedCountDict[pondId] = seaweedCount;
            if (greenAlgaeCount > 0) greenAlgaeCountDict[pondId] = greenAlgaeCount;
            if (whiteAlgaeCount > 0) whiteAlgaeCountDict[pondId] = whiteAlgaeCount;
        }

        Game1.player.WriteData(DataField.FishPondQualityDict, pondQualityDict.ToString(',', ';'));
        Game1.player.WriteData(DataField.FishPondFamilyQualityDict, familyQualityDict.ToString(',', ';'));
        Game1.player.WriteData(DataField.FishPondFamilyCountDict, familyCountDict.ToString(',', ';'));
        Game1.player.WriteData(DataField.FishPondDaysEmptyDict, daysEmptyDict.ToString(',', ';'));
        Game1.player.WriteData(DataField.FishPondSeaweedCountDict, seaweedCountDict.ToString(',', ';'));
        Game1.player.WriteData(DataField.FishPondGreenAlgaeDict, greenAlgaeCountDict.ToString(',', ';'));
        Game1.player.WriteData(DataField.FishPondWhiteAlgaeDict, whiteAlgaeCountDict.ToString(',', ';'));
    }
}