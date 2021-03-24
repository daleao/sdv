using StardewModdingAPI.Events;

namespace TheLion.AwesomeProfessions
{
	internal class ProspectorHuntUpdateTickedEvent : UpdateTickedEvent
	{
		private ProspectorHunt _Hunt { get; }

		/// <summary>Construct an instance.</summary>
		internal ProspectorHuntUpdateTickedEvent(ProspectorHunt hunt)
		{
			_Hunt = hunt;
		}

		/// <summary>Raised after the game state is updated. Handle Prospector hunt events.</summary>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event arguments.</param>
		public override void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
		{
			_Hunt.Update(e.Ticks);
		}
	}
}
