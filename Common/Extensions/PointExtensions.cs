using Microsoft.Xna.Framework;

namespace TheLion.Common
{
	public static class PointExtensions
	{
		/// <summary>Convert the calling Point to a Vector2.</summary>
		public static Vector2 ToVector2(this Point p)
		{
			return new Vector2(p.X, p.Y);
		}
	}
}