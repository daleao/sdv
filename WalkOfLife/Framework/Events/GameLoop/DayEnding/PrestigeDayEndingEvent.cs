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
		internal PrestigeDayEndingEvent(SkillType skillType)
		{
			SkillsToPrestige.Enqueue(skillType);
		}

		public Queue<SkillType> SkillsToPrestige { get; } = new();

		/// <inheritdoc />
		public override void OnDayEnding(object sender, DayEndingEventArgs e)
		{
			while (SkillsToPrestige.Any()) Game1.player.PrestigeSkill(SkillsToPrestige.Dequeue());

			ModEntry.Subscriber.Unsubscribe(GetType());
		}
	}
}