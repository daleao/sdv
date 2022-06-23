namespace DaLion.Stardew.Professions.Framework.Events.GameLoop;

#region using directives

using JetBrains.Annotations;
using StardewModdingAPI.Events;
using StardewValley;

using Common.Events;
using Common.Extensions;
using Extensions;

#endregion using directives

[UsedImplicitly]
internal sealed class PrestigeAchievementOneSecondUpdateTickedEvent : OneSecondUpdateTickedEvent
{
    /// <inheritdoc />
    protected override void OnOneSecondUpdateTickedImpl(object sender, OneSecondUpdateTickedEventArgs e)
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
                ModEntry.EventManager.Hook<AchievementUnlockedDayStartedEvent>();
        }

        Unhook();
    }
}