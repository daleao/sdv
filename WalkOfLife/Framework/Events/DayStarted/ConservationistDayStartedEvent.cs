using StardewModdingAPI.Events;
using StardewValley;

namespace TheLion.AwesomeProfessions
{
	public class ConservationistDayStartedEvent : BaseDayStartedEvent
	{
		/// <summary>Raised after a new in-game day starts, or after connecting to a multiplayer world. Reset Conservationist trash count.</summary>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event arguments.</param>
		public override void OnDayStarted(object sender, DayStartedEventArgs e)
		{
			if (AwesomeProfessions.Data.OceanTrashCollectedThisSeason > 0 && Game1.dayOfMonth == 1) AwesomeProfessions.Data.OceanTrashCollectedThisSeason = 0;
		}
	}
}
