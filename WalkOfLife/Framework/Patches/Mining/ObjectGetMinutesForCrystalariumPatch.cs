using HarmonyLib;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Reflection;
using TheLion.Stardew.Common.Harmony;
using TheLion.Stardew.Professions.Framework.Extensions;
using SObject = StardewValley.Object;

namespace TheLion.Stardew.Professions.Framework.Patches
{
	internal class ObjectGetMinutesForCrystalariumPatch : BasePatch
	{
		/// <summary>Construct an instance.</summary>
		internal ObjectGetMinutesForCrystalariumPatch()
		{
			Original = typeof(SObject).MethodNamed("getMinutesForCrystalarium");
			Postfix = new HarmonyMethod(GetType(), nameof(ObjectGetMinutesForCrystalariumPostfix));
		}

		#region harmony patches

		/// <summary>Patch to speed up crystalarium processing time for each Gemologist.</summary>
		[HarmonyPostfix]
		private static void ObjectGetMinutesForCrystalariumPostfix(SObject __instance, ref int __result)
		{
			try
			{
				var owner = Game1.getFarmer(__instance.owner.Value);
				if (owner.HasProfession("Gemologist")) __result = (int) (__result * 0.75);
			}
			catch (Exception ex)
			{
				ModEntry.Log($"Failed in {MethodBase.GetCurrentMethod().Name}:\n{ex}", LogLevel.Error);
			}
		}

		#endregion harmony patches
	}
}