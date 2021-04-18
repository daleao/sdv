using Harmony;
using StardewValley.Menus;
using System;

namespace TheLion.AwesomeProfessions
{
	internal class LevelUpMenuGetProfessionNamePatch : BasePatch
	{
		/// <inheritdoc/>
		public override void Apply(HarmonyInstance harmony)
		{
			harmony.Patch(
				original: AccessTools.Method(typeof(LevelUpMenu), name: "getProfessionName"),
				prefix: new HarmonyMethod(GetType(), nameof(LevelUpMenuGetProfessionNamePrefix))
			);
		}

		#region harmony patches

		/// <summary>Patch to apply modded profession names.</summary>
		private static bool LevelUpMenuGetProfessionNamePrefix(ref string __result, int whichProfession)
		{
			try
			{
				if (!Utility.ProfessionMap.Contains(whichProfession)) return true; // run original logic

				__result = Utility.ProfessionMap.Reverse[whichProfession];
				return false; // don't run original logic
			}
			catch (Exception ex)
			{
				Monitor.Log($"Failed in {nameof(LevelUpMenuGetProfessionNamePrefix)}:\n{ex}");
				return true; // default to original logic
			}
		}

		#endregion harmony patches
	}
}