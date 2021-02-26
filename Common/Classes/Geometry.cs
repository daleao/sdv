using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace TheLion.Common.Classes.Geometry
{
	public static class Geometry
	{
		/// <summary>Get all the tiles within a certain radius of an origin. Optimized.</summary>
		/// <param name="origin">The origin of the circle in the game world reference.</param>
		/// <param name="radius">The radius of the circle.</param>
		public static IEnumerable<Vector2> GetTilesAround(Vector2 origin, int radius)
		{
			bool[,] outlineGrid = GetCircleOutlineGrid(radius);
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

		/// <summary>Get a boolean grid circle outline.</summary>
		/// <param name="radius">The radius of the circle.</param>
		private static bool[,]	GetCircleOutlineGrid(int radius)
		{
			bool[,] circleGrid = new bool[radius * 2 + 1, radius * 2 + 1];
			int f = 1 - radius;
			int ddF_x = 1;
			int ddF_y = -2 * radius;
			int x = 0;
			int y = radius;

			circleGrid[radius, radius + radius] = true;
			circleGrid[radius, radius - radius] = true;
			circleGrid[radius + radius, radius] = true;
			circleGrid[radius - radius, radius] = true;
			
			while (x < y)
			{
				if (f >= 0)
				{
					y--;
					ddF_y += 2;
					f += ddF_y;
				}

				x++;
				ddF_x += 2;
				f += ddF_x;

				circleGrid[radius + x, radius + y] = true;
				circleGrid[radius - x, radius + y] = true;
				circleGrid[radius + x, radius - y] = true;
				circleGrid[radius - x, radius - y] = true;
				circleGrid[radius + y, radius + x] = true;
				circleGrid[radius - y, radius + x] = true;
				circleGrid[radius + y, radius - x] = true;
				circleGrid[radius - y, radius - x] = true;
			}

			return circleGrid;
		}

		/// <summary>Determine whether a point belongs to the circle bound by a boolean grid using the ray casting method.</summary>
		/// <param name="p">The point to be tested.</param>
		/// <param name="outlineGrid">Boolean grid circle outline.</param>
		/// <param name="radius">The radius of the circle.</param>
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
