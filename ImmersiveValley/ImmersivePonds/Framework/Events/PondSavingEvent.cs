namespace DaLion.Stardew.Ponds.Framework.Events;

#region using directives

using Common.Data;
using Common.Events;
using Common.Extensions;
using Common.Extensions.Collections;
using JetBrains.Annotations;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Buildings;
using System.Collections.Generic;
using System.Linq;

#endregion using directives

[UsedImplicitly]
internal sealed class PondSavingEvent : SavingEvent
{
    /// <summary>Construct an instance.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal PondSavingEvent(EventManager manager)
        : base(manager) { }

    /// <inheritdoc />
    protected override void OnSavingImpl(object? sender, SavingEventArgs e)
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

            var fishQualities = ModDataIO.ReadData(pond, "FishQualities");
            if (!string.IsNullOrEmpty(fishQualities)) fishQualitiesDict[pondId] = fishQualities;

            var familyQualities = ModDataIO.ReadData(pond, "FamilyQualities");
            if (!string.IsNullOrEmpty(familyQualities)) familyQualitiesDict[pondId] = familyQualities;

            var familyLivingHere = ModDataIO.ReadDataAs<int>(pond, "FamilyLivingHere");
            if (familyLivingHere > 0) familyOccupantsDict[pondId] = familyLivingHere;

            var daysEmpty = ModDataIO.ReadDataAs<int>(pond, "DaysEmpty");
            if (daysEmpty > 0) daysEmptyDict[pondId] = daysEmpty;

            var seaweedLivingHere = ModDataIO.ReadDataAs<int>(pond, "SeaweedLivingHere");
            if (seaweedLivingHere > 0) seaweedOccupantsDict[pondId] = seaweedLivingHere;

            var greenAlgaeLivingHere = ModDataIO.ReadDataAs<int>(pond, "GreenAlgaeLivingHere");
            if (greenAlgaeLivingHere > 0) greenAlgaeOccupantsDict[pondId] = greenAlgaeLivingHere;

            var whiteAlgaeLivingHere = ModDataIO.ReadDataAs<int>(pond, "WhiteAlgaeLivingHere");
            if (whiteAlgaeLivingHere > 0) whiteAlgaeOccupantsDict[pondId] = whiteAlgaeLivingHere;

            var itemsHeld = ModDataIO.ReadData(pond, "ItemsHeld");
            if (!string.IsNullOrEmpty(itemsHeld)) itemsHeldDict[pondId] = itemsHeld;
        }

        ModDataIO.WriteData(Game1.player, ModData.FishQualitiesDict.ToString(), fishQualitiesDict.Stringify(">", "/"));
        ModDataIO.WriteData(Game1.player, ModData.FamilyQualitiesDict.ToString(),
            familyQualitiesDict.Stringify(">", "/"));
        ModDataIO.WriteData(Game1.player, ModData.FamilyOccupantsDict.ToString(), familyOccupantsDict.Stringify());
        ModDataIO.WriteData(Game1.player, ModData.DaysEmptyDict.ToString(), daysEmptyDict.Stringify());
        ModDataIO.WriteData(Game1.player, ModData.SeaweedOccupantsDict.ToString(), seaweedOccupantsDict.Stringify());
        ModDataIO.WriteData(Game1.player, ModData.GreenAlgaeOccupantsDict.ToString(),
            greenAlgaeOccupantsDict.Stringify());
        ModDataIO.WriteData(Game1.player, ModData.WhiteAlgaeOccupantsDict.ToString(),
            whiteAlgaeOccupantsDict.Stringify());
        ModDataIO.WriteData(Game1.player, ModData.HeldItemsDict.ToString(), itemsHeldDict.Stringify(">", "/"));
    }
}