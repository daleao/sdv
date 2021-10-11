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
	internal class TreeDayUpdatePatch : BasePatch
	{
		/// <summary>Construct an instance.</summary>
		internal TreeDayUpdatePatch()
		{
			Original = typeof(Tree).MethodNamed(nameof(Tree.dayUpdate));
			Prefix = new HarmonyMethod(GetType(), nameof(TreeDayUpdatePrefix));
			Postfix = new HarmonyMethod(GetType(), nameof(TreeDayUpdatePostfix));
		}

		#region harmony patches

		/// <summary>Patch to increase Abrorist tree growth odds.</summary>
		[HarmonyPrefix]
		// ReSharper disable once RedundantAssignment
		private static bool TreeDayUpdatePrefix(Tree __instance, ref int __state)
		{
			__state = __instance.growthStage.Value;
			return true; // run original logic
		}

		/// <summary>Patch to increase Abrorist non-fruit tree growth odds.</summary>
		[HarmonyPostfix]
		private static void TreeDayUpdatePostfix(ref Tree __instance, int __state)
		{
			try
			{
				var anyPlayerIsArborist = Game1.game1.DoesAnyPlayerHaveProfession("Arborist", out var n);
				if (__instance.growthStage.Value > __state || !anyPlayerIsArborist || !__instance.CanGrow()) return;

				if (__instance.treeType.Value == Tree.mahoganyTree)
				{
					if (Game1.random.NextDouble() < 0.075 * n ||
						__instance.fertilized.Value && Game1.random.NextDouble() < 0.3 * n)
						++__instance.growthStage.Value;
				}
				else if (Game1.random.NextDouble() < 0.1 * n)
				{
					++__instance.growthStage.Value;
				}
			}
			catch (Exception ex)
			{
				ModEntry.Log($"Failed in {MethodBase.GetCurrentMethod().Name}:\n{ex}", LogLevel.Error);
			}
		}

		#endregion harmony patches
	}
}