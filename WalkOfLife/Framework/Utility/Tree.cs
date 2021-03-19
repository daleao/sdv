using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.TerrainFeatures;
using System.Collections.Generic;
using TheLion.Common.Extensions;
using STree = StardewValley.TerrainFeatures.Tree;

namespace TheLion.AwesomeProfessions
{
	public static partial class Utility
	{
		/// <summary>Whether a given common tree satisfies all conditions to advance a stage.</summary>
		/// <param name="tree">The given tree.</param>
		/// <param name="environment">The tree's game location.</param>
		/// <param name="tileLocation">The tree's tile location.</param>
		public static bool CanThisTreeGrow(STree tree, GameLocation environment, Vector2 tileLocation)
		{
			if (Game1.GetSeasonForLocation(tree.currentLocation).Equals("winter") && !tree.treeType.Value.AnyOf(STree.palmTree, STree.palmTree2) && !environment.CanPlantTreesHere(-1, (int)tileLocation.X, (int)tileLocation.Y) && !tree.fertilized.Value)
				return false;

			string s = environment.doesTileHaveProperty((int)tileLocation.X, (int)tileLocation.Y, "NoSpawn", "Back");
			if (s != null && s.AnyOf("All", "Tree", "True")) return false;

			Rectangle growthRect = new Rectangle((int)((tileLocation.X - 1f) * 64f), (int)((tileLocation.Y - 1f) * 64f), 192, 192);
			if (tree.growthStage.Value == 4)
			{
				foreach (KeyValuePair<Vector2, TerrainFeature> kvp in environment.terrainFeatures.Pairs)
				{
					if (kvp.Value is STree && !kvp.Value.Equals(tree) && (kvp.Value as STree).growthStage.Value >= 5 && kvp.Value.getBoundingBox(kvp.Key).Intersects(growthRect))
						return false;
				}
			}
			else if (tree.growthStage.Value == 0 && environment.objects.ContainsKey(tileLocation)) return false;

			return true;
		}
	}
}
