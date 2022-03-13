namespace DaLion.Stardew.FishPonds.Framework.Events;

#region using directives

using System.Linq;
using StardewValley;
using StardewValley.Buildings;
using StardewModdingAPI;
using StardewModdingAPI.Events;

using Common.Extensions;
using Extensions;

#endregion using directives

internal class SaveLoadedEvent : IEvent
{
    /// <inheritdoc />
    public void Hook()
    {
        ModEntry.ModHelper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
    }

    /// <inheritdoc />
    public void Unhook()
    {
        ModEntry.ModHelper.Events.GameLoop.SaveLoaded -= OnSaveLoaded;
    }

    /// <summary>
    ///     Raised after loading a save (including the first day after creating a new save), or connecting to a
    ///     multiplayer world.
    /// </summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event data.</param>
    public void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
    {
        if (!Context.IsMainPlayer) return;

        var pondQualityDict = Game1.player.ReadData(DataField.FishPondQualityDict).ToDictionary<int, int>(',', ';');
        var familyQualityDict = Game1.player.ReadData(DataField.FishPondFamilyQualityDict).ToDictionary<int, int>(',', ';');
        var familyCountDict = Game1.player.ReadData(DataField.FishPondFamilyCountDict).ToDictionary<int, int>(',', ';');
        var daysEmptyDict = Game1.player.ReadData(DataField.FishPondDaysEmptyDict).ToDictionary<int, int>(',', ';');
        var seaweedCountDict = Game1.player.ReadData(DataField.FishPondSeaweedCountDict).ToDictionary<int, int>(',', ';');
        var greenAlgaeDict = Game1.player.ReadData(DataField.FishPondGreenAlgaeDict).ToDictionary<int, int>(',', ';');
        var whiteAlgaeDict = Game1.player.ReadData(DataField.FishPondWhiteAlgaeDict).ToDictionary<int, int>(',', ';');

        foreach (var pond in Game1.getFarm().buildings.OfType<FishPond>().Where(p => (p.owner.Value == Game1.player.UniqueMultiplayerID || !Context.IsMultiplayer) && !p.isUnderConstruction()))
        {
            var pondId = pond.GetCenterTile().ToString().GetDeterministicHashCode();
            pond.WriteData("QualityRating",
                pondQualityDict.TryGetValue(pondId, out var qualityRating)
                    ? qualityRating.ToString()
                    : pond.FishCount.ToString());

            if (familyQualityDict.TryGetValue(pondId, out var familyQualityRating))
                pond.WriteData("FamilyQualityRating", familyQualityRating.ToString());

            if (familyCountDict.TryGetValue(pondId, out var familyCount))
                pond.WriteData("FamilyCount", familyCount.ToString());

            if (daysEmptyDict.TryGetValue(pondId, out var daysEmpty))
                pond.WriteData("DaysEmpty", daysEmpty.ToString());

            if (seaweedCountDict.TryGetValue(pondId, out var seaweedCount))
                pond.WriteData("SeaweedCount", seaweedCount.ToString());

            if (greenAlgaeDict.TryGetValue(pondId, out var greenAlgaeCount))
                pond.WriteData("GreenAlgaeCount", greenAlgaeCount.ToString());

            if (whiteAlgaeDict.TryGetValue(pondId, out var whiteAlgaeCount))
                pond.WriteData("WhiteAlgaeCount", whiteAlgaeCount.ToString());
        }
    }
}