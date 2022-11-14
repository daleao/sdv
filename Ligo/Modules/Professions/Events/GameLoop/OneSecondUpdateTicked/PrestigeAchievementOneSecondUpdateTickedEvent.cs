namespace DaLion.Ligo.Modules.Professions.Events.GameLoop;

#region using directives

using DaLion.Ligo.Modules.Professions.Extensions;
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
        if (Ligo.Integrations.CookingSkillApi is not null &&
            !SCSkill.Loaded.ContainsKey("blueberry.LoveOfCooking.CookingSkill"))
        {
            return;
        }

        // check for prestige achievements
        if (Game1.player.HasAllProfessions())
        {
            string name =
                ModEntry.i18n.Get("prestige.achievement.name" +
                                  (Game1.player.IsMale ? ".male" : ".female"));
            if (!Game1.player.achievements.Contains(name.GetDeterministicHashCode()))
            {
                this.Manager.Enable<AchievementUnlockedDayStartedEvent>();
            }
        }

        this.Disable();
    }
}
