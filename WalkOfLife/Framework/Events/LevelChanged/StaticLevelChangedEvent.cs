using StardewModdingAPI.Events;
using StardewValley.Menus;

namespace TheLion.AwesomeProfessions
{
	internal class StaticLevelChangedEvent : LevelChangedEvent
	{
		/// <summary>Raised after a player's skill level changes.</summary>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event arguments.</param>
		public override void OnLevelChanged(object sender, LevelChangedEventArgs e)
		{
			if (!e.IsLocalPlayer || e.NewLevel != 0) return;

			// ensure immediate perks get removed on skill reset
			var first = (int)e.Skill * 6;
			var last = first + 5;
			for (var profession = first; profession <= last; ++profession) LevelUpMenu.removeImmediateProfessionPerk(profession);
		}
	}
}