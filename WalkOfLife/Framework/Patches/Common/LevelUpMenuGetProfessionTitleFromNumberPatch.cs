using System;
using System.Reflection;
using HarmonyLib;
using JetBrains.Annotations;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;

namespace TheLion.Stardew.Professions.Framework.Patches
{
	[UsedImplicitly]
	internal class LevelUpMenuGetProfessionTitleFromNumberPatch : BasePatch
	{
		/// <summary>Construct an instance.</summary>
		internal LevelUpMenuGetProfessionTitleFromNumberPatch()
		{
			Original = RequireMethod<LevelUpMenu>(nameof(LevelUpMenu.getProfessionTitleFromNumber));
		}

		#region harmony patches

		/// <summary>Patch to apply modded profession names.</summary>
		[HarmonyPrefix]
		private static bool LevelUpMenuGetProfessionTitleFromNumberPrefix(ref string __result, int whichProfession)
		{
			try
			{
				if (!Utility.Professions.IndexByName.Contains(whichProfession)) return true; // run original logic

				__result = ModEntry.ModHelper.Translation.Get(Utility.Professions.NameOf(whichProfession) + ".name." +
				                                              (Game1.player.IsMale ? "male" : "female"));
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