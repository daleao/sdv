using StardewModdingAPI.Events;

namespace TheLion.Stardew.Professions.Framework.Events
{
	public class ProspectorHuntDayStartedEvent : DayStartedEvent
	{
		/// <inheritdoc/>
		public override void OnDayStarted(object sender, DayStartedEventArgs e)
		{
			if (ModEntry.ProspectorHunt != null) ModEntry.ProspectorHunt.ResetAccumulatedBonus();
		}
	}
}