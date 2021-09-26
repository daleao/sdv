using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;
using System;
using System.Linq;
using TheLion.Stardew.Common.Extensions;

namespace TheLion.Stardew.Professions.Framework.Events
{
	public class SpelunkerBuffDisplayUpdateTickedEvent : UpdateTickedEvent
	{
		private const int SHEET_INDEX = 40;

		private readonly int _buffID;

		/// <summary>Construct an instance.</summary>
		internal SpelunkerBuffDisplayUpdateTickedEvent()
		{
			_buffID = (ModEntry.UniqueID + Util.Professions.IndexOf("Spelunker")).Hash();
		}

		/// <inheritdoc/>
		public override void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
		{
			if (Game1.currentLocation is not MineShaft) return;

			var buff = Game1.buffsDisplay.otherBuffs.FirstOrDefault(p => p.which == _buffID);
			if (buff != null) return;

			var bonusLadderChance = ModEntry.SpelunkerLadderStreak;
			var bonusSpeed = Math.Min(ModEntry.SpelunkerLadderStreak / 5 + 1, 10);
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
						 speed: bonusSpeed,
						 0,
						 0,
						 minutesDuration: 1,
						 source: "Spelunker",
						 displaySource: ModEntry.ModHelper.Translation.Get("spelunker.buffname"))
				{
					which = _buffID,
					sheetIndex = SHEET_INDEX,
					millisecondsDuration = 49,
					description = ModEntry.ModHelper.Translation.Get("spelunker.buffdescription", new { bonusLadderChance, bonusSpeed })
				}
			);
		}
	}
}