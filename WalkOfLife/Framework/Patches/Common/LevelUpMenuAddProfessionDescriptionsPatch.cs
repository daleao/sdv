using HarmonyLib;
using StardewModdingAPI;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Reflection;
using TheLion.Stardew.Common.Harmony;

namespace TheLion.Stardew.Professions.Framework.Patches
{
	internal class LevelUpMenuAddProfessionDescriptionsPatch : BasePatch
	{
		/// <summary>Construct an instance.</summary>
		internal LevelUpMenuAddProfessionDescriptionsPatch()
		{
			Original = typeof(LevelUpMenu).MethodNamed(name: "addProfessionDescriptions");
			Prefix = new HarmonyMethod(GetType(), nameof(LevelUpMenuAddProfessionDescriptionsPrefix));
		}

		#region harmony patches

		/// <summary>Patch to apply modded profession descriptions.</summary>
		[HarmonyPrefix]
		private static bool LevelUpMenuAddProfessionDescriptionsPrefix(List<string> descriptions, string professionName)
		{
			try
			{
				if (!Util.Professions.IndexByName.Contains(professionName)) return true; // run original logic

				descriptions.Add(ModEntry.I18n.Get(professionName + ".name"));
				descriptions.AddRange(ModEntry.I18n.Get(professionName + ".desc").ToString()
					.Split('\n'));
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