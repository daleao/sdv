using System;
using System.Linq;
using StardewModdingAPI.Events;
using StardewValley;
using TheLion.Stardew.Common.Extensions;

namespace TheLion.Stardew.Professions.Framework.Events
{
	internal class DemolitionistBuffDisplayUpdateTickedEvent : UpdateTickedEvent
	{
		private const int SHEET_INDEX_I = 41;

		private readonly int _buffID;

		/// <summary>Construct an instance.</summary>
		internal DemolitionistBuffDisplayUpdateTickedEvent()
		{
			_buffID = (ModEntry.UniqueID + Utility.Professions.IndexOf("Demolitionist")).Hash();
		}

		/// <inheritdoc />
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
			if (buff is not null) return;

			Game1.buffsDisplay.addOtherBuff(
				new(0,
					0,
					0,
					0,
					0,
					0,
					0,
					0,
					0,
					ModEntry.DemolitionistExcitedness,
					0,
					0,
					1,
					"Demolitionist",
					ModEntry.ModHelper.Translation.Get(
						"demolitionist.name." + (Game1.player.IsMale ? "male" : "female")))
				{
					which = buffID,
					sheetIndex = SHEET_INDEX_I,
					millisecondsDuration = 0,
					description = ModEntry.ModHelper.Translation.Get("demolitionist.buffdesc")
				}
			);
		}
	}
}