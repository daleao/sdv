using StardewModdingAPI.Events;

namespace TheLion.AwesomeProfessions
{
	internal class ScavengerDayStartedEvent : DayStartedEvent
	{
		/// <inheritdoc/>
		public override void OnDayStarted(object sender, DayStartedEventArgs e)
		{
			AwesomeProfessions.ScavengerHunt.ResetAccumulatedBonus();
		}
	}
}