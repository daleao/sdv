using StardewModdingAPI.Events;
using StardewValley;
using System.Linq;

namespace TheLion.AwesomeProfessions
{
	internal class BruteWarpedEvent : WarpedEvent
	{
		/// <summary>Construct an instance.</summary>
		internal BruteWarpedEvent() { }

		/// <summary>Raised after the current player moves to a new location. Reset Brute buff.</summary>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event arguments.</param>
		public override void OnWarped(object sender, WarpedEventArgs e)
		{
			if (e.IsLocalPlayer && AwesomeProfessions.bruteKillStreak > 0 && e.NewLocation.GetType() != e.OldLocation.GetType())
				AwesomeProfessions.bruteKillStreak = 0;
		}
	}
}
