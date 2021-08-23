using Microsoft.Xna.Framework;
using System;

namespace TheLion.Stardew.Common.Extensions
{
	public static class Vector2Extensions
	{
		/// <summary>Rotates the calling Vector2 by t to a Vector2 by <paramref name="degrees"/>.</summary>
		public static Vector2 Rotate(this Vector2 v, double degrees)
		{
			float sin = (float)Math.Sin(degrees * Math.PI / 180);
			float cos = (float)Math.Cos(degrees * Math.PI / 180);

			float tx = v.X;
			float ty = v.Y;
			v.X = (cos * tx) - (sin * ty);
			v.Y = (sin * tx) + (cos * ty);
			return v;
		}
	}
}