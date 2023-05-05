namespace DaLion.Overhaul.Modules.Professions.Events.GameLoop;

#region using directives

using DaLion.Overhaul.Modules.Professions.Extensions;
using DaLion.Shared.Events;
using DaLion.Shared.Extensions;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
internal sealed class PrestigeAchievementOneSecondUpdateTickedEvent : OneSecondUpdateTickedEvent
{
    /// <summary>Initializes a new instance of the <see cref="PrestigeAchievementOneSecondUpdateTickedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal PrestigeAchievementOneSecondUpdateTickedEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    protected override void OnOneSecondUpdateTickedImpl(object? sender, OneSecondUpdateTickedEventArgs e)
    {
        // check for prestige achievements
        if (Game1.player.HasAllProfessions())
        {
            string title =
                _I18n.Get("prestige.achievement.title" +
                                  (Game1.player.IsMale ? ".male" : ".female"));
            if (!Game1.player.achievements.Contains(title.GetDeterministicHashCode()))
            {
                this.Manager.Enable<AchievementUnlockedDayStartedEvent>();
            }
        }

        this.Disable();
    }
}
