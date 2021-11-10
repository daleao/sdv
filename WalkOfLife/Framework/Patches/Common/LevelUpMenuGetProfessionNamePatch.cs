using System;
using System.Reflection;
using HarmonyLib;
using JetBrains.Annotations;
using StardewModdingAPI;
using StardewValley.Menus;
using TheLion.Stardew.Common.Harmony;

namespace TheLion.Stardew.Professions.Framework.Patches
{
	[UsedImplicitly]
	internal class LevelUpMenuGetProfessionNamePatch : BasePatch
	{
		/// <summary>Construct an instance.</summary>
		internal LevelUpMenuGetProfessionNamePatch()
		{
			Original = RequireMethod<LevelUpMenu>("getProfessionName");
			Prefix = new(GetType().MethodNamed(nameof(LevelUpMenuGetProfessionNamePrefix)));
		}

		#region harmony patches

		/// <summary>Patch to apply modded profession names.</summary>
		[HarmonyPrefix]
		private static bool LevelUpMenuGetProfessionNamePrefix(ref string __result, int whichProfession)
		{
			try
			{
				if (!Utility.Professions.IndexByName.Contains(whichProfession)) return true; // run original logic

				__result = Utility.Professions.IndexByName.Reverse[whichProfession];
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