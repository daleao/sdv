using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.TerrainFeatures;
using System;
using System.Reflection;
using TheLion.Stardew.Common.Extensions;
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
		// ReSharper disable once RedundantAssignment
		[HarmonyPrefix]
		private static bool TreeDayUpdatePrefix(Tree __instance, ref int __state)
		{
			__state = __instance.growthStage.Value;
			return true; // run original logic
		}

		/// <summary>Patch to increase Abrorist non-fruit tree growth odds.</summary>
		[HarmonyPostfix]
		private static void TreeDayUpdatePostfix(ref Tree __instance, int __state, GameLocation environment,
			Vector2 tileLocation)
		{
			try
			{
				var anyPlayerIsArborist = Game1.game1.DoesAnyPlayerHaveProfession("Arborist", out var n);
				if (__instance.growthStage.Value > __state || !anyPlayerIsArborist ||
				    !CanThisTreeGrow(__instance, environment, tileLocation)) return;

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

		#region private methods

		/// <summary>Whether a given common tree satisfies all conditions to advance a stage.</summary>
		/// <param name="tree">The given tree.</param>
		/// <param name="environment">The tree's game location.</param>
		/// <param name="tileLocation">The tree's tile location.</param>
		private static bool CanThisTreeGrow(Tree tree, GameLocation environment, Vector2 tileLocation)
		{
			if (Game1.GetSeasonForLocation(tree.currentLocation) == "winter" &&
			    !tree.treeType.Value.AnyOf(Tree.palmTree, Tree.palmTree2) &&
			    !environment.CanPlantTreesHere(-1, (int) tileLocation.X, (int) tileLocation.Y) &&
			    !tree.fertilized.Value)
				return false;

			var s = environment.doesTileHaveProperty((int) tileLocation.X, (int) tileLocation.Y, "NoSpawn", "Back");
			if (s != null && s.AnyOf("All", "Tree", "True")) return false;

			var growthRect = new Rectangle((int) ((tileLocation.X - 1f) * 64f), (int) ((tileLocation.Y - 1f) * 64f),
				192, 192);
			if (tree.growthStage.Value == 4)
			{
				foreach (var pair in environment.terrainFeatures.Pairs)
					if (pair.Value is Tree value && !value.Equals(tree) && value.growthStage.Value >= 5 &&
					    value.getBoundingBox(pair.Key).Intersects(growthRect))
						return false;
			}
			else if (tree.growthStage.Value == 0 && environment.objects.ContainsKey(tileLocation))
			{
				return false;
			}

			return true;
		}

		#endregion private methods
	}
}