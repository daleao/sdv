using StardewModdingAPI.Events;

namespace TheLion.AwesomeProfessions
{
	internal abstract class UpdateTickedEvent : BaseEvent
	{
		/// <summary>Construct an instance.</summary>
		internal UpdateTickedEvent() { }

		/// <summary>Hook this event to an event listener.</summary>
		/// <param name="listener">Interface to the SMAPI event handler.</param>
		public override void Hook(IModEvents listener)
		{
			listener.GameLoop.UpdateTicked += OnUpdateTicked;
		}

		/// <summary>Unhook this event from an event listener.</summary>
		/// <param name="listener">Interface to the SMAPI event handler.</param>
		public override void Unhook(IModEvents listener)
		{
			listener.GameLoop.UpdateTicked -= OnUpdateTicked;
		}

		/// <summary>Raised after the game state is updated.</summary>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event arguments.</param>
		public abstract void OnUpdateTicked(object sender, UpdateTickedEventArgs e);
	}
}
