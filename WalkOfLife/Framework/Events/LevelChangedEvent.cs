using StardewModdingAPI.Events;
using StardewValley.Menus;

namespace TheLion.AwesomeProfessions
{
	internal class LevelChangedEvent : IEvent
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
		private void OnLevelChanged(object sender, LevelChangedEventArgs e)
		{
			// ensure immediate perks get removed on skill reset
			if (e.IsLocalPlayer && e.NewLevel == 0)
			{
				int first = (int)e.Skill * 6;
				int last = first + 5;
				for (int profession = first; profession <= last; ++profession)
					LevelUpMenu.removeImmediateProfessionPerk(profession);
			}
		}
	}
}