using HarmonyLib;
using StardewModdingAPI;
using StardewValley.Menus;
using System;
using System.Reflection;
using TheLion.Stardew.Common.Harmony;

namespace TheLion.Stardew.Professions.Framework.Patches
{
	internal class LevelUpMenuGetProfessionTitleFromNumberPatch : BasePatch
	{
		/// <summary>Construct an instance.</summary>
		internal LevelUpMenuGetProfessionTitleFromNumberPatch()
		{
			Original = typeof(LevelUpMenu).MethodNamed(nameof(LevelUpMenu.getProfessionTitleFromNumber));
			Prefix = new HarmonyMethod(GetType(), nameof(LevelUpMenuGetProfessionTitleFromNumberPrefix));
		}

		#region harmony patches

		/// <summary>Patch to apply modded profession names.</summary>
		[HarmonyPrefix]
		private static bool LevelUpMenuGetProfessionTitleFromNumberPrefix(ref string __result, int whichProfession)
		{
			try
			{
				if (!Util.Professions.IndexByName.Contains(whichProfession)) return true; // run original logic

				__result = ModEntry.I18n.Get(Util.Professions.NameOf(whichProfession) + ".name");
				return false; // don't run original logic
			}
			catch (Exception ex)
			{
				ModEntry.Log($"Failed in {MethodBase.GetCurrentMethod().Name}:\n{ex}", LogLevel.Error);
				return true; // default to original logic
			}
		}

		#endregion harmony patches
	}
}