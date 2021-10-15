using HarmonyLib;
using StardewModdingAPI;
using StardewValley.Menus;
using System;
using System.Reflection;
using TheLion.Stardew.Common.Harmony;

namespace TheLion.Stardew.Professions.Framework.Patches
{
	internal class LevelUpMenuGetProfessionNamePatch : BasePatch
	{
		/// <summary>Construct an instance.</summary>
		internal LevelUpMenuGetProfessionNamePatch()
		{
			Original = typeof(LevelUpMenu).MethodNamed("getProfessionName");
			Prefix = new(GetType(), nameof(LevelUpMenuGetProfessionNamePrefix));
		}

		#region harmony patches

		/// <summary>Patch to apply modded profession names.</summary>
		[HarmonyPrefix]
		private static bool LevelUpMenuGetProfessionNamePrefix(ref string __result, int whichProfession)
		{
			try
			{
				if (!Util.Professions.IndexByName.Contains(whichProfession)) return true; // run original logic

				__result = Util.Professions.IndexByName.Reverse[whichProfession];
				return false; // don't run original logic
			}
			catch (Exception ex)
			{
				ModEntry.Log($"Failed in {MethodBase.GetCurrentMethod()?.Name}:\n{ex}", LogLevel.Error);
				return true; // default to original logic
			}
		}

		#endregion harmony patches
	}
}