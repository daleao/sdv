using StardewModdingAPI;
using StardewModdingAPI.Enums;
using StardewModdingAPI.Events;
using StardewValley.Menus;
using TheLion.Common.Extensions;

namespace TheLion.AwesomeProfessions
{
	public partial class AwesomeProfessions 
	{
		/// <summary>Raised after a player's skill level changes.</summary>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event arguments.</param>
		private void OnLevelChanged(object sender, LevelChangedEventArgs e)
		{
			// ensure immediate perks get removed on skill level reset
			if (e.IsLocalPlayer && e.Skill.AnyOf(SkillType.Combat, SkillType.Fishing) && e.NewLevel == 0) LevelUpMenu.RevalidateHealth(e.Player);
		}
	}
}
