using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using JetBrains.Annotations;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;

namespace TheLion.Stardew.Professions.Framework.Patches
{
	[UsedImplicitly]
	internal class LevelUpMenuAddProfessionDescriptionsPatch : BasePatch
	{
		/// <summary>Construct an instance.</summary>
		internal LevelUpMenuAddProfessionDescriptionsPatch()
		{
			Original = RequireMethod<LevelUpMenu>("addProfessionDescriptions");
			Prefix = new(AccessTools.Method(GetType(), nameof(LevelUpMenuAddProfessionDescriptionsPrefix)));
		}

		#region harmony patches

		/// <summary>Patch to apply modded profession descriptions.</summary>
		[HarmonyPrefix]
		private static bool LevelUpMenuAddProfessionDescriptionsPrefix(List<string> descriptions, string professionName)
		{
			try
			{
				if (!Utility.Professions.IndexByName.Contains(professionName)) return true; // run original logic

				descriptions.Add(ModEntry.ModHelper.Translation.Get(professionName + ".name." +
				                                                    (Game1.player.IsMale ? "male" : "female")));
				descriptions.AddRange(ModEntry.ModHelper.Translation.Get(professionName + ".desc").ToString()
					.Split('\n'));
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