using StardewModdingAPI.Events;

namespace TheLion.Stardew.Professions.Framework.Events
{
	public class StaticLevelChangedEvent : LevelChangedEvent
	{
		/// <summary>Raised after a player's skill level changes.</summary>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event arguments.</param>
		public override void OnLevelChanged(object sender, LevelChangedEventArgs e)
		{
			if (!e.IsLocalPlayer || e.NewLevel != 0) return;

			// clean up Poacher events and data on skill reset
			ModEntry.Subscriber.CleanUpRogueEvents();
			ModEntry.Data.CleanUpRogueDataFields();
		}
	}
}