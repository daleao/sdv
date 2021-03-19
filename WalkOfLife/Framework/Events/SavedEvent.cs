using StardewModdingAPI.Events;

namespace TheLion.AwesomeProfessions
{
	public class SavedEvent : IEvent
	{
		/// <summary>Hook this event to an event listener.</summary>
		/// <param name="listener">Interface to the SMAPI event handler.</param>
		public void Hook(IModEvents listener)
		{
			listener.GameLoop.Saved += OnSaved;
		}

		/// <summary>Unhook this event from an event listener.</summary>
		/// <param name="listener">Interface to the SMAPI event handler.</param>
		public void Unhook(IModEvents listener)
		{
			listener.GameLoop.Saved -= OnSaved;
		}

		/// <summary>Raised after the game writes data to save file (except the initial save creation).</summary>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event data.</param>
		private void OnSaved(object sender, SavedEventArgs e)
		{
			AwesomeProfessions.ModHelper.Data.WriteSaveData("thelion.AwesomeProfessions", AwesomeProfessions.Data);
		}
	}
}
