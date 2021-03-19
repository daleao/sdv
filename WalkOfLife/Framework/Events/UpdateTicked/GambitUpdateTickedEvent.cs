using StardewModdingAPI.Events;
using StardewValley;
using System;
using System.Linq;

namespace TheLion.AwesomeProfessions
{
	public class GambitUpdateTickedEvent : BaseUpdateTickedEvent
	{
		/// <summary>Raised after the game state is updated. Add or update Gambit buff.</summary>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event arguments.</param>
		public override void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
		{
			double healthPercent;
			if ((healthPercent = (double)Game1.player.health / Game1.player.maxHealth) < 1)
			{
				Buff buff = Game1.buffsDisplay.otherBuffs.FirstOrDefault(p => p.which == Utility.GambitBuffID);
				if (buff != null)
					Game1.buffsDisplay.removeOtherBuff(Utility.GambitBuffID);

				Game1.buffsDisplay.addOtherBuff(
					buff = new Buff(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, minutesDuration: 1, source: "gambit", displaySource: AwesomeProfessions.I18n.Get("gambit.name"))
					{
						which = Utility.GambitBuffID,
						sheetIndex = 20,
						millisecondsDuration = 50,
						description = AwesomeProfessions.I18n.Get("gambit.buffdescription", new { buffMagnitude = Math.Truncate(200.0 / (healthPercent + 0.2) - 200.0 / 1.2) / 10 })
					}
				);
			}
		}
	}
}
