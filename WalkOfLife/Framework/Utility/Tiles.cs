using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Locations;
using System.Collections.Generic;
using TheLion.Common.Extensions;
using xTile.Dimensions;

namespace TheLion.AwesomeProfessions
{
	/// <summary>Holds common methods and properties related to map tilesheets.</summary>
	public static partial class Utility
	{
		/// <summary>Find all tiles in a mine map containing either a ladder or shaft.</summary>
		/// <param name="shaft">The MineShaft location.</param>
		/// <remarks>Credit to pomepome for this logic.</remarks>
		public static IEnumerable<Vector2> GetLadderTiles(MineShaft shaft)
		{
			for (int i = 0; i < shaft.Map.GetLayer("Buildings").LayerWidth; ++i)
			{
				for (int j = 0; j < shaft.Map.GetLayer("Buildings").LayerHeight; ++j)
				{
					int index = shaft.getTileIndexAt(new Point(i, j), "Buildings");
					if (index.AnyOf(173, 174)) yield return new Vector2(i, j);
				}
			}
		}

		/// <summary>Check if a tile on a map is valid for spawning diggable treasure.</summary>
		/// <param name="tile">The tile to check.</param>
		/// <param name="location">The game location.</param>
		public static bool IsTileValidForTreasure(Vector2 tile, GameLocation location)
		{
			string noSpawn = location.doesTileHaveProperty((int)tile.X, (int)tile.Y, "NoSpawn", "Back");
			if ((noSpawn != null && noSpawn != "") || !location.isTileLocationTotallyClearAndPlaceable(tile) || !IsTileClearOfDebris(tile, location) || location.isBehindBush(tile) || location.isBehindTree(tile))
				return false;

			return true;
		}

		/// <summary>Check if a tile is clear of debris.</summary>
		/// <param name="tile">The tile to check.</param>
		/// <param name="location">The game location.</param>
		public static bool IsTileClearOfDebris(Vector2 tile, GameLocation location)
		{
			foreach (Debris debris in location.debris)
			{
				if (debris.item != null && debris.Chunks.Count > 0)
				{
					Vector2 debrisTile = new Vector2((int)(debris.Chunks[0].position.X / Game1.tileSize) + 1, (int)(debris.Chunks[0].position.Y / Game1.tileSize) + 1);
					if (debrisTile == tile) return false;
				}
			}

			return true;
		}

		/// <summary>Force a tile to be affected by the hoe.</summary>
		/// <param name="tile">The tile to change.</param>
		/// <param name="location">The game location.</param>
		public static bool MakeTileDiggable(Vector2 tile, GameLocation location)
		{
			if (location.doesTileHaveProperty((int)tile.X, (int)tile.Y, "Diggable", "Back") == null)
			{
				Location digSpot = new Location((int)tile.X * Game1.tileSize, (int)tile.Y * Game1.tileSize);
				location.Map.GetLayer("Back").PickTile(digSpot, Game1.viewport.Size).Properties["Diggable"] = true;
				return false;
			}
			return true;
		}
	}
}
