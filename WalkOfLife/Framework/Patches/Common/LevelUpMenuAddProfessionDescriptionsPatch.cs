using Harmony;
using StardewValley.Menus;
using System.Collections.Generic;

namespace TheLion.AwesomeProfessions
{
	internal class LevelUpMenuAddProfessionDescriptionsPatch : BasePatch
	{
		/// <inheritdoc/>
		public override void Apply(HarmonyInstance harmony)
		{
			harmony.Patch(
				AccessTools.Method(typeof(LevelUpMenu), name: "addProfessionDescriptions"),
				prefix: new HarmonyMethod(GetType(), nameof(LevelUpMenuAddProfessionDescriptionsPrefix))
			);
		}

		#region harmony patches

		/// <summary>Patch to apply modded profession descriptions.</summary>
		private static bool LevelUpMenuAddProfessionDescriptionsPrefix(List<string> descriptions, string professionName)
		{
			if (!Utility.ProfessionMap.Contains(professionName)) return true; // run original logic

			descriptions.Add(AwesomeProfessions.I18n.Get(professionName + ".name"));
			descriptions.AddRange(AwesomeProfessions.I18n.Get(professionName + ".description").ToString().Split('\n'));
			return false; // don't run original logic
		}

		#endregion harmony patches
	}
}