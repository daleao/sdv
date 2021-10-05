using HarmonyLib;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Reflection;
using TheLion.Stardew.Common.Harmony;
using SObject = StardewValley.Object;

namespace TheLion.Stardew.Professions.Framework.Patches
{
	internal class ObjectLoadDisplayNamePatch : BasePatch
	{
		/// <summary>Construct an instance.</summary>
		internal ObjectLoadDisplayNamePatch()
		{
			Original = typeof(SObject).MethodNamed("loadDisplayName");
			Postfix = new HarmonyMethod(GetType(), nameof(ObjectLoadDisplayNamePostfix));
		}

		/// <summary>Patch to add honey-specific mead names.</summary>
		[HarmonyPostfix]
		private static void ObjectLoadDisplayNamePostfix(SObject __instance, ref string __result)
		{
			try
			{
				if (!__instance.name.Contains("Mead") || __instance.preservedParentSheetIndex.Value <= 0) return;

				var prefix = Game1.objectInformation[__instance.preservedParentSheetIndex.Value].Split('/')[4];
				__result = prefix + " " + __result;
			}
			catch (Exception ex)
			{
				ModEntry.Log($"Failed in {MethodBase.GetCurrentMethod().Name}:\n{ex}", LogLevel.Error);
			}
		}
	}
}