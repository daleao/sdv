using JetBrains.Annotations;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using System.Linq;
using TheLion.Stardew.Common.Extensions;
using TheLion.Stardew.Professions.Framework.Events.GameLoop.DayStarted;
using TheLion.Stardew.Professions.Framework.Extensions;

namespace TheLion.Stardew.Professions.Framework.Events.GameLoop.SaveLoaded;

[UsedImplicitly]
internal class StaticSaveLoadedEvent : SaveLoadedEvent
{
    /// <inheritdoc />
    public override void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
    {
        // load persisted mod data
        ModEntry.Data.Value.Load();

        // subscribe player's profession events
        ModEntry.Subscriber.SubscribeEventsForLocalPlayer();

        // load and validate Super Mode
        ModEntry.Log("Loading persisted Super Mode index.", LogLevel.Trace);
        var superModeIndex = ModEntry.Data.Value.Read<int>("SuperModeIndex");
        switch (superModeIndex)
        {
            case < 0 when Game1.player.professions.Any(p => p is >= 26 and < 30):
                ModEntry.Log($"Player eligible for Super Mode but currently not registered to any. Setting to a default value.", LogLevel.Warn);
                ModEntry.State.Value.SuperModeIndex = Game1.player.professions.First(p => p is >= 26 and < 30);
                break;

            case > 0 and < 26 or >= 30:
                ModEntry.Log($"Unexpected Super Mode index {superModeIndex}. Resetting to a default value.", LogLevel.Warn);
                ResetSuperMode();
                break;

            case >= 26 and < 30 when !Game1.player.professions.Contains(superModeIndex):
                ModEntry.Log($"Missing corresponding profession for Super Mode index {superModeIndex}. Resetting to a default value.", LogLevel.Warn);
                ResetSuperMode();
                break;

            default:
                ModEntry.State.Value.SuperModeIndex = superModeIndex;
                break;
        }

        // check for prestige achievements
        if (!Game1.player.HasAllProfessions()) return;

        string name =
            ModEntry.ModHelper.Translation.Get("prestige.achievement.name." +
                                               (Game1.player.IsMale ? "male" : "female"));
        if (Game1.player.achievements.Contains(name.GetDeterministicHashCode())) return;

        ModEntry.Subscriber.Subscribe(new AchievementUnlockedDayStartedEvent());
    }

    public static void ResetSuperMode()
    {
        if (Game1.player.professions.Any(p => p is >= 26 and < 30))
            ModEntry.State.Value.SuperModeIndex = Game1.player.professions.First(p => p is >= 26 and < 30);
        else
            ModEntry.State.Value.SuperModeIndex = -1;
    }
}