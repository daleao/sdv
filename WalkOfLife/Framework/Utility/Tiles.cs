using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Locations;
using System;
using System.Collections.Generic;
using TheLion.Common.Extensions;

namespace TheLion.AwesomeProfessions
{
	public static partial class Utility
	{
		/// <summary>Find all tiles in a mine map containing either a ladder or shaft.</summary>
		/// <param name="shaft">The MineShaft location.</param>
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

		/// <summary>Choose a random tile on a map.</summary>
		/// <param name="location"></param>
		/// <returns>Returns the tile vector if the chosen tile is valid for object placement, otherwise returns null.</returns>
		public static Vector2? ChooseTile(GameLocation location)
		{
			Random r = new Random(location.GetHashCode() + (int)Game1.stats.DaysPlayed - Game1.timeOfDay);
			int x = r.Next(location.Map.DisplayWidth / 64);
			int y = r.Next(location.Map.DisplayHeight / 64);
			Vector2 v = new Vector2(x, y);
			if (_IsTileValidForPlacement(v, location)) return v;
			
			return null;
		}

		/// <summary>Check if a tile on a map is valid for object placement.</summary>
		/// <param name="tile">The tile to check.</param>
		/// <param name="location">The game location.</param>
		private static bool _IsTileValidForPlacement(Vector2 tile, GameLocation location)
		{
			string noSpawn = location.doesTileHaveProperty((int)tile.X, (int)tile.Y, "NoSpawn", "Back");
			if ((noSpawn != null && noSpawn != "") || !location.isTileLocationTotallyClearAndPlaceable(tile) || !_IsTileClearOfDebris(tile, location))
				return false;
			
			return true;
		}

		/// <summary>Check if a tile is clear of debris.</summary>
		/// <param name="tile">The tile to check.</param>
		/// <param name="location">The game location.</param>
		private static bool _IsTileClearOfDebris(Vector2 tile, GameLocation location)
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

		/// <summary>Draw a forage arrow pointer over a tile.</summary>
		/// <param name="tile"></param>
		public static void DrawPointerOverTile(Vector2 tile, Color color)
		{
			Rectangle srcRect = new Rectangle(412, 495, 5, 4);
			float renderScale = 5f;
			Vector2 targetPixel = new Vector2((tile.X * 64f) + 32f, (tile.Y * 64f) + 32f) + new Vector2(0f, -33f);
			Vector2 adjustedPixel = Game1.GlobalToLocal(Game1.viewport, targetPixel);
			adjustedPixel = StardewValley.Utility.ModifyCoordinatesForUIScale(adjustedPixel);
			Game1.spriteBatch.Draw(Game1.mouseCursors, adjustedPixel, srcRect, color, (float)Math.PI, new Vector2(2f, 2f), renderScale, SpriteEffects.None, 1f);
		}
	}
}
