using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace TheLion.AwesomeProfessions
{
	public partial class AwesomeProfessions
	{
		/// <summary>Raised after a new in-game day starts, or after connecting to a multiplayer world.</summary>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event arguments.</param>
		private void OnDayStarted(object sender, DayStartedEventArgs e)
		{
			// reset ScavengerTreasureHunts
			//if (TreasureHunt.TreasuresScavengedToday > 0) TreasureHunt.TreasuresScavengedToday = 0;

			// reset Prospector treasure hunts
			//if (TreasureHunt.TreasuresProspectedToday > 0) TreasureHunt.TreasuresProspectedToday = 0;

			// reset Conservationist collected trash
			if (Data.OceanTrashCollectedThisSeason > 0 && Game1.dayOfMonth == 1) Data.OceanTrashCollectedThisSeason = 0;
		}
	}
}
