namespace DaLion.Stardew.Ponds.Framework.Events;

#region using directives

using System.Linq;
using JetBrains.Annotations;
using StardewValley;
using StardewValley.Buildings;
using StardewModdingAPI;
using StardewModdingAPI.Events;

using Common.Data;
using Common.Events;
using Common.Extensions;
using Extensions;

#endregion using directives

[UsedImplicitly]
internal sealed class PondSaveLoadedEvent : SaveLoadedEvent
{
    /// <inheritdoc />
    protected override void OnSaveLoadedImpl(object sender, SaveLoadedEventArgs e)
    {
        if (!Context.IsMainPlayer) return;

        var fishQualitiesDict = ModDataIO.ReadData(Game1.player, ModData.FishQualitiesDict.ToString())
            .ParseDictionary<int, string>(">", "/");
        var familyQualitiesDict =
            ModDataIO.ReadData(Game1.player, ModData.FamilyQualitiesDict.ToString())
                .ParseDictionary<int, string>(">", "/");
        var familyOccupantsDict = ModDataIO.ReadData(Game1.player, ModData.FamilyOccupantsDict.ToString())
            .ParseDictionary<int, int>();
        var daysEmptyDict = ModDataIO.ReadData(Game1.player, ModData.DaysEmptyDict.ToString())
            .ParseDictionary<int, int>();
        var seaweedOccupantsDict = ModDataIO.ReadData(Game1.player, ModData.SeaweedOccupantsDict.ToString())
            .ParseDictionary<int, int>();
        var greenAlgaeOccupantsDict =
            ModDataIO.ReadData(Game1.player, ModData.GreenAlgaeOccupantsDict.ToString()).ParseDictionary<int, int>();
        var whiteAlgaeOccupantsDict =
            ModDataIO.ReadData(Game1.player, ModData.WhiteAlgaeOccupantsDict.ToString()).ParseDictionary<int, int>();
        var itemsHeldDict = ModDataIO.ReadData(Game1.player, ModData.HeldItemsDict.ToString())
            .ParseDictionary<int, string>(">", "/");

        foreach (var pond in Game1.getFarm().buildings.OfType<FishPond>().Where(p =>
                     (p.owner.Value == Game1.player.UniqueMultiplayerID || !Context.IsMultiplayer) &&
                     !p.isUnderConstruction()))
        {
            var pondId = pond.GetCenterTile().ToString().GetDeterministicHashCode();

            if (fishQualitiesDict.TryGetValue(pondId, out var fishQualities))
                pond.WriteData("FishQualities", fishQualities);

            if (familyQualitiesDict.TryGetValue(pondId, out var familyQualities))
                pond.WriteData("FamilyQualities", familyQualities);

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