using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.TerrainFeatures;
using System.Collections.Generic;
using System.Linq;
using SObject = StardewValley.Object;

namespace TheLion.Stardew.Tools.Framework.Effects
{
	/// <summary>Applies Axe-specific effects.</summary>
	internal class AxeEffect : BaseEffect
	{
		public Configs.AxeConfig Config { get; }

		/// <summary>The Axe upgrade levels needed to break supported resource clumps.</summary>
		/// <remarks>Derived from <see cref="ResourceClump.performToolAction"/>.</remarks>
		private IDictionary<int, int> UpgradeLevelsNeededForResource { get; } = new Dictionary<int, int>
		{
			[ResourceClump.stumpIndex] = Tool.copper,
			[ResourceClump.hollowLogIndex] = Tool.steel
		};

		/// <summary>Construct an instance.</summary>
		/// <param name="config">The effect settings.</param>
		/// <param name="modRegistry">Metadata about loaded mods.</param>
		public AxeEffect(Configs.AxeConfig config, IModRegistry modRegistry)
			: base(modRegistry)
		{
			Config = config;
		}

		/// <inheritdoc/>
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
				if (Config.ClearDebris && clump != null && UpgradeLevelsNeededForResource.ContainsKey(clump.parentSheetIndex.Value) && tool.UpgradeLevel >= UpgradeLevelsNeededForResource[clump.parentSheetIndex.Value])
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

		/// <summary>Spreads the Axe effect to an area around the player.</summary>
		/// <param name="tool">The tool selected by the player.</param>
		/// <param name="origin">The center of the shockwave (i.e. the Axe's tile location).</param>
		/// <param name="multiplier">Stamina cost multiplier.</param>
		/// <param name="location">The player's location.</param>
		/// <param name="who">The player.</param>
		public void SpreadToolEffect(Tool tool, Vector2 origin, float multiplier, GameLocation location, Farmer who)
		{
			base.SpreadToolEffect(tool, origin, multiplier, location, who, Config.RadiusAtEachPowerLevel);
		}

		/// <summary>Get whether a given tree should be chopped.</summary>
		/// <param name="tree">The tree to check.</param>
		private bool ShouldCut(Tree tree)
		{

			return tree.growthStage.Value switch
			{
				Tree.seedStage => Config.ClearTreeSeeds, // seed
				< Tree.treeStage => Config.ClearTreeSaplings, // sapling
				_ => tree.tapped.Value ? Config.CutTappedTrees : Config.CutGrownTrees // full-ground
			};
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