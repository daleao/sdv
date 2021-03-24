using StardewModdingAPI.Events;

namespace TheLion.AwesomeProfessions
{
	internal abstract class ButtonsChangedEvent : BaseEvent
	{
		/// <summary>Construct an instance.</summary>
		internal ButtonsChangedEvent() { }

		/// <summary>Hook this event to an event listener.</summary>
		/// <param name="listener">Interface to the SMAPI event handler.</param>
		public override void Hook(IModEvents listener)
		{
			listener.Input.ButtonsChanged += OnButtonsChanged;
		}

		/// <summary>Unhook this event from an event listener.</summary>
		/// <param name="listener">Interface to the SMAPI event handler.</param>
		public override void Unhook(IModEvents listener)
		{
			listener.Input.ButtonsChanged -= OnButtonsChanged;
		}

		/// <summary>Raised after the player released a keyboard, mouse, or controller button.</summary>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event arguments.</param>
		public abstract void OnButtonsChanged(object sender, ButtonsChangedEventArgs e);
	}
}
