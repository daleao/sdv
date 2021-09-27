using HarmonyLib;
using StardewValley.Menus;
using TheLion.Stardew.Common.Extensions;
using TheLion.Stardew.Common.Harmony;

namespace TheLion.Stardew.Professions.Framework.Patches
{
	internal class SkillsPagePerformHoverActionPatch : BasePatch
	{
		/// <summary>Construct an instance.</summary>
		internal SkillsPagePerformHoverActionPatch()
		{
			Original = typeof(SkillsPage).MethodNamed(nameof(SkillsPage.performHoverAction));
			Postfix = new HarmonyMethod(GetType(), nameof(SkillsPagePerformHoverActionPostfix));
		}

		#region harmony patches

		/// <summary>Patch to truncate profession descriptions in hover menu.</summary>
		[HarmonyPostfix]
		private static void SkillsPagePerformHoverActionPostfix(ref string ___hoverText)
		{
			___hoverText = ___hoverText.Truncate(90);
		}

		#endregion harmony patches
	}
}