using StardewModdingAPI.Events;

namespace TheLion.AwesomeProfessions
{
	public abstract class BaseWarpedEvent : IEvent
	{
		/// <summary>Hook this event to an event listener.</summary>
		/// <param name="listener">Interface to the SMAPI event handler.</param>
		public void Hook(IModEvents listener)
		{
			listener.Player.Warped += OnWarped;
		}

		/// <summary>Unhook this event from an event listener.</summary>
		/// <param name="listener">Interface to the SMAPI event handler.</param>
		public void Unhook(IModEvents listener)
		{
			listener.Player.Warped -= OnWarped;
		}

		/// <summary>Raised after the current player moves to a new location.</summary>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event arguments.</param>
		public abstract void OnWarped(object sender, WarpedEventArgs e);
	}
}
