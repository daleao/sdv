using StardewModdingAPI.Events;
using StardewValley;
using System;
using System.IO;
using SObject = StardewValley.Object;

namespace TheLion.AwesomeProfessions
{
	internal class OenologistDayEndingEvent : DayEndingEvent
	{
		/// <summary>Construct an instance.</summary>
		internal OenologistDayEndingEvent() { }

		/// <summary>Raised before the game ends the current day. Receive Oenologist mail from the SWA about Decanter's award.</summary>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event arguments.</param>
		public override void OnDayEnding(object sender, DayEndingEventArgs e)
		{
			foreach (var item in Game1.getFarm().getShippingBin(Game1.player))
			{
				if (Utility.IsWine(item))
				{
					switch ((item as SObject).Quality)
					{
						case SObject.bestQuality:
							_data.OenologyFameAccrued += 3;
							break;
						case SObject.highQuality:
							_data.OenologyFameAccrued += 1;
							break;
						case SObject.medQuality:
							_data.OenologyFameAccrued += 0;
							break;
						case SObject.lowQuality:
							_data.OenologyFameAccrued = Math.Max(_data.OenologyFameAccrued - 1, 0);
							break;
					}
				}
			}

			if (Game1.dayOfMonth == 28 && _data.OenologyFameAccrued > 0)
			{
				uint awardLevel = Utility.GetLocalPlayerOenologyAwardLevel();
				if (awardLevel > _data.HighestOenologyAwardEarned)
				{
					_data.HighestOenologyAwardEarned = awardLevel;
					AwesomeProfessions.ModHelper.Content.InvalidateCache(Path.Combine("Data", "mail"));
					Game1.addMailForTomorrow("OenologistAwardNotice");
				}
			}
		}
	}
}
