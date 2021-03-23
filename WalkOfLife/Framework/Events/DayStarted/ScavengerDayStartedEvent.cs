using StardewModdingAPI.Events;

namespace TheLion.AwesomeProfessions
{
	internal class ScavengerDayStartedEvent : DayStartedEvent
	{
		private readonly ScavengerHunt _hunt;

		/// <summary>Construct an instance.</summary>
		internal ScavengerDayStartedEvent(ScavengerHunt hunt)
		{
			_hunt = hunt;
		}

		/// <summary>Raised after a new in-game day starts, or after connecting to a multiplayer world. Reset accumulated Scavenger Hunt trigger chance bonus.</summary>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event arguments.</param>
		public override void OnDayStarted(object sender, DayStartedEventArgs e)
		{
			_hunt.ResetAccumulatedBonus();
		}
	}
}
