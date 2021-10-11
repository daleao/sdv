using StardewModdingAPI.Events;

namespace TheLion.Stardew.Professions.Framework.Events
{
	public class StaticLevelChangedEvent : LevelChangedEvent
	{
		/// <inheritdoc />
		public override void OnLevelChanged(object sender, LevelChangedEventArgs e)
		{
			if (!e.IsLocalPlayer || e.NewLevel != 0) return;

			// clean up rogue events and data on skill reset
			ModEntry.Subscriber.CleanUpRogueEvents();
		}
	}
}