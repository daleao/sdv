using StardewModdingAPI.Events;

namespace TheLion.AwesomeProfessions
{
	public class BruteWarpedEvent : BaseWarpedEvent
	{
		/// <summary>Raised after the current player moves to a new location. Reset Brute buff.</summary>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event arguments.</param>
		public override void OnWarped(object sender, WarpedEventArgs e)
		{
			if (e.IsLocalPlayer && AwesomeProfessions.BruteKillStreak > 0 && e.NewLocation.GetType() != e.OldLocation.GetType()) AwesomeProfessions.BruteKillStreak = 0;
		}
	}
}
