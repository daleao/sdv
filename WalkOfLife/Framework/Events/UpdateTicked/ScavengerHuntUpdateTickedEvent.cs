using StardewModdingAPI.Events;

namespace TheLion.AwesomeProfessions
{
	internal class ScavengerHuntUpdateTickedEvent : UpdateTickedEvent
	{
		private ScavengerHunt _Hunt { get; }

		/// <summary>Construct an instance.</summary>
		internal ScavengerHuntUpdateTickedEvent(ScavengerHunt hunt)
		{
			_Hunt = hunt;
		}

		/// <summary>Raised after the game state is updated. Handle Scavenger hunt events.</summary>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event arguments.</param>
		public override void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
		{
			_Hunt.Update(e.Ticks);
		}
	}
}
