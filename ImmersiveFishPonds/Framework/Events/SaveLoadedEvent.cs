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

        var pondQualityDict = Game1.player.ReadData(DataField.QualityDict).ToDictionary<int, int>();
        var familyQualityDict = Game1.player.ReadData(DataField.FamilyQualityDict).ToDictionary<int, int>();
        var familyOccupantsDict = Game1.player.ReadData(DataField.FamilyOccupantsDict).ToDictionary<int, int>();
        var daysEmptyDict = Game1.player.ReadData(DataField.DaysEmptyDict).ToDictionary<int, int>();
        var seaweedOccupantsDict = Game1.player.ReadData(DataField.SeaweedOccupantsDict).ToDictionary<int, int>();
        var greenAlgaeOccupantsDict = Game1.player.ReadData(DataField.GreenAlgaeOccupantsDict).ToDictionary<int, int>();
        var whiteAlgaeOccupantsDict = Game1.player.ReadData(DataField.WhiteAlgaeOccupantsDict).ToDictionary<int, int>();
        var itemsHeldDict = Game1.player.ReadData(DataField.HeldItemsDict).ToDictionary<int, string>(">", "/");

        foreach (var pond in Game1.getFarm().buildings.OfType<FishPond>().Where(p => (p.owner.Value == Game1.player.UniqueMultiplayerID || !Context.IsMultiplayer) && !p.isUnderConstruction()))
        {
            var pondId = pond.GetCenterTile().ToString().GetDeterministicHashCode();
            pond.WriteData("QualityRating",
                pondQualityDict.TryGetValue(pondId, out var qualityRating)
                    ? qualityRating.ToString()
                    : pond.FishCount.ToString());

            if (familyQualityDict.TryGetValue(pondId, out var familyQualityRating))
                pond.WriteData("FamilyQualityRating", familyQualityRating.ToString());

            if (familyOccupantsDict.TryGetValue(pondId, out var familyLivingHere))
                pond.WriteData("FamilyLivingHere", familyLivingHere.ToString());

            if (daysEmptyDict.TryGetValue(pondId, out var daysEmpty))
                pond.WriteData("DaysEmpty", daysEmpty.ToString());

            if (seaweedOccupantsDict.TryGetValue(pondId, out var seaweedLivingHere))
                pond.WriteData("SeaweedLivingHere", seaweedLivingHere.ToString());

            if (greenAlgaeOccupantsDict.TryGetValue(pondId, out var greenAlgaeLivingHere))
                pond.WriteData("GreenAlgaeLivingHere", greenAlgaeLivingHere.ToString());

            if (whiteAlgaeOccupantsDict.TryGetValue(pondId, out var whiteAlgaeLivingHere))
                pond.WriteData("WhiteAlgaeLivingHere", whiteAlgaeLivingHere.ToString());

            if (itemsHeldDict.TryGetValue(pondId, out var itemsHeld))
                pond.WriteData("ItemsHeld", itemsHeld);
        }
    }
}