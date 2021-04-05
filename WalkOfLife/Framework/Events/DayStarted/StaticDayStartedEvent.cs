using StardewModdingAPI.Events;

namespace TheLion.AwesomeProfessions
{
	internal class StaticDayStartedEvent : DayStartedEvent
	{
		/// <inheritdoc/>
		public override void OnDayStarted(object sender, DayStartedEventArgs e)
		{
			AwesomeProfessions.EventManager.SubscribeMissingEvents();
		}
	}
}