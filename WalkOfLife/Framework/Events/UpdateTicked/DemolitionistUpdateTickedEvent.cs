using StardewModdingAPI.Events;
using StardewValley;
using System;
using System.Linq;

namespace TheLion.AwesomeProfessions
{
	public class DemolitionistUpdateTickedEvent : BaseUpdateTickedEvent
	{
		/// <summary>Raised after the game state is updated. Add or update Demolitionist buff.</summary>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event arguments.</param>
		public override void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
		{
			if (AwesomeProfessions.DemolitionistBuffMagnitude > 0)
			{
				if (e.Ticks % 30 == 0)
				{
					int buffDecay = AwesomeProfessions.DemolitionistBuffMagnitude > 4 ? 2 : 1;
					AwesomeProfessions.DemolitionistBuffMagnitude = Math.Max(0, AwesomeProfessions.DemolitionistBuffMagnitude - buffDecay);
				}

				int buffId = Utility.DemolitionistBuffID + AwesomeProfessions.DemolitionistBuffMagnitude;
				Buff buff = Game1.buffsDisplay.otherBuffs.FirstOrDefault(p => p.which == buffId);
				if (buff == null)
				{
					Game1.buffsDisplay.addOtherBuff(
						buff = new Buff(0, 0, 0, 0, 0, 0, 0, 0, 0, speed: AwesomeProfessions.DemolitionistBuffMagnitude, 0, 0, minutesDuration: 1, source: "demolitionist", displaySource: AwesomeProfessions.I18n.Get("demolitionist.name"))
						{
							which = buffId,
							millisecondsDuration = 50,
							description = AwesomeProfessions.I18n.Get("demolitionist.buffdescription")
						}
					);
				}
			}
		}
	}
}
