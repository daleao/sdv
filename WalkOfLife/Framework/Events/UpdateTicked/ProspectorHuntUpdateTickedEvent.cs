using StardewModdingAPI.Events;

namespace TheLion.AwesomeProfessions
{
	internal class ProspectorHuntUpdateTickedEvent : UpdateTickedEvent
	{
		/// <summary>Construct an instance.</summary>
		internal ProspectorHuntUpdateTickedEvent() { }

		/// <summary>Raised after the game state is updated. Handle Prospector hunt events.</summary>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event arguments.</param>
		public override void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
		{
			AwesomeProfessions.ProspectorHunt.Update(e.Ticks);
		}
	}
}
