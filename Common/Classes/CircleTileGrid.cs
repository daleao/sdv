using Microsoft.Xna.Framework;
using System.Collections;
using System.Collections.Generic;

namespace TheLion.Common
{
	public class CircleTileGrid : IEnumerable<Vector2>
	{
		private readonly Vector2 _origin;
		private readonly int _radius;
		private readonly bool[,] _outlineGrid;
		private readonly List<Vector2> _tiles = new();

		/// <summary>Construct an instance.</summary>
		/// <param name="origin">The center tile of the circle in the world reference.</param>
		/// <param name="radius">The radius of the circle in tile units.</param>
		public CircleTileGrid(Vector2 origin, int radius)
		{
			_origin = origin;
			_radius = radius;

			_outlineGrid = _GetBooleanOutlineGrid(_radius);
			_GetTiles();
		}

		/// <summary>Construct an instance.</summary>
		/// <param name="origin">The center tile of the circle in the world reference.</param>
		/// <param name="radius">The radius of the circle in tile units.</param>
		public CircleTileGrid(Vector2 origin, int radius, bool excludeOrigin = false)
		{
			_origin = origin;
			_radius = radius;

			_outlineGrid = _GetBooleanOutlineGrid(_radius);
			_GetTiles(excludeOrigin);
		}

		public IEnumerator<Vector2> GetEnumerator()
		{
			return _tiles.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return _tiles.GetEnumerator();
		}

		/// <summary>Get all the world tiles within a certain radius from an origin.</summary>
		/// <param name="origin">The origin of the circle in the world reference.</param>
		private void _GetTiles(bool excludeOrigin = false)
		{
			var center = new Vector2(_radius, _radius); // the center of the circle in the grid reference

			// get the central axes
			for (var i = 0; i < _radius * 2 + 1; ++i)
			{
				if (i != _radius)
				{
					_tiles.Add(_origin - center + new Vector2(i, _radius));
					_tiles.Add(_origin - center + new Vector2(_radius, i));
				}
			}

			// loop over the first remaining quadrant and mirror it 3 times
			for (var x = 0; x < _radius; ++x)
			{
				for (var y = 0; y < _radius; ++y)
				{
					if (_IsInsideOutlineGrid(new Point(x, y)))
					{
						_tiles.Add(_origin - center + new Vector2(y, x));
						_tiles.Add(_origin - center + new Vector2(y, 2 * _radius - x));
						_tiles.Add(_origin - center + new Vector2(2 * _radius - y, x));
						_tiles.Add(_origin - center + new Vector2(2 * _radius - y, 2 * _radius - x));
					}
				}
			}

			if (!excludeOrigin)
			{
				_tiles.Add(_origin);
			}
		}

		/// <summary>Create a circle outline boolean grid.</summary>
		/// <param name="radius">The radius of the circle.</param>
		private bool[,] _GetBooleanOutlineGrid(int radius)
		{
			var circleGrid = new bool[radius * 2 + 1, radius * 2 + 1];
			var f = 1 - radius;
			var ddF_x = 1;
			var ddF_y = -2 * radius;
			var x = 0;
			var y = radius;

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

		/// <summary>Determine whether a point in the grid reference intersects with the circle outline grid using the ray casting method.</summary>
		/// <param name="point">The point to be tested.</param>
		public bool _IsInsideOutlineGrid(Point point)
		{
			// handle out of bounds
			if (point.X < 0 || point.Y < 0 || point.X > _radius * 2 || point.Y > _radius * 2)
			{
				return false;
			}

			// handle edge points
			if (point.X == 0 || point.Y == 0 || point.X == _radius * 2 || point.Y == _radius * 2)
			{
				return _outlineGrid[point.Y, point.X];
			}

			// handle central axes
			if (point.X == _radius || point.Y == _radius)
			{
				return true;
			}

			// handle remaining outline points
			if (_outlineGrid[point.Y, point.X])
			{
				return true;
			}

			// mirror point into the first quadrant
			if (point.X > _radius)
			{
				point.X = _radius - point.X;
			}
			if (point.Y > _radius)
			{
				point.Y = _radius - point.Y;
			}

			// cast rays
			for (var i = point.X; i < _radius; ++i)
			{
				if (_outlineGrid[point.Y, i])
				{
					return false;
				}
			}

			return true;
		}
	}
}