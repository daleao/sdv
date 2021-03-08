using Harmony;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Objects;
using StardewValley.Locations;
using System;
using System.Collections.Generic;
using System.Linq;
using TheLion.Common.Extensions;
using SObject = StardewValley.Object;

namespace TheLion.AwesomeProfessions.Framework.Patches
{
	internal class CrabPotDayUpdatePatch : BasePatch
	{
		private static readonly int _treasureId = 166;

		/// <summary>Look-up table for different types of bait by id.</summary>
		private static readonly Dictionary<int, string> _baitIds = new Dictionary<int, string>
		{
			{ 685, "Bait" },
			{ 703, "Magnet" },
			{ 774, "Wild Bait" },
			{ 908, "Magic Bait" }
		};

		/// <summary>Construct an instance.</summary>
		/// <param name="config">The mod settings.</param>
		/// <param name="monitor">Interface for writing to the SMAPI console.</param>
		internal CrabPotDayUpdatePatch(ModConfig config, IMonitor monitor)
		: base(config, monitor) { }

		/// <summary>Apply internally-defined Harmony patches.</summary>
		/// <param name="harmony">The Harmony instance for this mod.</param>
		protected internal override void Apply(HarmonyInstance harmony)
		{
			harmony.Patch(
				AccessTools.Method(typeof(CrabPot), nameof(CrabPot.DayUpdate)),
				prefix: new HarmonyMethod(GetType(), nameof(CrabPotDayUpdatePrefix))
			);
		}

		/// <summary>Patch for Trapper fish quality + Luremaster bait mechanics + Conservationist trash collection mechanics.</summary>
		protected static bool CrabPotDayUpdatePrefix(ref CrabPot __instance, GameLocation location)
		{
			Farmer who = Game1.getFarmer(__instance.owner.Value);
			if (__instance.bait.Value == null && !Utils.SpecificPlayerHasProfession("conservationist", who) || __instance.heldObject.Value != null)
			{
				return false; // don't run original logic
			}

			__instance.tileIndexToShow = 714;
			__instance.readyForHarvest.Value = true;

			Random r = new Random((int)Game1.stats.DaysPlayed + (int)Game1.uniqueIDForThisGame / 2 + (int)__instance.TileLocation.X * 1000 + (int)__instance.TileLocation.Y);
			Dictionary<string, string> locationData = Game1.content.Load<Dictionary<string, string>>("Data\\Locations");
			Dictionary<int, string> fishData = Game1.content.Load<Dictionary<int, string>>("Data\\Fish");
			int whichFish = -1;
			if (Utils.SpecificPlayerHasProfession("luremaster", who))
			{
				if (!_IsUsingMagnet(__instance))
				{
					var rawFishData = _IsUsingMagicBait(__instance) ? _GetRawFishDataForAllSeasons(location, locationData) : _GetRawFishDataForThisSeason(location, locationData);
					var rawFishDataWithLocation = _GetRawFishDataWithLocation(rawFishData);
					whichFish = _ChooseFish(__instance, fishData, rawFishDataWithLocation, location, r);
					if (whichFish < 0)
					{
						whichFish = _ChooseTrapFish(__instance, fishData, location, r, isLuremaster: true);
					}
				}
				else if(r.NextDouble() < 0.3)
				{
					whichFish = _treasureId;
				}
			}
			else if (__instance.bait.Value != null)
			{
				whichFish = _ChooseTrapFish(__instance, fishData, location, r, isLuremaster: false);
			}

			int fishQuality = 0;
			if (whichFish < 0)
			{
				whichFish = _GetTrash(r);
				if (Utils.SpecificPlayerHasProfession("conservationist", who) && who.IsLocalPlayer)
				{
					if (++ModEntry.Data.TrashCollectedAsConservationist % 10 == 0)
					{
						Utility.improveFriendshipWithEveryoneInRegion(Game1.player, 1, 2);
					}
				}
			}
			else
			{
				fishQuality = _GetFishQuality(who, r);
			}

			bool shouldCatchDouble = _ShouldCatchDoubleFish(__instance, who, r);
			__instance.heldObject.Value = new SObject(whichFish, initialStack: shouldCatchDouble ? 2 : 1, quality: fishQuality);
			return false; // don't run original logic
		}

		/// <summary>Whether the crab pot instance is using regular bait.</summary>
		/// <param name="crabpot">The crab pot instance.</param>
		private static bool _IsUsingRegularBait(CrabPot crabpot)
		{
			return _baitIds.TryGetValue(crabpot.bait.Value.ParentSheetIndex, out string baitName) && baitName.Equals("Bait");
		}

		/// <summary>Whether the crab pot instance is using magnet as bait.</summary>
		/// <param name="crabpot">The crab pot instance.</param>
		private static bool _IsUsingMagnet(CrabPot crabpot)
		{
			return _baitIds.TryGetValue(crabpot.bait.Value.ParentSheetIndex, out string baitName) && baitName.Equals("Magnet");
		}

		/// <summary>Whether the crab pot instance is using wild bait.</summary>
		/// <param name="crabpot">The crab pot instance.</param>
		private static bool _IsUsingWildBait(CrabPot crabpot)
		{
			return _baitIds.TryGetValue(crabpot.bait.Value.ParentSheetIndex, out string baitName) && baitName.Equals("Wild Bait");
		}

		/// <summary>Whether the crab pot instance is using magic bait.</summary>
		/// <param name="crabpot">The crab pot instance.</param>
		private static bool _IsUsingMagicBait(CrabPot crabpot)
		{
			return _baitIds.TryGetValue(crabpot.bait.Value.ParentSheetIndex, out string baitName) && baitName.Equals("Magic Bait");
		}

		/// <summary>Get the raw fish data for the current game season.</summary>
		/// <param name="location">The location of the crab pot.</param>
		/// <param name="locationData">Raw location data from the game files.</param>
		private static string[] _GetRawFishDataForThisSeason(GameLocation location, Dictionary<string, string> locationData)
		{
			return locationData[location.NameOrUniqueName].Split('/')[4 + Utility.getSeasonNumber(Game1.currentSeason)].Split(' ');
		}

		/// <summary>Get the raw fish data for the all seasons.</summary>
		/// <param name="location">The location of the crab pot.</param>
		/// <param name="locationData">Raw location data from the game files.</param>
		private static string[] _GetRawFishDataForAllSeasons(GameLocation location, Dictionary<string, string> locationData)
		{
			List<string> allSeasonFish = new();
			for (int i = 0; i < 4; ++i)
			{
				var seasonalFishData = locationData[location.NameOrUniqueName].Split('/')[4 + i].Split(' ');
				if (seasonalFishData.Length > 1)
				{
					allSeasonFish.AddRange(seasonalFishData);
				}
			}
			
			return allSeasonFish.ToArray();
		}

		/// <summary>Convert raw fish data into a look-up dictionary for fishing locations from fish indices.</summary>
		/// <param name="rawFishData">String array of catchable fish indices and fishing locations.</param>
		private static Dictionary<string, string> _GetRawFishDataWithLocation(string[] rawFishData)
		{
			Dictionary<string, string> rawFishDataWithLocation = new();
			if (rawFishData.Length > 1)
			{
				for (int i = 0; i < rawFishData.Length; i += 2)
				{
					rawFishDataWithLocation[rawFishData[i]] = rawFishData[i + 1];
				}
			}

			return rawFishDataWithLocation;
		}

		/// <summary>Choose amongst a pre-select list of fish.</summary>
		/// <param name="crabpot">The crab pot instance.</param>
		/// <param name="fishData">Raw fish data from the game files.</param>
		/// <param name="rawFishDataWithLocation">Dictionary of pre-select fish and their fishing locations.</param>
		/// <param name="location">The location of the crab pot.</param>
		/// <param name="r">Random number generator.</param>
		private static int _ChooseFish(CrabPot crabpot, Dictionary<int, string> fishData, Dictionary<string, string> rawFishDataWithLocation, GameLocation location, Random r)
		{
			string[] keys = rawFishDataWithLocation.Keys.ToArray();
			Utility.Shuffle(r, keys);
			for (int i = 0; i < keys.Length; ++i)
			{
				string[] specificFishData = fishData[Convert.ToInt32(keys[i])].Split('/');
				if (_IsLegendaryFish(specificFishData))
				{
					continue;
				}

				if (_config.Luremaster.CatchOnlyLowLevelFish && !_IsLowLevelFish(specificFishData))
				{
					continue;
				}

				int specificFishLocation = Convert.ToInt32(rawFishDataWithLocation[keys[i]]);
				if (!_IsUsingMagicBait(crabpot) && (!_IsCorrectLocationAndTimeForThisFish(specificFishData, specificFishLocation, crabpot, location) || !_IsCorrectWeatherForThisFish(specificFishData, location)))
				{
					continue;
				}

				if (r.NextDouble() < _GetChanceForThisFish(specificFishData))
				{
					return Convert.ToInt32(keys[i]);
				}
			}

			return -1;
		}

		/// <summary>Whether the specific fish data corresponds to a legendary fish.</summary>
		/// <param name="specificFishData">Raw game file data for this fish.</param>
		private static bool _IsLegendaryFish(string[] specificFishData)
		{
			if (specificFishData[0].AnyOf("Crimsonfish", "Angler", "Legend", "Glacierfish", "Mutant Carp", "Son of Crimsonfish", "Ms. Angler", "Legend II", "Glacierfish Jr.", "Radioactive Carp"))
			{
				return true;
			}

			return false;
		}

		/// <summary>Whether the specific fish data corresponds to a sufficiently low level fish.</summary>
		/// <param name="specificFishData">Raw game file data for this fish.</param>
		private static bool _IsLowLevelFish(string[] specificFishData)
		{
			if (Convert.ToInt32(specificFishData[1]) < 50)
			{
				return true;
			}

			return false;
		}

		/// <summary>Whether the current fishing location and game time match the specific fish data.</summary>
		/// <param name="specificFishData">Raw game file data for this fish.</param>
		/// <param name="specificFishLocation">The fishing location index for this fish.</param>
		/// <param name="crabpot">The crab pot instance.</param>
		/// <param name="location">The location of the crab pot.</param>
		private static bool _IsCorrectLocationAndTimeForThisFish(string[] specificFishData, int specificFishLocation, CrabPot crabpot, GameLocation location)
		{
			string[] specificFishSpawnTimes = specificFishData[5].Split(' ');
			if (specificFishLocation == -1 || location.getFishingLocation(crabpot.TileLocation) == specificFishLocation)
			{
				for (int t = 0; t < specificFishSpawnTimes.Length; t += 2)
				{
					if (Game1.timeOfDay >= Convert.ToInt32(specificFishSpawnTimes[t]) && Game1.timeOfDay < Convert.ToInt32(specificFishSpawnTimes[t + 1]))
					{
						return true;
					}
				}
			}

			return false;
		}

		/// <summary>Whether the current weather matches the specific fish data.</summary>
		/// <param name="specificFishData">Raw game file data for this fish.</param>
		/// <param name="location">The location of the crab pot.</param>
		private static bool _IsCorrectWeatherForThisFish(string[] specificFishData, GameLocation location)
		{
			if (specificFishData[7].Equals("both"))
			{
				return true;
			}

			if (specificFishData[7].Equals("rainy") && !Game1.IsRainingHere(location))
			{
				return false;
			}
			else if (specificFishData[7].Equals("sunny") && Game1.IsRainingHere(location))
			{
				return false;
			}

			return true;
		}

		/// <summary>Get the chance of selecting a specific fish from the fish pool.</summary>
		/// <param name="specificFishData">Raw game file data for this fish.</param>
		private static double _GetChanceForThisFish(string[] specificFishData)
		{
			return Convert.ToDouble(specificFishData[10]);
		}

		/// <summary>Choose amongst a pre-select list of shellfish.</summary>
		/// <param name="crabpot">The crab pot instance.</param>
		/// <param name="fishData">Raw fish data from the game files.</param>
		/// <param name="location">The location of the crab pot.</param>
		/// <param name="r">Random number generator.</param>
		/// <param name="isLuremaster">Whether the owner of the crab pot is luremaster.</param>
		private static int _ChooseTrapFish(CrabPot crabpot, Dictionary<int, string> fishData, GameLocation location, Random r, bool isLuremaster)
		{
			List<int> keys = new();
			foreach (KeyValuePair<int, string> kvp in fishData)
			{
				if (!kvp.Value.Contains("trap"))
				{
					continue;
				}

				bool shouldCatchOceanFish = _ShouldCatchOceanFish(crabpot, location);
				string[] rawSplit = kvp.Value.Split('/');
				if ((rawSplit[4].Equals("ocean") && !shouldCatchOceanFish) || (rawSplit[4].Equals("freshwater") && shouldCatchOceanFish))
				{
					continue;
				}

				if (isLuremaster)
				{
					keys.Add(kvp.Key);
					continue;
				}

				if (r.NextDouble() < _GetChanceForThisTrapFish(rawSplit))
				{
					return kvp.Key;
				}
			}

			if (isLuremaster && keys.Count > 0)
			{
				return keys[r.Next(keys.Count())];
			}

			return -1;
		}

		/// <summary>Whether a crab pot should catch ocean-specific shellfish.</summary>
		/// <param name="crabpot">The crab pot instance.</param>
		/// <param name="location">The location of the crab pot.</param>
		private static bool _ShouldCatchOceanFish(CrabPot crabpot, GameLocation location)
		{
			return location is Beach || location.catchOceanCrabPotFishFromThisSpot((int)crabpot.TileLocation.X, (int)crabpot.TileLocation.Y);
		}

		/// <summary>Get the chance of selecting a specific shellfish from the shellfish pool.</summary>
		/// <param name="rawSplit">Raw game file data for this shellfish.</param>
		private static double _GetChanceForThisTrapFish(string[] rawSplit)
		{
			return Convert.ToDouble(rawSplit[2]);
		}

		/// <summary>Get random trash.</summary>
		/// <param name="r">Random number generator.</param>
		private static int _GetTrash(Random r)
		{
			return r.Next(168, 173);
		}

		/// <summary>Determine the quality of the catch.</summary>
		/// <param name="who">The owner of the crab pot.</param>
		/// <param name="r">Random number generator.</param>
		private static int _GetFishQuality(Farmer who, Random r)
		{
			if (!Utils.SpecificPlayerHasProfession("trapper", who))
			{
				return 0;
			}

			if (r.NextDouble() < who.FishingLevel / 30.0)
			{
				return 2;
			}
			
			if (r.NextDouble() < who.FishingLevel / 15.0)
			{
				return 1;
			}

			return 0;
		}

		/// <summary>Whether the crab pot catch should be doubled.</summary>
		/// <param name="crabpot">The crab pot instance.</param>
		/// <param name="who">The owner of the crab pot.</param>
		/// <param name="r">Random number generator.</param>
		private static bool _ShouldCatchDoubleFish(CrabPot crabpot, Farmer who, Random r)
		{
			if (_IsUsingWildBait(crabpot) && r.NextDouble() < 0.25 + who.DailyLuck / 2.0)
			{
				return true;
			}

			return false;
		}
	}
}
