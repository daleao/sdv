using StardewModdingAPI.Events;

namespace TheLion.Stardew.Professions.Framework.Events
{
	public class StaticDayEndingEvent : DayEndingEvent
	{
		/// <inheritdoc />
		public override void OnDayEnding(object sender, DayEndingEventArgs e)
		{
			// fix dumb shit with other mods
			ModEntry.Subscriber.CleanUpRogueEvents();
			ModEntry.Data.CleanUpRogueDataFields();
			ModEntry.Subscriber.SubscribeEventsForLocalPlayer();
			ModEntry.Data.InitializeDataFieldsForLocalPlayer();
		}
	}
}