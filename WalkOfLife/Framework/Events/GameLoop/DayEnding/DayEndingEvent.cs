using StardewModdingAPI.Events;

namespace TheLion.Stardew.Professions.Framework.Events
{
	public abstract class DayEndingEvent : BaseEvent
	{
		/// <inheritdoc/>
		public override void Hook()
		{
			ModEntry.Events.GameLoop.DayEnding += OnDayEnding;
		}

		/// <inheritdoc/>
		public override void Unhook()
		{
			ModEntry.Events.GameLoop.DayEnding -= OnDayEnding;
		}

		/// <summary>Raised before the game ends the current day.</summary>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event arguments.</param>
		public abstract void OnDayEnding(object sender, DayEndingEventArgs e);
	}
}