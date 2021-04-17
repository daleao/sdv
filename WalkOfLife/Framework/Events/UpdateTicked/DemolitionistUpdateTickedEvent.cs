using StardewModdingAPI.Events;
using StardewValley;
using System;
using System.Linq;

namespace TheLion.AwesomeProfessions
{
	internal class DemolitionistUpdateTickedEvent : UpdateTickedEvent
	{
		/// <inheritdoc/>
		public override void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
		{
			if (AwesomeProfessions.demolitionistBuffMagnitude <= 0) return;

			if (e.Ticks % 30 == 0)
			{
				var buffDecay = AwesomeProfessions.demolitionistBuffMagnitude > 4 ? 2 : 1;
				AwesomeProfessions.demolitionistBuffMagnitude = Math.Max(0, AwesomeProfessions.demolitionistBuffMagnitude - buffDecay);
			}

			var buffId = Utility.DemolitionistBuffID + AwesomeProfessions.demolitionistBuffMagnitude;
			var buff = Game1.buffsDisplay.otherBuffs.FirstOrDefault(p => p.which == buffId);
			if (buff == null)
			{
				Game1.buffsDisplay.addOtherBuff(
					new Buff(0, 0, 0, 0, 0, 0, 0, 0, 0, speed: AwesomeProfessions.demolitionistBuffMagnitude, 0, 0, minutesDuration: 1, source: "demolitionist", displaySource: AwesomeProfessions.I18n.Get("demolitionist.name"))
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