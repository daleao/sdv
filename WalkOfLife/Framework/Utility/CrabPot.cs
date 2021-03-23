using StardewValley;
using StardewValley.Objects;
using StardewValley.Locations;
using System;
using System.Collections.Generic;
using System.Linq;
using TheLion.Common.Extensions;
using SUtility = StardewValley.Utility;

namespace TheLion.AwesomeProfessions
{
	public static partial class Utility
	{
		#region look-up tables
		/// <summary>Look-up table for different types of bait by id.</summary>
		private static readonly Dictionary<int, string> _baitById = new()
		{
			{ 685, "Bait" },
			{ 703, "Magnet" },
			{ 774, "Wild Bait" },
			{ 908, "Magic Bait" }
		};

		/// <summary>Look-up table for trappable treasure items using magnet.</summary>
		private static readonly Dictionary<int, string[]> _pirateTreasureTable = new()
		{
			{ 14, new string[] { "1.003", "1", "1" } },		// neptune's glaive
			{ 51, new string[] { "1.003", "1", "1" } },		// broken trident
			{ 166, new string[] { "0.03", "1", "1" } },		// treasure chest
			{ 109, new string[] { "0.009", "1", "1" } },	// ancient sword
			{ 110, new string[] { "0.009", "1", "1" } },	// rusty spoon
			{ 111, new string[] { "0.009", "1", "1" } },	// rusty spur
			{ 112, new string[] { "0.009", "1", "1" } },	// rusty cog
			{ 117, new string[] { "0.009", "1", "1" } },	// anchor
			{ 378, new string[] { "0.39", "1", "24" } },	// copper ore
			{ 380, new string[] { "0.24", "1", "24" } },	// iron ore
			{ 384, new string[] { "0.12", "1", "24" } },	// gold ore
			{ 386, new string[] { "0.065", "1", "2" } },	// iridium ore
			{ 516, new string[] { "0.024", "1", "1" } },	// small glow ring
			{ 517, new string[] { "1.009", "1", "1" } },	// glow ring
			{ 518, new string[] { "0.024", "1", "1" } },	// small magnet ring
			{ 519, new string[] { "1.009", "1", "1" } },	// magnet ring
			{ 527, new string[] { "0.005", "1", "1" } },	// iridum band
			{ 529, new string[] { "0.005", "1", "1" } },	// amethyst ring
			{ 530, new string[] { "0.005", "1", "1" } },	// topaz ring
			{ 531, new string[] { "0.005", "1", "1" } },	// aquamarine ring
			{ 532, new string[] { "0.005", "1", "1" } },	// jade ring
			{ 533, new string[] { "0.005", "1", "1" } },	// emerald ring
			{ 534, new string[] { "0.005", "1", "1" } },	// ruby ring
			{ 890, new string[] { "0.03", "1", "3" } }		// qi bean
		};
		#endregion look-up tables

		/// <summary>Whether the crab pot instance is using regular bait.</summary>
		/// <param name="crabpot">The crab pot instance.</param>
		public static bool IsUsingRegularBait(CrabPot crabpot)
		{
			return crabpot.bait.Value != null && _baitById.TryGetValue(crabpot.bait.Value.ParentSheetIndex, out string baitName) && baitName.Equals("Bait");
		}

		/// <summary>Whether the crab pot instance is using magnet as bait.</summary>
		/// <param name="crabpot">The crab pot instance.</param>
		public static bool IsUsingMagnet(CrabPot crabpot)
		{
			return crabpot.bait.Value != null && _baitById.TryGetValue(crabpot.bait.Value.ParentSheetIndex, out string baitName) && baitName.Equals("Magnet");
		}

		/// <summary>Whether the crab pot instance is using wild bait.</summary>
		/// <param name="crabpot">The crab pot instance.</param>
		public static bool IsUsingWildBait(CrabPot crabpot)
		{
			return crabpot.bait.Value != null && _baitById.TryGetValue(crabpot.bait.Value.ParentSheetIndex, out string baitName) && baitName.Equals("Wild Bait");
		}

		/// <summary>Whether the crab pot instance is using magic bait.</summary>
		/// <param name="crabpot">The crab pot instance.</param>
		public static bool IsUsingMagicBait(CrabPot crabpot)
		{
			return crabpot.bait.Value != null && _baitById.TryGetValue(crabpot.bait.Value.ParentSheetIndex, out string baitName) && baitName.Equals("Magic Bait");
		}

		/// <summary>Get the raw fish data for the current game season.</summary>
		/// <param name="location">The location of the crab pot.</param>
		/// <param name="locationData">Raw location data from the game files.</param>
		public static string[] GetRawFishDataForThisSeason(GameLocation location, Dictionary<string, string> locationData)
		{
			return locationData[location.NameOrUniqueName].Split('/')[4 + SUtility.getSeasonNumber(Game1.currentSeason)].Split(' ');
		}

		/// <summary>Get the raw fish data for the all seasons.</summary>
		/// <param name="location">The location of the crab pot.</param>
		/// <param name="locationData">Raw location data from the game files.</param>
		public static string[] GetRawFishDataForAllSeasons(GameLocation location, Dictionary<string, string> locationData)
		{
			List<string> allSeasonFish = new();
			for (int i = 0; i < 4; ++i)
			{
				var seasonalFishData = locationData[location.NameOrUniqueName].Split('/')[4 + i].Split(' ');
				if (seasonalFishData.Length > 1) allSeasonFish.AddRange(seasonalFishData);
			}

			return allSeasonFish.ToArray();
		}

		/// <summary>Convert raw fish data into a look-up dictionary for fishing locations from fish indices.</summary>
		/// <param name="rawFishData">String array of catchable fish indices and fishing locations.</param>
		public static Dictionary<string, string> GetRawFishDataWithLocation(string[] rawFishData)
		{
			Dictionary<string, string> rawFishDataWithLocation = new();
			if (rawFishData.Length > 1)
			{
				for (int i = 0; i < rawFishData.Length; i += 2) rawFishDataWithLocation[rawFishData[i]] = rawFishData[i + 1];
			}

			return rawFishDataWithLocation;
		}

		/// <summary>Choose amongst a pre-select list of fish.</summary>
		/// <param name="crabpot">The crab pot instance.</param>
		/// <param name="fishData">Raw fish data from the game files.</param>
		/// <param name="rawFishDataWithLocation">Dictionary of pre-select fish and their fishing locations.</param>
		/// <param name="location">The location of the crab pot.</param>
		/// <param name="r">Random number generator.</param>
		public static int ChooseFish(CrabPot crabpot, Dictionary<int, string> fishData, Dictionary<string, string> rawFishDataWithLocation, GameLocation location, Random r)
		{
			string[] keys = rawFishDataWithLocation.Keys.ToArray();
			SUtility.Shuffle(r, keys);
			for (int i = 0; i < keys.Length; ++i)
			{
				string[] specificFishData = fishData[Convert.ToInt32(keys[i])].Split('/');
				if (IsLegendaryFish(specificFishData)) continue;

				if (!IsUsingMagicBait(crabpot) && !IsLowLevelFish(specificFishData)) continue;

				int specificFishLocation = Convert.ToInt32(rawFishDataWithLocation[keys[i]]);
				if (!IsUsingMagicBait(crabpot) && (!IsCorrectLocationAndTimeForThisFish(specificFishData, specificFishLocation, crabpot, location) || !IsCorrectWeatherForThisFish(specificFishData, location)))
					continue;

				if (r.NextDouble() < GetChanceForThisFish(specificFishData)) return Convert.ToInt32(keys[i]);
			}

			return -1;
		}

		/// <summary>Whether the specific fish data corresponds to a legendary fish.</summary>
		/// <param name="specificFishData">Raw game file data for this fish.</param>
		public static bool IsLegendaryFish(string[] specificFishData)
		{
			if (specificFishData[0].AnyOf("Crimsonfish", "Angler", "Legend", "Glacierfish", "Mutant Carp", "Son of Crimsonfish", "Ms. Angler", "Legend II", "Glacierfish Jr.", "Radioactive Carp"))
				return true;

			return false;
		}

		/// <summary>Whether the specific fish data corresponds to a sufficiently low level fish.</summary>
		/// <param name="specificFishData">Raw game file data for this fish.</param>
		public static bool IsLowLevelFish(string[] specificFishData)
		{
			if (Convert.ToInt32(specificFishData[1]) < 50) return true;

			return false;
		}

		/// <summary>Whether the current fishing location and game time match the specific fish data.</summary>
		/// <param name="specificFishData">Raw game file data for this fish.</param>
		/// <param name="specificFishLocation">The fishing location index for this fish.</param>
		/// <param name="crabpot">The crab pot instance.</param>
		/// <param name="location">The location of the crab pot.</param>
		public static bool IsCorrectLocationAndTimeForThisFish(string[] specificFishData, int specificFishLocation, CrabPot crabpot, GameLocation location)
		{
			string[] specificFishSpawnTimes = specificFishData[5].Split(' ');
			if (specificFishLocation == -1 || location.getFishingLocation(crabpot.TileLocation) == specificFishLocation)
			{
				for (int t = 0; t < specificFishSpawnTimes.Length; t += 2)
				{
					if (Game1.timeOfDay >= Convert.ToInt32(specificFishSpawnTimes[t]) && Game1.timeOfDay < Convert.ToInt32(specificFishSpawnTimes[t + 1]))
						return true;
				}
			}

			return false;
		}

		/// <summary>Whether the current weather matches the specific fish data.</summary>
		/// <param name="specificFishData">Raw game file data for this fish.</param>
		/// <param name="location">The location of the crab pot.</param>
		public static bool IsCorrectWeatherForThisFish(string[] specificFishData, GameLocation location)
		{
			if (specificFishData[7].Equals("both")) return true;

			if (specificFishData[7].Equals("rainy") && !Game1.IsRainingHere(location)) return false;
			else if (specificFishData[7].Equals("sunny") && Game1.IsRainingHere(location)) return false;

			return true;
		}

		/// <summary>Get the chance of selecting a specific fish from the fish pool.</summary>
		/// <param name="specificFishData">Raw game file data for this fish.</param>
		public static double GetChanceForThisFish(string[] specificFishData)
		{
			return Convert.ToDouble(specificFishData[10]);
		}

		/// <summary>Choose a treasure from the pirate treasure loot table.</summary>
		/// <param name="r">Random number generator.</param>
		public static int ChoosePirateTreasure(Random r, Farmer who)
		{
			int[] keys = _pirateTreasureTable.Keys.ToArray();
			SUtility.Shuffle(r, keys);
			for (int i = 0; i < keys.Length; ++i)
			{
				if (keys[i] == 890 && !who.team.SpecialOrderRuleActive("DROP_QI_BEANS")) continue;

				if (r.NextDouble() < GetChanceForThisTreasure(keys[i])) return keys[i];
			}

			return -1;
		}

		/// <summary>Get the chance of selecting a specific pirate treasure from the pirate treasure table.</summary>
		/// <param name="index">The treasure item index.</param>
		public static double GetChanceForThisTreasure(int index)
		{
			return Convert.ToDouble(_pirateTreasureTable[index][0]);
		}

		/// <summary>Choose amongst a pre-select list of shellfish.</summary>
		/// <param name="crabpot">The crab pot instance.</param>
		/// <param name="fishData">Raw fish data from the game files.</param>
		/// <param name="location">The location of the crab pot.</param>
		/// <param name="r">Random number generator.</param>
		/// <param name="isLuremaster">Whether the owner of the crab pot is luremaster.</param>
		public static int ChooseTrapFish(CrabPot crabpot, Dictionary<int, string> fishData, GameLocation location, Random r, bool isLuremaster)
		{
			List<int> keys = new();
			foreach (KeyValuePair<int, string> kvp in fishData)
			{
				if (!kvp.Value.Contains("trap")) continue;

				bool shouldCatchOceanFish = ShouldCatchOceanFish(crabpot, location);
				string[] rawSplit = kvp.Value.Split('/');
				if ((rawSplit[4].Equals("ocean") && !shouldCatchOceanFish) || (rawSplit[4].Equals("freshwater") && shouldCatchOceanFish))
					continue;

				if (isLuremaster)
				{
					keys.Add(kvp.Key);
					continue;
				}

				if (r.NextDouble() < GetChanceForThisTrapFish(rawSplit)) return kvp.Key;
			}

			if (isLuremaster && keys.Count > 0) return keys[r.Next(keys.Count())];

			return -1;
		}

		/// <summary>Whether a crab pot should catch ocean-specific shellfish.</summary>
		/// <param name="crabpot">The crab pot instance.</param>
		/// <param name="location">The location of the crab pot.</param>
		public static bool ShouldCatchOceanFish(CrabPot crabpot, GameLocation location)
		{
			return location is Beach || location.catchOceanCrabPotFishFromThisSpot((int)crabpot.TileLocation.X, (int)crabpot.TileLocation.Y);
		}

		/// <summary>Get the chance of selecting a specific shellfish from the shellfish pool.</summary>
		/// <param name="rawSplit">Raw game file data for this shellfish.</param>
		public static double GetChanceForThisTrapFish(string[] rawSplit)
		{
			return Convert.ToDouble(rawSplit[2]);
		}

		/// <summary>Get the quality for the chosen catch.</summary>
		/// <param name="who">The owner of the crab pot.</param>
		/// <param name="r">Random number generator.</param>
		public static int GetTrapFishQuality(int whichFish, Farmer who, Random r)
		{
			if (!SpecificPlayerHasProfession("trapper", who) || _pirateTreasureTable.ContainsKey(whichFish)) return 0;

			if (r.NextDouble() < who.FishingLevel / 30.0) return 2;

			if (r.NextDouble() < who.FishingLevel / 15.0) return 1;

			return 0;
		}

		/// <summary>Get initial stack for the chosen stack.</summary>
		/// <param name="crabpot">The crab pot instance.</param>
		/// <param name="whichFish">The chosen fish</param>
		/// <param name="who">The owner of the crab pot.</param>
		/// <param name="r">Random number generator.</param>
		public static int GetTrapFishQuantity(CrabPot crabpot, int whichFish, Farmer who, Random r)
		{
			if (IsUsingWildBait(crabpot) && r.NextDouble() < 0.25 + who.DailyLuck / 2.0)
				return 2;

			if (_pirateTreasureTable.TryGetValue(whichFish, out string[] treasureData))
				return r.Next(Convert.ToInt32(treasureData[1]), Convert.ToInt32(treasureData[2]) + 1);

			return 1;
		}

		/// <summary>Get random trash.</summary>
		/// <param name="r">Random number generator.</param>
		public static int GetTrash(Random r)
		{
			return r.Next(168, 173);
		}

		/// <summary>Whether the given crab pot instance is holding an object that can only be caught by a Luremaster.</summary>
		/// <param name="crabpot">The crab pot instance.</param>
		public static bool IsHoldingSpecialLuremasterCatch(CrabPot crabpot)
		{
			var obj = crabpot.heldObject.Value;
			return (obj.Type == "Fish" && !(IsTrapFish(obj) || IsTrash(obj))) || _pirateTreasureTable.ContainsKey(obj.ParentSheetIndex);
		}
	}
}
