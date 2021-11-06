using StardewModdingAPI.Events;

namespace TheLion.Stardew.Professions.Framework.Events
{
	internal class ProspectorHuntDayStartedEvent : DayStartedEvent
	{
		/// <inheritdoc />
		public override void OnDayStarted(object sender, DayStartedEventArgs e)
		{
			if (ModEntry.ProspectorHunt is not null) ModEntry.ProspectorHunt.ResetAccumulatedBonus();
		}
	}
}