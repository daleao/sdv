using StardewModdingAPI.Events;

namespace TheLion.Stardew.Professions.Framework.Events
{
	public class StaticSaveLoadedEvent : SaveLoadedEvent
	{
		/// <summary>Raised after loading a save (including the first day after creating a new save), or connecting to a multiplayer world.</summary>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event data.</param>
		public override void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
		{
			// load persisted mod data
			ModEntry.Data.Load();

			// set super mode
			ModEntry.SuperModeIndex = ModEntry.Data.ReadField<int>("SuperModeIndex");

			// subcribe player's profession events
			ModEntry.Subscriber.SubscribeEventsForLocalPlayer();
		}
	}
}