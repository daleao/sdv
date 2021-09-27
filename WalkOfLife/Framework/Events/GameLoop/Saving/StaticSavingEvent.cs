using StardewModdingAPI.Events;

namespace TheLion.Stardew.Professions.Framework.Events
{
	public class StaticSavingEvent : SavingEvent
	{
		/// <inheritdoc/>
		public override void OnSaving(object sender, SavingEventArgs e)
		{
			// clean up rogue data
			ModEntry.Data.CleanUpRogueDataFields();
		}
	}
}