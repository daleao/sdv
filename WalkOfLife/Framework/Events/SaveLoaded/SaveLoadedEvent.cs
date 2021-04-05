using StardewModdingAPI.Events;

namespace TheLion.AwesomeProfessions
{
	internal abstract class SaveLoadedEvent : IEvent
	{
		/// <inheritdoc/>
		public void Hook()
		{
			AwesomeProfessions.Events.GameLoop.SaveLoaded += OnSaveLoaded;
		}

		/// <inheritdoc/>
		public void Unhook()
		{
			AwesomeProfessions.Events.GameLoop.SaveLoaded -= OnSaveLoaded;
		}

		/// <summary>Raised after loading a save (including the first day after creating a new save), or connecting to a multiplayer world.</summary>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event data.</param>
		public abstract void OnSaveLoaded(object sender, SaveLoadedEventArgs e);
	}
}