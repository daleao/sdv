using StardewModdingAPI.Events;

namespace TheLion.AwesomeProfessions
{
	internal class ProspectorDayStartedEvent : DayStartedEvent
	{
		/// <inheritdoc/>
		public override void OnDayStarted(object sender, DayStartedEventArgs e)
		{
			if (AwesomeProfessions.ProspectorHunt != null) AwesomeProfessions.ProspectorHunt.ResetAccumulatedBonus();
		}
	}
}