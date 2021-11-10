using HarmonyLib;
using JetBrains.Annotations;
using StardewValley.Menus;
using TheLion.Stardew.Common.Harmony;

namespace TheLion.Stardew.Professions.Framework.Patches
{
	[UsedImplicitly]
	internal class SkillsPageCtorPatch : BasePatch
	{
		/// <summary>Construct an instance.</summary>
		internal SkillsPageCtorPatch()
		{
			Original = RequireConstructor<SkillsPage>(typeof(int), typeof(int), typeof(int), typeof(int));
			Postfix = new(GetType().MethodNamed(nameof(SkillsPageCtorPostfix)));
		}

		#region harmony patches

		/// <summary>Patch to increase the width of the skills page in the game menu to fit prestige ribbons.</summary>
		[HarmonyPostfix]
		private static void SkillsPageCtorPostfix(ref SkillsPage __instance)
		{
			__instance.width += 64;
		}

		#endregion harmony patches
	}
}