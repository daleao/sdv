namespace DaLion.Stardew.Professions.Framework.Events.GameLoop;

#region using directives

using StardewModdingAPI.Events;
using StardewValley;

using Common.Extensions;

#endregion using directives

internal class AchievementUnlockedDayStartedEvent : DayStartedEvent
{
    /// <inheritdoc />
    protected override void OnDayStartedImpl(object sender, DayStartedEventArgs e)
    {
        string name =
            ModEntry.ModHelper.Translation.Get("prestige.achievement.name." +
                                               (Game1.player.IsMale ? "male" : "female"));
        Game1.player.achievements.Add(name.GetDeterministicHashCode());
        Game1.playSound("achievement");
        Game1.addHUDMessage(new(name, true));

        Disable();
    }
}