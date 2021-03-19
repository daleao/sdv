using StardewModdingAPI.Events;

namespace TheLion.AwesomeProfessions
{
	public class ScavengerWarpedEvent : BaseWarpedEvent
	{
		/// <summary>Raised after the current player moves to a new location. Trigger Scavenger hunt events.</summary>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event arguments.</param>
		public override void OnWarped(object sender, WarpedEventArgs e)
		{
			if (e.IsLocalPlayer && e.NewLocation.IsOutdoors)
				AwesomeProfessions.ScavengerHunt.TryStartNewHunt(e.NewLocation);
		}
	}
}
