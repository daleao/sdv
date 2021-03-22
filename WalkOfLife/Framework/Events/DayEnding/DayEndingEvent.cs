using StardewModdingAPI.Events;

namespace TheLion.AwesomeProfessions
{
	internal abstract class DayEndingEvent : BaseEvent
	{
		/// <summary>Construct an instance.</summary>
		internal DayEndingEvent() { }

		/// <summary>Hook this event to an event listener.</summary>
		/// <param name="listener">Interface to the SMAPI event handler.</param>
		public override void Hook(IModEvents listener)
		{
			listener.GameLoop.DayEnding += OnDayEnding;
		}

		/// <summary>Unhook this event from an event listener.</summary>
		/// <param name="listener">Interface to the SMAPI event handler.</param>
		public override void Unhook(IModEvents listener)
		{
			listener.GameLoop.DayEnding -= OnDayEnding;
		}

		/// <summary>Raised before the game ends the current day.</summary>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event arguments.</param>
		public abstract void OnDayEnding(object sender, DayEndingEventArgs e);
	}
}
