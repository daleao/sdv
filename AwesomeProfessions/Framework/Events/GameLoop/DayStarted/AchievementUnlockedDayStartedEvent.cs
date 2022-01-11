using StardewModdingAPI.Events;
using StardewValley;
using TheLion.Stardew.Common.Extensions;
using TheLion.Stardew.Professions.Framework.AssetEditors;

namespace TheLion.Stardew.Professions.Framework.Events.GameLoop.DayStarted;

internal class AchievementUnlockedDayStartedEvent : DayStartedEvent
{
    /// <inheritdoc />
    protected override void OnDayStartedImpl(object sender, DayStartedEventArgs e)
    {
        if (!ModEntry.ModHelper.Content.AssetEditors.ContainsType(typeof(AchivementsEditor)))
            ModEntry.ModHelper.Content.AssetEditors.Add(new AchivementsEditor());

        string name =
            ModEntry.ModHelper.Translation.Get("prestige.achievement.name." +
                                               (Game1.player.IsMale ? "male" : "female"));
        Game1.player.achievements.Add(name.GetDeterministicHashCode());
        Game1.playSound("achievement");
        Game1.addHUDMessage(new(name, true));

        Disable();
    }
}