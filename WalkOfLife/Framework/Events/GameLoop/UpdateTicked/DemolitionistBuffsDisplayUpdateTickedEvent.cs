using StardewModdingAPI.Events;
using StardewValley;
using System;
using System.Linq;
using TheLion.Stardew.Common.Extensions;

namespace TheLion.Stardew.Professions.Framework.Events
{
	public class DemolitionistBuffsDisplayUpdateTickedEvent : UpdateTickedEvent
	{
		private const int SHEET_INDEX = 41;

		private readonly int _buffID;

		/// <summary>Construct an instance.</summary>
		internal DemolitionistBuffsDisplayUpdateTickedEvent()
		{
			_buffID = (ModEntry.UniqueID + Util.Professions.IndexOf("Demolitionist")).Hash();
		}

		/// <inheritdoc/>
		public override void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
		{
			if (ModEntry.DemolitionistExcitedness <= 0) ModEntry.Subscriber.Unsubscribe(GetType());

			if (e.Ticks % 30 == 0)
			{
				var buffDecay = ModEntry.DemolitionistExcitedness > 4 ? 2 : 1;
				ModEntry.DemolitionistExcitedness = Math.Max(0, ModEntry.DemolitionistExcitedness - buffDecay);
			}

			var buffID = _buffID + ModEntry.DemolitionistExcitedness;
			var buff = Game1.buffsDisplay.otherBuffs.FirstOrDefault(p => p.which == buffID);
			if (buff != null) return;

			Game1.buffsDisplay.addOtherBuff(
				new Buff(0,
					0,
					0,
					0,
					0,
					0,
					0,
					0,
					0,
					speed: ModEntry.DemolitionistExcitedness,
					0,
					0,
					minutesDuration: 1,
					source: "Demolitionist",
					displaySource: ModEntry.I18n.Get("demolitionist.name"))
				{
					which = buffID,
					sheetIndex = SHEET_INDEX,
					millisecondsDuration = 49,
					description = ModEntry.I18n.Get("demolitionist.buffdescription")
				}
			);
		}
	}
}