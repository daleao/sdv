using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xTile.Dimensions;
using xTile.Tiles;
using SObject = StardewValley.Object;

namespace TheLion.AwesomeProfessions
{
	internal class TreasureHunt
	{
		public static bool TheHuntIsOn { get; set; } = false;
		public static int ScavengerStreak { get; set; } = 0;
		public static int ProspectorStreak { get; set; } = 0;
		public static Vector2? TreasureTile { get; private set; } = null;

		public static void StartAt(Vector2? tile)
		{
			TreasureTile = tile;

			// choose reward
			// start monitoring distance
		}

		public static int ChooseReward()
		{
			return 0;
		}

		private static IEnumerable<int> _scavengerTreasureTable = new HashSet<int>
		{

		};

		private static IEnumerable<int> _prospectorTreasureTable = new HashSet<int>
		{

		};
	}
}