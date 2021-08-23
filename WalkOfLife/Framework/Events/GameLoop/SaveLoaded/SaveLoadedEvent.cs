using StardewModdingAPI.Events;

namespace TheLion.Stardew.Professions.Framework.Events
{
	public abstract class SaveLoadedEvent : BaseEvent
	{
		/// <inheritdoc/>
		public override void Hook()
		{
			ModEntry.Events.GameLoop.SaveLoaded += OnSaveLoaded;
		}

		/// <inheritdoc/>
		public override void Unhook()
		{
			ModEntry.Events.GameLoop.SaveLoaded -= OnSaveLoaded;
		}

		/// <summary>Raised after loading a save (including the first day after creating a new save), or connecting to a multiplayer world.</summary>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event data.</param>
		public abstract void OnSaveLoaded(object sender, SaveLoadedEventArgs e);
	}
}