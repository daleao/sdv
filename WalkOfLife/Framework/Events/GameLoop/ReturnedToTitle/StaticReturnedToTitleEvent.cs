using StardewModdingAPI.Events;

namespace TheLion.Stardew.Professions.Framework.Events
{
	public class StaticReturnedToTitleEvent : ReturnedToTitleEvent
	{
		/// <summary>Raised after the game returns to the title screen.</summary>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event data.</param>
		public override void OnReturnedToTitle(object sender, ReturnedToTitleEventArgs e)
		{
			// release mod data
			ModEntry.Data.Unload();
			
			// reset super mode
			if (ModEntry.SuperModeIndex > -1) ModEntry.SuperModeIndex = -1;

			// unsubscribe player's profession events
			ModEntry.Subscriber.UnsubscribeLocalPlayerEvents();
		}
	}
}