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

#endregion using directives

[UsedImplicitly]
internal sealed class PondSaveLoadedEvent : SaveLoadedEvent
{
    /// <summary>Construct an instance.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal PondSaveLoadedEvent(EventManager manager)
        : base(manager) { }

    /// <inheritdoc />
    protected override void OnSaveLoadedImpl(object? sender, SaveLoadedEventArgs e)
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
                ModDataIO.WriteData(pond, "FishQualities", fishQualities);

            if (familyQualitiesDict.TryGetValue(pondId, out var familyQualities))
                ModDataIO.WriteData(pond, "FamilyQualities", familyQualities);

            if (familyOccupantsDict.TryGetValue(pondId, out var familyLivingHere))
                ModDataIO.WriteData(pond, "FamilyLivingHere", familyLivingHere.ToString());

            if (daysEmptyDict.TryGetValue(pondId, out var daysEmpty))
                ModDataIO.WriteData(pond, "DaysEmpty", daysEmpty.ToString());

            if (seaweedOccupantsDict.TryGetValue(pondId, out var seaweedLivingHere))
                ModDataIO.WriteData(pond, "SeaweedLivingHere", seaweedLivingHere.ToString());

            if (greenAlgaeOccupantsDict.TryGetValue(pondId, out var greenAlgaeLivingHere))
                ModDataIO.WriteData(pond, "GreenAlgaeLivingHere", greenAlgaeLivingHere.ToString());

            if (whiteAlgaeOccupantsDict.TryGetValue(pondId, out var whiteAlgaeLivingHere))
                ModDataIO.WriteData(pond, "WhiteAlgaeLivingHere", whiteAlgaeLivingHere.ToString());

            if (itemsHeldDict.TryGetValue(pondId, out var itemsHeld))
                ModDataIO.WriteData(pond, "ItemsHeld", itemsHeld);
        }
    }
}