using StardewModdingAPI.Events;

namespace TheLion.AwesomeProfessions
{
	internal abstract class WarpedEvent : BaseEvent
	{
		/// <summary>Construct an instance.</summary>
		internal WarpedEvent() { }

		/// <summary>Hook this event to an event listener.</summary>
		/// <param name="listener">Interface to the SMAPI event handler.</param>
		public override void Hook(IModEvents listener)
		{
			listener.Player.Warped += OnWarped;
		}

		/// <summary>Unhook this event from an event listener.</summary>
		/// <param name="listener">Interface to the SMAPI event handler.</param>
		public override void Unhook(IModEvents listener)
		{
			listener.Player.Warped -= OnWarped;
		}

		/// <summary>Raised after the current player moves to a new location.</summary>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event arguments.</param>
		public abstract void OnWarped(object sender, WarpedEventArgs e);
	}
}
