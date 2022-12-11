namespace DaLion.Ligo.Modules.Professions.Events.GameLoop;

#region using directives

using DaLion.Shared.Events;
using DaLion.Shared.Extensions;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
internal sealed class AchievementUnlockedDayStartedEvent : DayStartedEvent
{
    /// <summary>Initializes a new instance of the <see cref="AchievementUnlockedDayStartedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal AchievementUnlockedDayStartedEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    protected override void OnDayStartedImpl(object? sender, DayStartedEventArgs e)
    {
        string title =
            i18n.Get("prestige.achievement.title" +
                              (Game1.player.IsMale ? ".male" : ".female"));
        Game1.player.achievements.Add(title.GetDeterministicHashCode());
        Game1.playSound("achievement");
        Game1.addHUDMessage(new HUDMessage(title, true));

        this.Disable();
    }
}
