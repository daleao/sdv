using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;
using System.Linq;

namespace TheLion.AwesomeProfessions
{
	internal class SpelunkerUpdateTickedEvent : UpdateTickedEvent
	{
		private ITranslationHelper _I18n { get; }

		/// <summary>Construct an instance.</summary>
		internal SpelunkerUpdateTickedEvent(ITranslationHelper i18n)
		{
			_I18n = i18n;
		}

		/// <summary>Raised after the game state is updated. Add or update Spelunker buff.</summary>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event arguments.</param>
		public override void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
		{
			if (Game1.currentLocation is MineShaft)
			{
				Buff buff = Game1.buffsDisplay.otherBuffs.FirstOrDefault(p => p.which == Utility.SpelunkerBuffID);
				if (buff == null)
				{
					Game1.buffsDisplay.addOtherBuff(
						buff = new Buff(0, 0, 0, 0, 0, 0, 0, 0, 0, speed: 1, 0, 0, minutesDuration: 1, source: "spelunker", displaySource: _I18n.Get("spelunker.name"))
						{
							which = Utility.SpelunkerBuffID,
						}
					);

				}
			}
		}
	}
}
