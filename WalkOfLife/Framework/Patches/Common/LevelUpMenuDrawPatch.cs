using HarmonyLib;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Menus;
using TheLion.Stardew.Common.Harmony;

namespace TheLion.Stardew.Professions.Framework.Patches
{
	internal class LevelUpMenuDrawPatch : BasePatch
	{
		/// <summary>Construct an instance.</summary>
		internal LevelUpMenuDrawPatch()
		{
			Original = typeof(LevelUpMenu).MethodNamed(nameof(LevelUpMenu.draw), new[] { typeof(SpriteBatch) });
			Prefix = new HarmonyMethod(GetType(), nameof(LevelUpMenuDrawPrefix));
		}

		/// <summary>Patch to increase the height of Level Up Menu to fit longer profession descriptions.</summary>
		[HarmonyPrefix]
		private static void LevelUpMenuDrawPrefix(LevelUpMenu __instance, int ___currentSkill, int ___currentLevel)
		{
			if (__instance.isProfessionChooser && ___currentLevel == 10)
				__instance.height += 108;
		}
	}
}
