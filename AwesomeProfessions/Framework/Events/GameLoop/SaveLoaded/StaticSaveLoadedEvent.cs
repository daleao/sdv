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
using SuperMode;

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

        // load or initialize Super Mode index
        var superModeIndex = Game1.player.ReadDataAs(DataField.SuperModeIndex, SuperModeIndex.None);

        // validate Super Mode index
        switch (superModeIndex)
        {
            case SuperModeIndex.None when Game1.player.professions.Any(p => p is >= 26 and < 30):
                Log.W("Player eligible for Super Mode but not currently registered to any. Setting to a default value.");
                superModeIndex = (SuperModeIndex) Game1.player.professions.First(p => p is >= 26 and < 30);
                Game1.player.WriteData(DataField.SuperModeIndex, superModeIndex.ToString());

                break;

            case > SuperModeIndex.None when !Game1.player.professions.Contains((int) superModeIndex):
                Log.W($"Missing corresponding profession for {superModeIndex} Super Mode. Resetting to a default value.");
                if (Game1.player.professions.Any(p => p is >= 26 and < 30))
                {
                    superModeIndex = (SuperModeIndex) Game1.player.professions.First(p => p is >= 26 and < 30);
                    Game1.player.WriteData(DataField.SuperModeIndex, superModeIndex.ToString());
                }
                else
                {
                    superModeIndex = SuperModeIndex.None;
                    Game1.player.WriteData(DataField.SuperModeIndex, null);
                }

                break;
        }

        // initialize Super Mode
        if (superModeIndex > SuperModeIndex.None)
        {
            ModEntry.PlayerState.Value.SuperMode =
#pragma warning disable CS8509
                ModEntry.PlayerState.Value.SuperMode = superModeIndex switch
#pragma warning restore CS8509
                {
                    SuperModeIndex.Brute => new BruteFury(),
                    SuperModeIndex.Poacher => new PoacherColdBlood(),
                    SuperModeIndex.Piper => new PiperEubstance(),
                    SuperModeIndex.Desperado => new DesperadoTemerity()
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
        if (ModEntry.Config.EnableFishPondRebalance && Context.IsMainPlayer)
        {
            var pondQualityDict = Game1.player.ReadData(DataField.FishPondQualityDict).ToDictionary<int, int>(',', ';');
            foreach (var pond in Game1.getFarm().buildings.OfType<FishPond>().Where(p => !p.isUnderConstruction()))
            {
                var pondId = pond.GetCenterTile().ToString().GetDeterministicHashCode();
                if (pondQualityDict.TryGetValue(pondId, out var qualityRating))
                    pond.WriteData("QualityRating", qualityRating.ToString());
            }
        }
    }
}