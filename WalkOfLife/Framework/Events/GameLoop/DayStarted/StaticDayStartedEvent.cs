using StardewModdingAPI.Events;

namespace TheLion.Stardew.Professions.Framework.Events
{
	public class StaticDayStartedEvent : DayStartedEvent
	{
		/// <inheritdoc/>
		public override void OnDayStarted(object sender, DayStartedEventArgs e)
		{
			ModEntry.Subscriber.SubscribeMissingEvents();
			ModEntry.Subscriber.CleanUpRogueEvents();
		}
	}
}