using StardewModdingAPI.Events;

namespace TheLion.Stardew.Professions.Framework.Events
{
	internal class ScavengerHuntDayStartedEvent : DayStartedEvent
	{
		/// <inheritdoc />
		public override void OnDayStarted(object sender, DayStartedEventArgs e)
		{
			if (ModEntry.ScavengerHunt is not null) ModEntry.ScavengerHunt.ResetAccumulatedBonus();
		}
	}
}