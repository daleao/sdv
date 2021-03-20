using StardewModdingAPI.Events;

namespace TheLion.AwesomeProfessions
{
	public class SaveLoadedEvent : IEvent
	{
		/// <summary>Hook this event to an event listener.</summary>
		/// <param name="listener">Interface to the SMAPI event handler.</param>
		public void Hook(IModEvents listener)
		{
			listener.GameLoop.SaveLoaded += OnSaveLoaded;
		}

		/// <summary>Unhook this event from an event listener.</summary>
		/// <param name="listener">Interface to the SMAPI event handler.</param>
		public void Unhook(IModEvents listener)
		{
			listener.GameLoop.SaveLoaded -= OnSaveLoaded;
		}

		/// <summary>Raised after loading a save (including the first day after creating a new save), or connecting to a multiplayer world.</summary>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event data.</param>
		private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
		{
			AwesomeProfessions.Data = AwesomeProfessions.ModHelper.Data.ReadSaveData<ProfessionsData>("thelion.AwesomeProfessions") ?? new ProfessionsData();
			BasePatch.SetData(AwesomeProfessions.Data);
			AwesomeProfessions.ProspectorHunt = new ProspectorHunt(AwesomeProfessions.Config, AwesomeProfessions.Data, AwesomeProfessions.I18n, AwesomeProfessions.ModHelper.Content);
			AwesomeProfessions.ScavengerHunt = new ScavengerHunt(AwesomeProfessions.Config, AwesomeProfessions.Data, AwesomeProfessions.I18n, AwesomeProfessions.ModHelper.Content);
			AwesomeProfessions.EventManager.SubscribeProfessionEventsForLocalPlayer();
		}
	}
}
