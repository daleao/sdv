namespace DaLion.Stardew.Professions.Framework.Events.GameLoop;

#region using directives

using Common.Events;
using Common.Extensions;
using Extensions;
using JetBrains.Annotations;
using StardewModdingAPI.Events;
using StardewValley;

#endregion using directives

[UsedImplicitly]
internal sealed class PrestigeAchievementOneSecondUpdateTickedEvent : OneSecondUpdateTickedEvent
{
    /// <summary>Construct an instance.</summary>
    /// <param name="manager">The <see cref="ProfessionEventManager"/> instance that manages this event.</param>
    internal PrestigeAchievementOneSecondUpdateTickedEvent(ProfessionEventManager manager)
        : base(manager) { }

    /// <inheritdoc />
    protected override void OnOneSecondUpdateTickedImpl(object? sender, OneSecondUpdateTickedEventArgs e)
    {
        if (ModEntry.CookingSkillApi is not null &&
            !ModEntry.CustomSkills.ContainsKey("blueberry.LoveOfCooking.CookingSkill")) return;

        // check for prestige achievements
        if (Game1.player.HasAllProfessions())
        {
            string name =
                ModEntry.i18n.Get("prestige.achievement.name" +
                                  (Game1.player.IsMale ? ".male" : ".female"));
            if (!Game1.player.achievements.Contains(name.GetDeterministicHashCode()))
                Manager.Enable<AchievementUnlockedDayStartedEvent>();
        }

        Disable();
    }
}