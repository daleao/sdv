using System.Collections.Generic;
using System.Linq;
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
		public Queue<SkillType> SkillQueue { get; } = new();

		internal PrestigeDayEndingEvent(SkillType whichSkill)
		{
			SkillQueue.Enqueue(whichSkill);
		}

		/// <inheritdoc />
		public override void OnDayEnding(object sender, DayEndingEventArgs e)
		{
			while (SkillQueue.Any()) Game1.player.PrestigeSkill(SkillQueue.Dequeue());

			ModEntry.Subscriber.Unsubscribe(GetType());
		}
	}
}