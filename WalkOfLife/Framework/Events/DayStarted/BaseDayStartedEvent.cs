using StardewModdingAPI.Events;

namespace TheLion.AwesomeProfessions
{
	public abstract class BaseDayStartedEvent : IEvent
	{
		/// <summary>Hook this event to an event listener.</summary>
		/// <param name="listener">Interface to the SMAPI event handler.</param>
		public void Hook(IModEvents listener)
		{
			listener.GameLoop.DayStarted += OnDayStarted;
		}

		/// <summary>Unhook this event from an event listener.</summary>
		/// <param name="listener">Interface to the SMAPI event handler.</param>
		public void Unhook(IModEvents listener)
		{
			listener.GameLoop.DayStarted -= OnDayStarted;
		}

		/// <summary>Raised after a new in-game day starts, or after connecting to a multiplayer world.</summary>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event arguments.</param>
		public abstract void OnDayStarted(object sender, DayStartedEventArgs e);
	}
}
