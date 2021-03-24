using StardewModdingAPI.Events;

namespace TheLion.AwesomeProfessions
{
	internal class ScavengerDayStartedEvent : DayStartedEvent
	{
		private ScavengerHunt _Hunt { get; }

		/// <summary>Construct an instance.</summary>
		internal ScavengerDayStartedEvent(ScavengerHunt hunt)
		{
			_Hunt = hunt;
		}

		/// <summary>Raised after a new in-game day starts, or after connecting to a multiplayer world. Reset accumulated Scavenger Hunt trigger chance bonus.</summary>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event arguments.</param>
		public override void OnDayStarted(object sender, DayStartedEventArgs e)
		{
			_Hunt.ResetAccumulatedBonus();
		}
	}
}
