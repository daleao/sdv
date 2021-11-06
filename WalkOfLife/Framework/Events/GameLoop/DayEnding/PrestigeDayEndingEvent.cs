using JetBrains.Annotations;
using StardewModdingAPI.Enums;
using StardewModdingAPI.Events;
using StardewValley;
using TheLion.Stardew.Professions.Framework.Extensions;

namespace TheLion.Stardew.Professions.Framework.Events
{
	[UsedImplicitly]
	internal class PrestigeDayEndingEvent : DayEndingEvent
	{
		public SkillType Skill { get; }

		internal PrestigeDayEndingEvent(SkillType whichSkill)
		{
			Skill = whichSkill;
		}

		/// <inheritdoc />
		public override void OnDayEnding(object sender, DayEndingEventArgs e)
		{
			Game1.player.PrestigeSkill(Skill);
		}
	}
}