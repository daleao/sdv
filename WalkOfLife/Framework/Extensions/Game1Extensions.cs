using StardewModdingAPI;
using StardewValley;
using System.Linq;

namespace TheLion.Stardew.Professions.Framework.Extensions
{
	public static class Game1Extensions
	{
		/// <summary>Whether any farmer in the current game session has a specific profession.</summary>
		/// <param name="professionName">The name of the profession.</param>
		/// <param name="numberOfPlayersWithThisProfession">How many players have this profession.</param>
		public static bool DoesAnyPlayerHaveProfession(this Game1 game1, string professionName,
			out int numberOfPlayersWithThisProfession)
		{
			if (!Context.IsMultiplayer)
				if (Game1.player.HasProfession(professionName))
				{
					numberOfPlayersWithThisProfession = 1;
					return true;
				}

			numberOfPlayersWithThisProfession = Game1.getAllFarmers()
				.Count(player => player.isActive() && player.HasProfession(professionName));
			return numberOfPlayersWithThisProfession > 0;
		}
	}
}