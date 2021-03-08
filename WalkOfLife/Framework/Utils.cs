using StardewValley;
using System;
using TheLion.Common.Classes;

namespace TheLion.AwesomeProfessions.Framework
{
	public static class Utils
	{
		public static int DemolitionistBuffUniqueID { get; } = _IdFromHashCode("demolitionist", 4);
		public static int SpelunkerBuffUniqueID { get; } = _IdFromHashCode("spelunker", 4);
		public static int BruteBuffUniqueID { get; } = _IdFromHashCode("brute", 4);
		public static int GambitBuffUniqueID { get; } = _IdFromHashCode("gambit", 4);

		/// <summary>Whether the local player has a specific profession.</summary>
		/// <param name="professionName">The name of the profession.</param>
		public static bool LocalPlayerHasProfession(string professionName)
		{
			return Game1.player.professions.Contains(ProfessionsMap.Forward[professionName]);
		}

		/// <summary>Whether the player has a specific profession.</summary>
		/// <param name="professionName">The name of the profession.</param>
		/// <param name="who">The player.</param>
		public static bool SpecificPlayerHasProfession(string professionName, Farmer who)
		{
			return who.professions.Contains(ProfessionsMap.Forward[professionName]);
		}

		/// <summary>Whether any player has a specific profession.</summary>
		/// <param name="professionName">The name of the profession.</param>
		/// <param name="numberOfPlayersWithThisProfession">How many players have this profession.</param>
		public static bool AnyPlayerHasProfession(string professionName, out int numberOfPlayersWithThisProfession)
		{
			if (!Game1.IsMultiplayer)
			{
				if (LocalPlayerHasProfession(professionName))
				{
					numberOfPlayersWithThisProfession = 1;
					return true;
				}
			}

			numberOfPlayersWithThisProfession = 0;
			foreach (Farmer player in Game1.getAllFarmers())
			{
				if (player.isActive() && SpecificPlayerHasProfession(professionName, player))
				{
					++numberOfPlayersWithThisProfession;
				}
			}

			return numberOfPlayersWithThisProfession > 0;
		}

		/// <summary>Bi-directional dictionary for looking-up profession id's by name or name's by id.</summary>
		public static BiMap<string, int> ProfessionsMap { get; } = new BiMap<string, int>
		{
			// farming
			{ "rancher", Farmer.rancher },				// 0
			{ "breeder", Farmer.butcher },				// 2 (coopmaster)
			{ "producer", Farmer.shepherd },			// 3

			{ "harvester", Farmer.tiller },				// 1
			{ "oenologist", Farmer.artisan },			// 4
			{ "agriculturist", Farmer.agriculturist },	// 5

			// fishing
			{ "fisher", Farmer.fisher },				// 6
			{ "angler", Farmer.angler },				// 8
			{ "aquarist", Farmer.pirate },				// 9

			{ "trapper", Farmer.trapper },				// 7
			{ "luremaster", Farmer.baitmaster },		// 10
			{ "conservationist", Farmer.mariner },		// 11
			// Note: the game code incorrectly labels mariner and baitmaster ids

			// foraging
			{ "lumberjack", Farmer.forester },			// 12
			{ "arborist", Farmer.lumberjack },			// 14
			{ "tapper", Farmer.tapper },				// 15

			{ "forager", Farmer.gatherer },				// 13
			{ "ecologist", Farmer.botanist },			// 16
			{ "scavenger", Farmer.tracker },			// 17

			// mining
			{ "miner", Farmer.miner },					// 18
			{ "spelunker", Farmer.blacksmith },			// 20
			{ "prospector", Farmer.burrower },			// 21 (prospector)

			{ "blaster", Farmer.geologist },			// 19
			{ "demolitionist", Farmer.excavator },		// 22
			{ "gemologist", Farmer.gemologist },		// 23


			// combat
			{ "fighter", Farmer.fighter },				// 24
			{ "brute", Farmer.brute },					// 26
			{ "gambit", Farmer.defender },				// 27

			{ "rascal", Farmer.scout },					// 25
			{ "marksman", Farmer.acrobat },				// 29
			{ "slimemaster", Farmer.desperado }			// 28
		};

		private static int _IdFromHashCode(string text, int digits)
		{
			return (int)(Math.Abs(text.GetHashCode()) / Math.Pow(10, Math.Floor(Math.Log10(Math.Abs(text.GetHashCode()))) - digits + 1));
		}
	}
}
