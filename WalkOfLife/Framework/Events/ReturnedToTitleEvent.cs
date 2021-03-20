using StardewModdingAPI.Events;

namespace TheLion.AwesomeProfessions
{
	public class ReturnedToTitleEvent : IEvent
	{
		/// <summary>Hook this event to an event listener.</summary>
		/// <param name="listener">Interface to the SMAPI event handler.</param>
		public void Hook(IModEvents listener)
		{
			listener.GameLoop.ReturnedToTitle += OnReturnedToTitle;
		}

		/// <summary>Unhook this event from an event listener.</summary>
		/// <param name="listener">Interface to the SMAPI event handler.</param>
		public void Unhook(IModEvents listener)
		{
			listener.GameLoop.ReturnedToTitle -= OnReturnedToTitle;
		}

		/// <summary>Raised after the game returns to the title screen.</summary>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event data.</param>
		private void OnReturnedToTitle(object sender, ReturnedToTitleEventArgs e)
		{
			AwesomeProfessions.EventManager.UnsubscribeLocalPlayerEvents();
		}
	}
}
