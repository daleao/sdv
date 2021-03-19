using StardewModdingAPI.Events;

namespace TheLion.AwesomeProfessions
{
	public class ArrowPointerUpdateTickedEvent : BaseUpdateTickedEvent
	{
		/// <summary>Raised after the game state is updated. Update tracking pointer offset for Prospector and Scavenger.</summary>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event arguments.</param>
		public override void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
		{
			if (e.Ticks % 4 == 0) Utility.ArrowPointer.Bob();
		}
	}
}
