using StardewModdingAPI.Events;

namespace TheLion.AwesomeProfessions
{
	public abstract class BaseUpdateTickedEvent : IEvent
	{
		/// <summary>Hook this event to an event listener.</summary>
		/// <param name="listener">Interface to the SMAPI event handler.</param>
		public void Hook(IModEvents listener)
		{
			listener.GameLoop.UpdateTicked += OnUpdateTicked;
		}

		/// <summary>Unhook this event from an event listener.</summary>
		/// <param name="listener">Interface to the SMAPI event handler.</param>
		public void Unhook(IModEvents listener)
		{
			listener.GameLoop.UpdateTicked -= OnUpdateTicked;
		}

		/// <summary>Raised after the game state is updated.</summary>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event arguments.</param>
		public abstract void OnUpdateTicked(object sender, UpdateTickedEventArgs e);
	}
}
