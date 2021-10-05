using Microsoft.Xna.Framework;
using System;

namespace TheLion.Stardew.Common.Extensions
{
	public static class Vector2Extensions
	{
		/// <summary>Rotates the calling Vector2 by t to a Vector2 by <paramref name="degrees"/>.</summary>
		public static Vector2 Rotate(this Vector2 v, double degrees)
		{
			var sin = (float)Math.Sin(degrees * Math.PI / 180);
			var cos = (float)Math.Cos(degrees * Math.PI / 180);

			var tx = v.X;
			var ty = v.Y;
			v.X = (cos * tx) - (sin * ty);
			v.Y = (sin * tx) + (cos * ty);
			return v;
		}
	}
}