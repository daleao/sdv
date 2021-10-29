using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using StardewValley.TerrainFeatures;
using System.Collections.Generic;
using SObject = StardewValley.Object;

namespace TheLion.Stardew.Tools.Framework.Effects
{
	/// <summary>Applies Pickaxe-specific effects.</summary>
	internal class PickaxeEffect : BaseEffect
	{
		public Configs.PickaxeConfig Config { get; }

		/// <summary>The Pickaxe upgrade levels needed to break supported resource clumps.</summary>
		/// <remarks>Derived from <see cref="ResourceClump.performToolAction"/>.</remarks>
		private IDictionary<int, int> UpgradeLevelsNeededForResource { get; } = new Dictionary<int, int>
		{
			[ResourceClump.meteoriteIndex] = Tool.gold,
			[ResourceClump.boulderIndex] = Tool.steel
		};

		/// <summary>Construct an instance.</summary>
		/// <param name="config">The effect settings.</param>
		/// <param name="modRegistry">Metadata about loaded mods.</param>
		public PickaxeEffect(Configs.PickaxeConfig config, IModRegistry modRegistry)
			: base(modRegistry)
		{
			Config = config;
		}

		/// <inheritdoc/>
		public override bool Apply(Vector2 tile, SObject tileObj, TerrainFeature tileFeature, Tool tool, GameLocation location, Farmer who)
		{
			// clear debris
			if (Config.ClearDebris && (IsStone(tileObj) || IsWeed(tileObj)))
			{
				return UseToolOnTile(tool, tile, who, location);
			}

			// break mine containers
			if (Config.BreakMineContainers && tileObj is not null)
			{
				return TryBreakContainer(tile, tileObj, tool, location);
			}

			// clear placed objects
			if (Config.ClearObjects && tileObj is not null)
			{
				return UseToolOnTile(tool, tile, who, location);
			}

			// clear placed paths & flooring
			if (Config.ClearFlooring && tileFeature is Flooring)
			{
				return UseToolOnTile(tool, tile, who, location);
			}

			// clear bushes
			if (Config.ClearBushes && tileFeature is Bush)
			{
				return UseToolOnTile(tool, tile, who, location);
			}

			// handle dirt
			if (tileFeature is HoeDirt dirt)
			{
				// clear tilled dirt
				if (dirt.crop is null && Config.ClearDirt)
				{
					return UseToolOnTile(tool, tile, who, location);
				}
				
				// clear crops
				if (dirt.crop is not null)
				{
					if (Config.ClearDeadCrops && dirt.crop.dead.Value)
					{
						return UseToolOnTile(tool, tile, who, location);
					}
					
					if (Config.ClearLiveCrops && !dirt.crop.dead.Value)
					{
						return UseToolOnTile(tool, tile, who, location);
					}
				}
			}

			// clear boulders / meteorites
			if (Config.BreakBouldersAndMeteorites)
			{
				var clump = GetResourceClumpCoveringTile(location, tile, who, out var applyTool);
				if (clump is not null && (!UpgradeLevelsNeededForResource.TryGetValue(clump.parentSheetIndex.Value, out int requiredUpgradeLevel) || tool.UpgradeLevel >= requiredUpgradeLevel))
				{
					return applyTool(tool);
				}
			}

			// harvest spawned mine objects
			if (Config.HarvestMineSpawns && location is MineShaft && tileObj?.IsSpawnedObject == true && CheckTileAction(location, tile, who))
			{
				CancelAnimation(who, FarmerSprite.harvestItemDown, FarmerSprite.harvestItemLeft, FarmerSprite.harvestItemRight, FarmerSprite.harvestItemUp);
				return true;
			}

			return false;
		}

		/// <summary>Spreads the Pickaxe effect to an area around the player.</summary>
		/// <param name="tool">The tool selected by the player.</param>
		/// <param name="origin">The center of the shockwave (i.e. the Pickaxe's tile location).</param>
		/// <param name="multiplier">Stamina cost multiplier.</param>
		/// <param name="location">The player's location.</param>
		/// <param name="who">The player.</param>
		public void SpreadToolEffect(Tool tool, Vector2 origin, float multiplier, GameLocation location, Farmer who)
		{
			base.SpreadToolEffect(tool, origin, multiplier, location, who, Config.RadiusAtEachPowerLevel);
		}
	}
}