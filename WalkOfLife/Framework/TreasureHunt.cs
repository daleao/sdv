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
		public static int ScavengedStreak { get; set; } = 0;
		public static int ProspectedStreak { get; set; } = 0;
		public Vector2? TreasureTile { get; set; } = null;

		private readonly GameLocation _location;
		private readonly Farmer _farmer;
		private readonly string _type;
		private uint _timer;

		private static IEnumerable<int> _scavengerTreasureTable = new HashSet<int>
		{

		};

		private static IEnumerable<int> _prospectorTreasureTable = new HashSet<int>
		{

		};

		public void TryStartTreasureHuntFor(string profession, Farmer who, GameLocation where)
		{
			Random r = new Random(where.GetHashCode() + (int)Game1.stats.DaysPlayed + Game1.timeOfDay);
		}
	}
}