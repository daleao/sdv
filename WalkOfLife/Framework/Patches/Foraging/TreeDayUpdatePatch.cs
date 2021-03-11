using Harmony;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.TerrainFeatures;
using System.Collections.Generic;
using TheLion.Common.Extensions;

namespace TheLion.AwesomeProfessions.Framework.Patches
{
	internal class TreeDayUpdatePatch : BasePatch
	{
		/// <summary>Construct an instance.</summary>
		/// <param name="config">The mod settings.</param>
		/// <param name="monitor">Interface for writing to the SMAPI console.</param>
		internal TreeDayUpdatePatch(ProfessionsConfig config, IMonitor monitor)
		: base(config, monitor) { }

		/// <summary>Apply internally-defined Harmony patches.</summary>
		/// <param name="harmony">The Harmony instance for this mod.</param>
		protected internal override void Apply(HarmonyInstance harmony)
		{
			harmony.Patch(
				AccessTools.Method(typeof(Tree), nameof(Tree.dayUpdate)),
				prefix: new HarmonyMethod(GetType(), nameof(TreeDayUpdatePrefix)),
				postfix: new HarmonyMethod(GetType(), nameof(TreeDayUpdatePostfix))
			);
		}

		/// <summary>Patch to increase Abrorist tree growth odds.</summary>
		protected static bool TreeDayUpdatePrefix(ref Tree __instance, ref int __state)
		{
			__state = __instance.growthStage.Value;
			return true; // run original logic
		}

		/// <summary>Patch to increase Abrorist non-fruit tree growth odds.</summary>
		protected static void TreeDayUpdatePostfix(ref Tree __instance, int __state, GameLocation environment, Vector2 tileLocation)
		{
			bool isThereAnyArborist = Utils.AnyPlayerHasProfession("arborist", out int n);
			if (__instance.growthStage.Value > __state || !isThereAnyArborist)
				return;

			if (_CanGrow(__instance, environment, tileLocation))
			{
				if (__instance.treeType.Value == Tree.mahoganyTree)
				{
					if (Game1.random.NextDouble() < 0.075 * n || (__instance.fertilized.Value && Game1.random.NextDouble() < 0.3 * n))
						++__instance.growthStage.Value;
				}
				else if (Game1.random.NextDouble() < 0.1 * n)
					++__instance.growthStage.Value;
			}
		}

		/// <summary>Whether a given common tree satisfies all conditions to advance a stage.</summary>
		/// <param name="tree">The given tree.</param>
		/// <param name="environment">The tree's game location.</param>
		/// <param name="tileLocation">The tree's tile location.</param>
		/// <returns></returns>
		private static bool _CanGrow(Tree tree, GameLocation environment, Vector2 tileLocation)
		{
			if (Game1.GetSeasonForLocation(tree.currentLocation).Equals("winter") && !tree.treeType.Value.AnyOf(Tree.palmTree, Tree.palmTree2) && !environment.CanPlantTreesHere(-1, (int)tileLocation.X, (int)tileLocation.Y) && !tree.fertilized.Value)
				return false;

			string s = environment.doesTileHaveProperty((int)tileLocation.X, (int)tileLocation.Y, "NoSpawn", "Back");
			if (s != null && (s.Equals("All") || s.Equals("Tree") || s.Equals("True")))
				return false;

			Rectangle growthRect = new Rectangle((int)((tileLocation.X - 1f) * 64f), (int)((tileLocation.Y - 1f) * 64f), 192, 192);
			if (tree.growthStage.Value == 4)
			{
				foreach (KeyValuePair<Vector2, TerrainFeature> kvp in environment.terrainFeatures.Pairs)
				{
					if (kvp.Value is Tree && !kvp.Value.Equals(tree) && (kvp.Value as Tree).growthStage.Value >= 5 && kvp.Value.getBoundingBox(kvp.Key).Intersects(growthRect))
						return false;
				}
			}
			else if (tree.growthStage.Value == 0 && environment.objects.ContainsKey(tileLocation))
				return false;

			return true;
		}
	}
}
