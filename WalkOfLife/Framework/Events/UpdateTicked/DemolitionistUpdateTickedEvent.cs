using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using System;
using System.Linq;

namespace TheLion.AwesomeProfessions
{
	internal class DemolitionistUpdateTickedEvent : UpdateTickedEvent
	{
		private ITranslationHelper _I18n { get; }

		/// <summary>Construct an instance.</summary>
		internal DemolitionistUpdateTickedEvent(ITranslationHelper i18n)
		{
			_I18n = i18n;
		}

		/// <summary>Raised after the game state is updated. Add or update Demolitionist buff.</summary>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event arguments.</param>
		public override void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
		{
			if (AwesomeProfessions.demolitionistBuffMagnitude > 0)
			{
				if (e.Ticks % 30 == 0)
				{
					int buffDecay = AwesomeProfessions.demolitionistBuffMagnitude > 4 ? 2 : 1;
					AwesomeProfessions.demolitionistBuffMagnitude = Math.Max(0, AwesomeProfessions.demolitionistBuffMagnitude - buffDecay);
				}

				int buffId = Utility.DemolitionistBuffID + AwesomeProfessions.demolitionistBuffMagnitude;
				Buff buff = Game1.buffsDisplay.otherBuffs.FirstOrDefault(p => p.which == buffId);
				if (buff == null)
				{
					Game1.buffsDisplay.addOtherBuff(
						buff = new Buff(0, 0, 0, 0, 0, 0, 0, 0, 0, speed: AwesomeProfessions.demolitionistBuffMagnitude, 0, 0, minutesDuration: 1, source: "demolitionist", displaySource: _I18n.Get("demolitionist.name"))
						{
							which = buffId,
							millisecondsDuration = 50,
							description = _I18n.Get("demolitionist.buffdescription")
						}
					);
				}
			}
		}
	}
}
