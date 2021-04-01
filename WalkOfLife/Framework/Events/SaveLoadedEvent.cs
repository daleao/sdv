using StardewModdingAPI.Events;
using StardewValley;

namespace TheLion.AwesomeProfessions
{
	internal class SaveLoadedEvent : IEvent
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
		private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
		{
			// load persisted mod data
			AwesomeProfessions.Data = Game1.player.modData;

			// verify mod data and initialize assets and helpers
			foreach (int professionIndex in Game1.player.professions) Utility.InitializeProfession(professionIndex);

			// subcribe events for loaded save
			AwesomeProfessions.EventManager.SubscribeProfessionEventsForLocalPlayer();
		}
	}
}