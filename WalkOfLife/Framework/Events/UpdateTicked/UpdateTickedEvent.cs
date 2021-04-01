using StardewModdingAPI.Events;

namespace TheLion.AwesomeProfessions
{
	internal abstract class UpdateTickedEvent : IEvent
	{
		/// <inheritdoc/>
		public void Hook()
		{
			AwesomeProfessions.Events.GameLoop.UpdateTicked += OnUpdateTicked;
		}

		/// <inheritdoc/>
		public void Unhook()
		{
			AwesomeProfessions.Events.GameLoop.UpdateTicked -= OnUpdateTicked;
		}

		/// <summary>Raised after the game state is updated.</summary>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event arguments.</param>
		public abstract void OnUpdateTicked(object sender, UpdateTickedEventArgs e);
	}
}