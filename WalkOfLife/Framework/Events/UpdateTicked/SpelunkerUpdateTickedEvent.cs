using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;
using System.Linq;

namespace TheLion.AwesomeProfessions
{
	internal class SpelunkerUpdateTickedEvent : UpdateTickedEvent
	{
		/// <inheritdoc/>
		public override void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
		{
			if (Game1.currentLocation is not MineShaft) return;

			var buff = Game1.buffsDisplay.otherBuffs.FirstOrDefault(p => p.which == Utility.SpelunkerBuffID);
			if (buff == null)
			{
				Game1.buffsDisplay.addOtherBuff(
					new Buff(0, 0, 0, 0, 0, 0, 0, 0, 0, speed: 1, 0, 0, minutesDuration: 1, source: "spelunker", displaySource: AwesomeProfessions.I18n.Get("spelunker.name"))
					{
						which = Utility.SpelunkerBuffID,
						millisecondsDuration = 50
					}
				);
			}
		}
	}
}