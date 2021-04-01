using StardewModdingAPI.Events;

namespace TheLion.AwesomeProfessions
{
	internal class ReturnedToTitleEvent : IEvent
	{
		/// <inheritdoc/>
		public void Hook()
		{
			AwesomeProfessions.Events.GameLoop.ReturnedToTitle += OnReturnedToTitle;
		}

		/// <inheritdoc/>
		public void Unhook()
		{
			AwesomeProfessions.Events.GameLoop.ReturnedToTitle -= OnReturnedToTitle;
		}

		/// <summary>Raised after the game returns to the title screen.</summary>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event data.</param>
		private void OnReturnedToTitle(object sender, ReturnedToTitleEventArgs e)
		{
			AwesomeProfessions.EventManager.UnsubscribeLocalPlayerEvents();
		}
	}
}