using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Locations;
using System;
using System.Collections.Generic;
using TheLion.Common.Classes;
using TheLion.Common.Extensions;
using SObject = StardewValley.Object;

namespace TheLion.AwesomeProfessions.Framework.Patches
{
	internal static class Globals
	{
		#region compile-time constants
		public static List<Vector2> InitialLadderTiles { get; } = new();
		public static int WineFameNeededForMaxValue { get; } = 1000;
		public static int MaxStartingFriendshipForNewbornAnimals { get; } = 200;
		public static int MineralsNeededForBestQuality { get; } = 500;
		public static int ForagesNeededForBestQuality { get; } = 500;
		public static float ChanceToStartTreasureHunt { get; } = 0.2f;
		public static int TreasureHuntDurationMinutes { get; } = 30;
		public static int TrashNeededForNextTaxLevel { get; } = 50;

		/// <summary>Bi-directional dictionary for looking-up profession id's by name or name's by id.</summary>
		public static BiMap<string, int> ProfessionMap { get; } = new BiMap<string, int>
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
			// Note: the game code has mariner and baitmaster ids mixed up

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
			{ "slimetamer", Farmer.acrobat },			// 28
			{ "desperado", Farmer.desperado }			// 29
		};
		#endregion compile-time constants

		#region runtime constants
		public static int UniqueBuffID { get; } = Common.Utils.GetDigitsFromHash("thelion.AwesomeProfessions", 8);
		public static int SpelunkerBuffID { get; } = UniqueBuffID + ProfessionMap.Forward["spelunker"];
		public static int DemolitionistBuffID { get; } = UniqueBuffID + ProfessionMap.Forward["demolitionist"];
		public static int BruteBuffID { get; } = UniqueBuffID - ProfessionMap.Forward["brute"];
		public static int GambitBuffID { get; } = UniqueBuffID - ProfessionMap.Forward["gambit"];

		public static int DemolitionistBuffMagnitude { get; set; } = 0;
		public static uint BruteKillStreak { get; set; } = 0;
		#endregion runtime constants

		#region public methods
		/// <summary>Whether the local farmer has a specific profession.</summary>
		/// <param name="professionName">The name of the profession.</param>
		public static bool LocalPlayerHasProfession(string professionName)
		{
			return Game1.player.professions.Contains(ProfessionMap.Forward[professionName]);
		}

		/// <summary>Whether a farmer has a specific profession.</summary>
		/// <param name="professionName">The name of the profession.</param>
		/// <param name="who">The player.</param>
		public static bool SpecificPlayerHasProfession(string professionName, Farmer who)
		{
			return who.professions.Contains(ProfessionMap.Forward[professionName]);
		}

		/// <summary>Whether any farmer in the current multiplayer session has a specific profession.</summary>
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
					++numberOfPlayersWithThisProfession;
			}

			return numberOfPlayersWithThisProfession > 0;
		}

		/// <summary>Get the bonus ladder spawn chance for Spelunker.</summary>
		public static double GetBonusLadderDownChanceForSpelunker()
		{
			return 1.0 / (1.0 + Math.Exp(Math.Log(2.0 / 3.0) / 120.0 * AwesomeProfessions.Data.LowestMineLevelReached)) - 0.5;
		}

		/// <summary>Get the quality of forage for Ecologist.</summary>
		public static int GetForageQualityForEcologist()
		{
			return AwesomeProfessions.Data.ItemsForaged < ForagesNeededForBestQuality ? (AwesomeProfessions.Data.ItemsForaged < ForagesNeededForBestQuality / 2 ? SObject.medQuality : SObject.highQuality) : SObject.bestQuality;
		}

		/// <summary>Get the quality of mineral for Gemologist.</summary>
		public static int GetMineralQualityForGemologist()
		{
			return AwesomeProfessions.Data.MineralsCollected < MineralsNeededForBestQuality ? (AwesomeProfessions.Data.MineralsCollected < MineralsNeededForBestQuality / 2 ? SObject.medQuality : SObject.highQuality) : SObject.bestQuality;
		}

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
		#endregion public methods
	}
}
