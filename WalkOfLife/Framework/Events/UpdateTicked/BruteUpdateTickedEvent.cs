using StardewModdingAPI.Events;
using StardewValley;
using System;
using System.Linq;

namespace TheLion.AwesomeProfessions
{
	internal class BruteUpdateTickedEvent : UpdateTickedEvent
	{
		/// <inheritdoc/>
		public override void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
		{
			if (AwesomeProfessions.bruteKillStreak <= 0) return;

			var buff = Game1.buffsDisplay.otherBuffs.FirstOrDefault(p => p.which == Utility.BruteBuffID);
			if (buff != null) Game1.buffsDisplay.removeOtherBuff(Utility.BruteBuffID);

			Game1.buffsDisplay.addOtherBuff(
				new Buff(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, minutesDuration: 1, source: "brute", displaySource: AwesomeProfessions.I18n.Get("brute.name"))
				{
					which = Utility.BruteBuffID,
					sheetIndex = 20,
					millisecondsDuration = 50,
					description = AwesomeProfessions.I18n.Get("brute.buffdescription", new { buffMagnitude = Math.Truncate(AwesomeProfessions.bruteKillStreak * 5.0) / 10 })
				}
			);
		}
	}
}