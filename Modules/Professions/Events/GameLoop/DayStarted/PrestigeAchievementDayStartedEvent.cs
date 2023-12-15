namespace DaLion.Overhaul.Modules.Professions.Events.GameLoop.DayStarted;

#region using directives

using System.Linq;
using DaLion.Shared.Events;
using DaLion.Shared.Extensions;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
internal sealed class PrestigeAchievementDayStartedEvent : DayStartedEvent
{
    private static readonly string AchievementTitle =
        _I18n.Get("prestige.achievement.title" + (Game1.player.IsMale ? ".male" : ".female"));

    /// <summary>Initializes a new instance of the <see cref="PrestigeAchievementDayStartedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal PrestigeAchievementDayStartedEvent(EventManager manager)
        : base(manager)
    {
    }

    private static int AchievementId => AchievementTitle.GetDeterministicHashCode();

    protected override void OnEnabled()
    {
        if (Game1.player.achievements.Contains(AchievementId))
        {
            this.Disable();
        }
    }

    /// <inheritdoc />
    protected override void OnDayStartedImpl(object? sender, DayStartedEventArgs e)
    {
        if (Skill.ListVanilla.Any(skill => skill.CurrentLevel != 20))
        {
            return;
        }

        Game1.player.achievements.Add(AchievementId);
        Game1.playSound("achievement");
        Game1.addHUDMessage(new HUDMessage(AchievementTitle, true));
        this.Disable();
    }
}
