namespace DaLion.Stardew.Professions.Framework.Events.GameLoop;

#region using directives

using System.Linq;
using JetBrains.Annotations;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Buildings;

using Common.Extensions;
using Extensions;
using Ultimate;

#endregion using directives

[UsedImplicitly]
internal class StaticSaveLoadedEvent : SaveLoadedEvent
{
    /// <summary>Construct an instance.</summary>
    internal StaticSaveLoadedEvent()
    {
        Enable();
    }

    /// <inheritdoc />
    protected override void OnSaveLoadedImpl(object sender, SaveLoadedEventArgs e)
    {
        // enable events
        EventManager.EnableAllForLocalPlayer();

        // load or initialize Ultimate index
        var superModeIndex = Game1.player.ReadDataAs(DataField.UltimateIndex, UltimateIndex.None);

        // validate Ultimate index
        switch (superModeIndex)
        {
            case UltimateIndex.None when Game1.player.professions.Any(p => p is >= 26 and < 30):
                Log.W("Player eligible for Ultimate but not currently registered to any. Setting to a default value.");
                superModeIndex = (UltimateIndex) Game1.player.professions.First(p => p is >= 26 and < 30);
                Game1.player.WriteData(DataField.UltimateIndex, superModeIndex.ToString());

                break;

            case > UltimateIndex.None when !Game1.player.professions.Contains((int) superModeIndex):
                Log.W($"Missing corresponding profession for {superModeIndex} Ultimate. Resetting to a default value.");
                if (Game1.player.professions.Any(p => p is >= 26 and < 30))
                {
                    superModeIndex = (UltimateIndex) Game1.player.professions.First(p => p is >= 26 and < 30);
                    Game1.player.WriteData(DataField.UltimateIndex, superModeIndex.ToString());
                }
                else
                {
                    superModeIndex = UltimateIndex.None;
                    Game1.player.WriteData(DataField.UltimateIndex, null);
                }

                break;
        }

        // initialize Ultimate
        if (superModeIndex > UltimateIndex.None)
        {
            ModEntry.PlayerState.Value.RegisteredUltimate =
#pragma warning disable CS8509
                ModEntry.PlayerState.Value.RegisteredUltimate = superModeIndex switch
#pragma warning restore CS8509
                {
                    UltimateIndex.Brute => new Frenzy(),
                    UltimateIndex.Poacher => new Ambush(),
                    UltimateIndex.Piper => new Pandemonia(),
                    UltimateIndex.Desperado => new DeathBlossom()
                };
        }

        // check for prestige achievements
        if (Game1.player.HasAllProfessions())
        {
            string name =
                ModEntry.ModHelper.Translation.Get("prestige.achievement.name." +
                                                   (Game1.player.IsMale ? "male" : "female"));
            if (Game1.player.achievements.Contains(name.GetDeterministicHashCode())) return;

            EventManager.Enable(typeof(AchievementUnlockedDayStartedEvent));
        }

        // restore fish pond quality data
        if (ModEntry.Config.RebalanceFishPonds && Context.IsMainPlayer)
        {
            var pondQualityDict = Game1.player.ReadData(DataField.FishPondQualityDict).ToDictionary<int, int>(',', ';');
            var familyQualityDict = Game1.player.ReadData(DataField.FishPondFamilyQualityDict).ToDictionary<int, int>(',', ';');
            var familyCountDict = Game1.player.ReadData(DataField.FishPondFamilyCountDict).ToDictionary<int, int>(',', ';');
            var daysEmptyDict = Game1.player.ReadData(DataField.FishPondDaysEmptyDict).ToDictionary<int, int>(',', ';');
            foreach (var pond in Game1.getFarm().buildings.OfType<FishPond>().Where(p => !p.isUnderConstruction()))
            {
                var pondId = pond.GetCenterTile().ToString().GetDeterministicHashCode();
                if (pondQualityDict.TryGetValue(pondId, out var qualityRating))
                    pond.WriteData("QualityRating", qualityRating.ToString());

                if (familyQualityDict.TryGetValue(pondId, out var familyQualityRating))
                    pond.WriteData("FamilyQualityRating", familyQualityRating.ToString());

                if (familyCountDict.TryGetValue(pondId, out var familyCount))
                    pond.WriteData("FamilyCount", familyCount.ToString());

                if (daysEmptyDict.TryGetValue(pondId, out var daysEmpty))
                    pond.WriteData("DaysEmpty", daysEmpty.ToString());
            }
        }
    }
}