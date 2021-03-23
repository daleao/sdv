using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using System;
using System.Linq;

namespace TheLion.AwesomeProfessions
{
	internal class BruteUpdateTickedEvent : UpdateTickedEvent
	{
		private ITranslationHelper _i18n;

		/// <summary>Construct an instance.</summary>
		internal BruteUpdateTickedEvent(ITranslationHelper i18n)
		{
			_i18n = i18n;
		}

		/// <summary>Raised after the game state is updated. Add or update Brute buff.</summary>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event arguments.</param>
		public override void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
		{
			if (AwesomeProfessions.BruteKillStreak > 0)
			{
				Buff buff = Game1.buffsDisplay.otherBuffs.FirstOrDefault(p => p.which == Utility.BruteBuffID);
				if (buff != null) Game1.buffsDisplay.removeOtherBuff(Utility.BruteBuffID);

				Game1.buffsDisplay.addOtherBuff(
					buff = new Buff(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, minutesDuration: 1, source: "brute", displaySource: _i18n.Get("brute.name"))
					{
						which = Utility.BruteBuffID,
						sheetIndex = 20,
						millisecondsDuration = 50,
						description = _i18n.Get("brute.buffdescription", new { buffMagnitude = Math.Truncate(AwesomeProfessions.BruteKillStreak * 5.0) / 10 })
					}
				);
			}
		}
	}
}
