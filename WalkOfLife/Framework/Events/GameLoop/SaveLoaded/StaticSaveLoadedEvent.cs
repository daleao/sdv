using System.Linq;
using JetBrains.Annotations;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using TheLion.Stardew.Common.Extensions;
using TheLion.Stardew.Professions.Framework.AssetEditors;
using TheLion.Stardew.Professions.Framework.Extensions;

namespace TheLion.Stardew.Professions.Framework.Events;

[UsedImplicitly]
internal class StaticSaveLoadedEvent : SaveLoadedEvent
{
    /// <inheritdoc />
    public override void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
    {
        // load persisted mod data
        ModEntry.Data.Load();

        // subscribe player's profession events
        ModEntry.Subscriber.SubscribeEventsForLocalPlayer();

        // load and validate Super Mode
        ModEntry.Log("Loading persisted Super Mode index.", LogLevel.Trace);
        var superModeIndex = ModEntry.Data.Read<int>("SuperModeIndex");
        switch (superModeIndex)
        {
            case < 0 when Game1.player.professions.Any(p => p is >= 26 and < 30):
                ModEntry.Log($"Player eligible for Super Mode but currently not registered to any. Setting to a default value.", LogLevel.Warn);
                ModState.SuperModeIndex = Game1.player.professions.First(p => p is >= 26 and < 30);
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
                ModState.SuperModeIndex = superModeIndex;
                break;
        }

        if (!Game1.player.HasAllProfessions()) return;

        // check for prestige achievements
        string name =
            ModEntry.ModHelper.Translation.Get("prestige.achievement.name." +
                                               (Game1.player.IsMale ? "male" : "female"));
        if (Game1.player.achievements.Contains(name.Hash()) &&
            !ModEntry.ModHelper.Content.AssetEditors.ContainsType(typeof(AchivementsEditor)))
            ModEntry.ModHelper.Content.AssetEditors.Add(new AchivementsEditor());
        else
            ModEntry.Subscriber.Subscribe(new AchievementUnlockedDayStartedEvent());
    }

    public static void ResetSuperMode()
    {
        if (Game1.player.professions.Any(p => p is >= 26 and < 30))
            ModState.SuperModeIndex = Game1.player.professions.First(p => p is >= 26 and < 30);
        else
            ModState.SuperModeIndex = -1;
    }
}