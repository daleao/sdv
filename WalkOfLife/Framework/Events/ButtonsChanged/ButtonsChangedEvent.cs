using StardewModdingAPI.Events;

namespace TheLion.AwesomeProfessions
{
	internal abstract class ButtonsChangedEvent : IEvent
	{
		/// <inheritdoc/>
		public void Hook()
		{
			AwesomeProfessions.Events.Input.ButtonsChanged += OnButtonsChanged;
		}

		/// <inheritdoc/>
		public void Unhook()
		{
			AwesomeProfessions.Events.Input.ButtonsChanged -= OnButtonsChanged;
		}

		/// <summary>Raised after the player released a keyboard, mouse, or controller button.</summary>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event arguments.</param>
		public abstract void OnButtonsChanged(object sender, ButtonsChangedEventArgs e);
	}
}