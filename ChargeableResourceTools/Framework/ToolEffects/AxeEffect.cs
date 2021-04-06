using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.TerrainFeatures;
using System.Collections.Generic;
using System.Linq;
using SObject = StardewValley.Object;

namespace TheLion.AwesomeTools
{
	/// <summary>Applies Axe-specific effects.</summary>
	internal class AxeEffect : BaseEffect
	{
		public AxeConfig Config { get; }

		/// <summary>The Axe upgrade levels needed to break supported resource clumps.</summary>
		/// <remarks>Derived from <see cref="ResourceClump.performToolAction"/>.</remarks>
		private IDictionary<int, int> _UpgradeLevelsNeededForResource { get; } = new Dictionary<int, int>
		{
			[ResourceClump.stumpIndex] = Tool.copper,
			[ResourceClump.hollowLogIndex] = Tool.steel
		};

		/// <summary>Construct an instance.</summary>
		/// <param name="config">The effect settings.</param>
		/// <param name="modRegistry">Metadata about loaded mods.</param>
		public AxeEffect(AxeConfig config, IModRegistry modRegistry)
			: base(modRegistry)
		{
			Config = config;
		}

		/// <summary>Apply the tool effect to the given tile.</summary>
		/// <param name="tile">The tile to modify.</param>
		/// <param name="tileObj">The object on the tile.</param>
		/// <param name="tileFeature">The feature on the tile.</param>
		/// <param name="tool">The tool selected by the player.</param>
		/// <param name="location">The current location.</param>
		/// <param name="who">The current player.</param>
		public override bool Apply(Vector2 tile, SObject tileObj, TerrainFeature tileFeature, Tool tool, GameLocation location, Farmer who)
		{
			// clear debris
			if (Config.ClearDebris && (IsTwig(tileObj) || IsWeed(tileObj)))
				return UseToolOnTile(tool, tile, who, location);

			// cut terrain features
			switch (tileFeature)
			{
				// cut non-fruit tree
				case Tree tree:
					return ShouldCut(tree) && UseToolOnTile(tool, tile, who, location);

				// cut fruit tree
				case FruitTree tree:
					return ShouldCut(tree) && UseToolOnTile(tool, tile, who, location);

				// cut bushes
				case Bush bush:
					return ShouldCut(bush) && UseToolOnTile(tool, tile, who, location);

				// clear crops
				case HoeDirt dirt when dirt.crop != null:
					if (Config.ClearDeadCrops && dirt.crop.dead.Value)
					{
						return UseToolOnTile(tool, tile, who, location);
					}
					else if (Config.ClearLiveCrops && !dirt.crop.dead.Value)
					{
						return UseToolOnTile(tool, tile, who, location);
					}
					break;
			}

			// cut resource stumps
			if (Config.ClearDebris || Config.CutGiantCrops)
			{
				ResourceClump clump = GetResourceClumpCoveringTile(location, tile, who, out var applyTool);

				// giant crops
				if (Config.CutGiantCrops && clump is GiantCrop)
				{
					return applyTool(tool);
				}

				// big stumps and fallen logs
				if (Config.ClearDebris && clump != null && _UpgradeLevelsNeededForResource.ContainsKey(clump.parentSheetIndex.Value) && tool.UpgradeLevel >= _UpgradeLevelsNeededForResource[clump.parentSheetIndex.Value])
				{
					return applyTool(tool);
				}
			}

			// cut bushes in large terrain features
			if (Config.ClearBushes)
			{
				if (location.largeTerrainFeatures.OfType<Bush>().Any(b => b.tilePosition.Value == tile))
				{
					return UseToolOnTile(tool, tile, who, location);
				}
			}

			return false;
		}

		/// <summary>Get whether a given tree should be chopped.</summary>
		/// <param name="tree">The tree to check.</param>
		private bool ShouldCut(Tree tree)
		{
			// seed
			if (tree.growthStage.Value == Tree.seedStage)
				return Config.ClearTreeSeeds;

			// sapling
			if (tree.growthStage.Value < Tree.treeStage)
				return Config.ClearTreeSaplings;

			// full-grown
			return tree.tapped.Value ? Config.CutTappedTrees : Config.CutGrownTrees;
		}

		/// <summary>Get whether a given tree should be chopped.</summary>
		/// <param name="tree">The tree to check.</param>
		private bool ShouldCut(FruitTree tree)
		{
			return tree.growthStage.Value switch
			{
				Tree.seedStage => Config.ClearFruitTreeSeeds,       // seed
				< Tree.treeStage => Config.ClearFruitTreeSaplings,  // sapling
				_ => Config.CutGrownFruitTrees                      // full-grown
			};
		}

		/// <summary>Get whether bushes should be chopped.</summary>
		/// <param name="bush">A bush.</param>
		private bool ShouldCut(Bush bush)
		{
			return Config.ClearBushes;
		}
	}
}