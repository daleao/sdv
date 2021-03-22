using StardewModdingAPI.Events;

namespace TheLion.AwesomeProfessions
{
	internal class SaveLoadedEvent : BaseEvent
	{
		private EventManager _manager;

		/// <summary>Construct an instance.</summary>
		internal SaveLoadedEvent(EventManager manager)
		{
			_manager = manager;
		}

		/// <summary>Hook this event to an event listener.</summary>
		/// <param name="listener">Interface to the SMAPI event handler.</param>
		public override void Hook(IModEvents listener)
		{
			listener.GameLoop.SaveLoaded += OnSaveLoaded;
		}

		/// <summary>Unhook this event from an event listener.</summary>
		/// <param name="listener">Interface to the SMAPI event handler.</param>
		public override void Unhook(IModEvents listener)
		{
			listener.GameLoop.SaveLoaded -= OnSaveLoaded;
		}

		/// <summary>Raised after loading a save (including the first day after creating a new save), or connecting to a multiplayer world.</summary>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event data.</param>
		private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
		{
			// get persisted mod data
			AwesomeProfessions.Data = AwesomeProfessions.ModHelper.Data.ReadSaveData<ProfessionsData>("thelion.AwesomeProfessions") ?? new ProfessionsData();
			_data = AwesomeProfessions.Data;
			BasePatch.SetData(_data);
			Utility.SetData(_data);

			// start treasure hunt managers
			AwesomeProfessions.ProspectorHunt = new ProspectorHunt(_config, _data, _manager, AwesomeProfessions.I18n, AwesomeProfessions.ModHelper.Content);
			AwesomeProfessions.ScavengerHunt = new ScavengerHunt(_config, _data, _manager, AwesomeProfessions.I18n, AwesomeProfessions.ModHelper.Content);
			
			// hook events for loaded save
			_manager.SubscribeProfessionEventsForLocalPlayer();
		}
	}
}
