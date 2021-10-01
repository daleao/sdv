using StardewModdingAPI;
using StardewValley;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SUtility = StardewValley.Utility;

namespace TheLion.Stardew.Professions.Framework.Extensions
{
	public static class GameLocationExtensions
	{
		/// <summary>Whether any farmer in the game location has a specific profession.</summary>
		/// <param name="professionName">The name of the profession.</param>
		public static bool DoesAnyPlayerHereHaveProfession(this GameLocation location, string professionName)
		{
			if (!Context.IsMultiplayer && location.Equals(Game1.currentLocation)) return Game1.player.HasProfession(professionName);
			return location.farmers.Any(farmer => farmer.HasProfession(professionName));
		}

		/// <summary>Whether any farmer in the game location has a specific profession.</summary>
		/// <param name="professionName">The name of the profession.</param>
		/// <param name="farmers">All the farmer instances in the location with the given profession.</param>
		public static bool DoesAnyPlayerHereHaveProfession(this GameLocation location, string professionName, out IList<Farmer> farmers)
		{
			farmers = new List<Farmer>();
			if (!Context.IsMultiplayer && location.Equals(Game1.player.currentLocation) && Game1.player.HasProfession(professionName))
			{
				farmers.Add(Game1.player);
			}
			else
			{
				foreach (var farmer in location.farmers.Where(farmer => farmer.HasProfession(professionName)))
					farmers.Add(farmer);
			}

			return farmers.Any();
		}

		/// <summary>Get the raw fish data for the game location and current game season.</summary>
		public static string[] GetRawFishDataForCurrentSeason(this GameLocation location)
		{
			var locationData = Game1.content.Load<Dictionary<string, string>>(Path.Combine("Data", "Locations"));
			return locationData[location.NameOrUniqueName].Split('/')[4 + SUtility.getSeasonNumber(Game1.currentSeason)].Split(' ');
		}

		/// <summary>Get the raw fish data for the game location and all seasons.</summary>
		public static string[] GetRawFishDataForAllSeasons(this GameLocation location)
		{
			var locationData = Game1.content.Load<Dictionary<string, string>>(Path.Combine("Data", "Locations"));
			List<string> allSeasonFish = new();
			for (var i = 0; i < 4; ++i)
			{
				var seasonalFishData = locationData[location.NameOrUniqueName].Split('/')[4 + i].Split(' ');
				if (seasonalFishData.Length > 1) allSeasonFish.AddRange(seasonalFishData);
			}
			return allSeasonFish.ToArray();
		}
	}
}