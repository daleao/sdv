using Microsoft.Xna.Framework;
using StardewValley.Locations;
using System.Collections.Generic;
using TheLion.Common.Extensions;

namespace TheLion.AwesomeProfessions
{
	public static partial class Utility
	{
		/// <summary>Find any tiles containing either a ladder or shaft.</summary>
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
	}
}
