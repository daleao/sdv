namespace DaLion.Stardew.Professions.Framework.Events.GameLoop;

#region using directives

using System.Linq;
using JetBrains.Annotations;
using StardewModdingAPI.Events;
using StardewValley;

using Common.Extensions;
using Extensions;

#endregion using directives

[UsedImplicitly]
internal class PrestigeAchievementOneSecondUpdateTickedEvent : OneSecondUpdateTickedEvent
{
    /// <inheritdoc />
    protected override void OnOneSecondUpdateTickedImpl(object sender, OneSecondUpdateTickedEventArgs e)
    {
        if (ModEntry.CookingSkillApi is not null &&
            ModEntry.CustomSkills.All(s => s.StringId != "blueberry.LoveOfCooking")) return;

        // check for prestige achievements
        if (Game1.player.HasAllProfessions())
        {
            string name =
                ModEntry.i18n.Get("prestige.achievement.name." +
                                  (Game1.player.IsMale ? "male" : "female"));
            if (!Game1.player.achievements.Contains(name.GetDeterministicHashCode()))
                EventManager.Enable(typeof(AchievementUnlockedDayStartedEvent));
        }

        Disable();
    }
}