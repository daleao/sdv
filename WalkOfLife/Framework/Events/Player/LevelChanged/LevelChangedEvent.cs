using StardewModdingAPI.Events;

namespace TheLion.Stardew.Professions.Framework.Events
{
	public abstract class LevelChangedEvent : BaseEvent
	{
		/// <inheritdoc/>
		public override void Hook()
		{
			ModEntry.Events.Player.LevelChanged += OnLevelChanged;
		}

		/// <inheritdoc/>
		public override void Unhook()
		{
			ModEntry.Events.Player.LevelChanged -= OnLevelChanged;
		}

		/// <summary>Raised after a player's skill level changes.</summary>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event arguments.</param>
		public abstract void OnLevelChanged(object sender, LevelChangedEventArgs e);
	}
}