using StardewModdingAPI.Events;

namespace TheLion.AwesomeProfessions
{
	internal abstract class LevelChangedEvent : IEvent
	{
		/// <inheritdoc/>
		public void Hook()
		{
			AwesomeProfessions.Events.Player.LevelChanged += OnLevelChanged;
		}

		/// <inheritdoc/>
		public void Unhook()
		{
			AwesomeProfessions.Events.Player.LevelChanged -= OnLevelChanged;
		}

		/// <summary>Raised after a player's skill level changes.</summary>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event arguments.</param>
		public abstract void OnLevelChanged(object sender, LevelChangedEventArgs e);
	}
}