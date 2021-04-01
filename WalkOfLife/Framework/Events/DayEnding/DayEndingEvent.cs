using StardewModdingAPI.Events;

namespace TheLion.AwesomeProfessions
{
	internal abstract class DayEndingEvent : IEvent
	{
		/// <inheritdoc/>
		public void Hook()
		{
			AwesomeProfessions.Events.GameLoop.DayEnding += OnDayEnding;
		}

		/// <inheritdoc/>
		public void Unhook()
		{
			AwesomeProfessions.Events.GameLoop.DayEnding -= OnDayEnding;
		}

		/// <summary>Raised before the game ends the current day.</summary>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event arguments.</param>
		public abstract void OnDayEnding(object sender, DayEndingEventArgs e);
	}
}