using StardewValley;
using System.Collections.Generic;
using System.Linq;

namespace TheLion.Stardew.Professions.Framework.Extensions
{
	public static class GameLocationExtensions
	{
		/// <summary>Whether any farmer in the game location has a specific profession.</summary>
		/// <param name="professionName">The name of the profession.</param>
		public static bool DoesAnyPlayerHereHaveProfession(this GameLocation location, string professionName)
		{
			if (!Game1.IsMultiplayer && location.Equals(Game1.currentLocation)) return Game1.player.HasProfession(professionName);
			return location.farmers.Any(farmer => farmer.HasProfession(professionName));
		}

		/// <summary>Whether any farmer in the game location has a specific profession.</summary>
		/// <param name="professionName">The name of the profession.</param>
		/// <param name="farmers">All the farmer instances in the location with the given profession.</param>
		public static bool DoesAnyPlayerHereHaveProfession(this GameLocation location, string professionName, out IList<Farmer> farmers)
		{
			farmers = new List<Farmer>();
			if (!Game1.IsMultiplayer && location.Equals(Game1.player.currentLocation) && Game1.player.HasProfession(professionName))
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
	}
}