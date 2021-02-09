using System.Collections.Generic;
using Microsoft.Xna.Framework;

using StardewValley;

namespace TheLion.AwesomeTools.Framework
{
	/// <summary>Useful methods that don't fit anywhere specific.</summary>
	public static class Utils
	{
		/// <summary>Get all the tiles within a certain radius of an origin. Optimized.</summary>
		/// <param name="tile">The origin of the circle in the game world reference.</param>
		/// <param name="radius">The radius of the circle.</param>
		public static IEnumerable<Vector2> GetTilesAround(Vector2 origin, int radius)
		{
			bool[,] outlineGrid = Game1.getCircleOutlineGrid(radius);
			Vector2 center = new Vector2(radius, radius);

			// get the central axes
			for (int i = 0; i < radius * 2 + 1; ++i)
			{
				if (i != radius)
				{
					yield return origin - center + new Vector2(i, radius);
					yield return origin - center + new Vector2(radius, i);
				}
			}

			// loop over the first remaining quadrant and mirror it 3 times
			for (int x = 0; x < radius; ++x)
			{
				for (int y = 0; y < radius; ++y)
				{
					if (IsInsideCircleOutlineGrid(new Point(x, y), outlineGrid, radius))
					{
						yield return origin - center + new Vector2(y, x);
						yield return origin - center + new Vector2(y, 2 * radius - x);
						yield return origin - center + new Vector2(2 * radius - y, x);
						yield return origin - center + new Vector2(2 * radius - y, 2 * radius - x);
					}
				}
			}
		}

		/// <summary>Get all the tiles within a certain radius of an origin. Brute force version.</summary>
		//public static IEnumerable<Vector2> GetFullCircleTileGrid(Vector2 origin, int radius)
		//{
		//    bool[,] outlineGrid = Game1.getCircleOutlineGrid(radius);

		//    for (int x = 0; x < radius * 2 + 1; x++)
		//    {
		//        for (int y = 0; y < radius * 2 + 1; y++)
		//        {
		//            if (IsInsideOutlineGrid(new Point(x, y), outlineGrid))
		//            {
		//                yield return origin + new Vector2(y, x) - new Vector2(radius, radius);
		//            }
		//        }
		//    }
		//}

		/// <summary>Get whether a point belongs to the shape bound by a boolean grid using the ray casting method.</summary>
		/// <param name="p">The point to be tested.</param>
		/// <param name="outlineGrid">Boolean grid which delineates the shape.</param>
		private static bool IsInsideCircleOutlineGrid(Point p, bool[,] outlineGrid, int radius)
		{
			// handle edge points
			if (p.X == 0 || p.Y == 0 || p.X == radius * 2 || p.Y == radius * 2)
			{
				return outlineGrid[p.Y, p.X];
			}

			// handle central axes
			if (p.X == radius || p.Y == radius)
			{
				return true;
			}

			// handle remaining outline points
			if (outlineGrid[p.Y, p.X])
			{
				return true;
			}

			// mirror point into the first quadrant
			if (p.X > radius)
			{
				p.X = radius - p.X;
			}
			if (p.Y > radius)
			{
				p.Y = radius - p.Y;
			}

			// cast rays
			for (int i = p.X; i < radius; ++i)
			{
				if (outlineGrid[p.Y, i])
				{
					return false;
				}
			}
			return true;
		}
	}
}
