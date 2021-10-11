using HarmonyLib;
using StardewModdingAPI;
using StardewValley;
using StardewValley.TerrainFeatures;
using System;
using System.Reflection;
using TheLion.Stardew.Common.Harmony;
using TheLion.Stardew.Professions.Framework.Extensions;

namespace TheLion.Stardew.Professions.Framework.Patches
{
	internal class FruitTreeDayUpdatePatch : BasePatch
	{
		/// <summary>Construct an instance.</summary>
		internal FruitTreeDayUpdatePatch()
		{
			Original = typeof(FruitTree).MethodNamed(nameof(FruitTree.dayUpdate));
			Postfix = new HarmonyMethod(GetType(), nameof(FruitTreeDayUpdatePostfix));
		}

		#region harmony patches

		/// <summary>Patch to increase Abrorist fruit tree growth speed.</summary>
		[HarmonyPostfix]
		private static void FruitTreeDayUpdatePostfix(ref FruitTree __instance)
		{
			try
			{
				if (Game1.game1.DoesAnyPlayerHaveProfession("Arborist", out _) &&
					__instance.daysUntilMature.Value % 4 == 0)
					--__instance.daysUntilMature.Value;
			}
			catch (Exception ex)
			{
				ModEntry.Log($"Failed in {MethodBase.GetCurrentMethod().Name}:\n{ex}", LogLevel.Error);
			}
		}

		#endregion harmony patches
	}
}