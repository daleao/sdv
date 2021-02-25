using StardewValley;
using System.Collections.Generic;
using System.Linq;
using TheLion.Common.Classes.BidirectionalMap;
using TheLion.Common.Extensions;
using SObject = StardewValley.Object;

namespace TheLion.AwesomeProfessions.Framework
{
	/// <summary>Some generally useful methods.</summary>
	public static class Utils
	{
		/// <summary>Whether the player has a specific profession.</summary>
		/// <param name="professionName">The name of the profession.</param>
		public static bool PlayerHasProfession(string professionName)
		{
			return Game1.player.professions.Contains(ProfessionsMap.Forward[professionName]);
		}

		/// <summary>Whether the player has a specific profession.</summary>
		/// <param name="professionName">The name of the profession.</param>
		/// <param name="who">The player.</param>
		public static bool PlayerHasProfession(string professionName, Farmer who)
		{
			return who.professions.Contains(ProfessionsMap.Forward[professionName]);
		}

		/// <summary>Bi-directional dictionary for looking-up profession id's by name or name's by id.</summary>
		public static BiMap<string, int> ProfessionsMap { get; set; } = new BiMap<string, int>
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
			{ "conservationist", Farmer.baitmaster },	// 10
			{ "mariner", Farmer.mariner },				// 11

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

		/// <summary>Set of item id's corresponding to animal produce or derived artisan goods.</summary>
		private static readonly IEnumerable<int> _animalProductIds = new HashSet<int>
		{
			107,	// dinosaur egg
			174,	// large egg
			176,	// egg
			180,	// brown egg
			182,	// large brown egg
			184,	// milk
			186,	// large milk
			289,	// ostrich egg
			305,	// void egg
			306,	// mayonnaise
			307,	// duck mayonnaise
			308,	// void mayonnaise
			424,	// cheese
			426,	// goat cheese
			428,	// cloth
			436,	// goat milk
			438,	// large goat milk
			440,	// wool
			442,	// duck egg
			444,	// duck feather
			446,	// rabbit's foot
			807,	// dinosaur mayonnaise
			928		// golden egg
		};

		/// <summary>Whether a given object is an animal produce or derived artisan good.</summary>
		/// <param name="obj">The given object.</param>
		public static bool IsAnimalProduct(SObject obj)
		{
			return _animalProductIds.Contains(obj.ParentSheetIndex);
		}

		/// <summary>Whether a given object is wine.</summary>
		/// <param name="obj">The given object.</param>
		public static bool IsWine(SObject obj)
		{
			return obj.ParentSheetIndex == 348;
		}

		/// <summary>Whether a given object is one of wine, juice, beer, mead or pale ale.</summary>
		/// <param name="obj">The given object.</param>
		public static bool IsWineOrBeverage(SObject obj)
		{
			int pale_ale = 303, beer = 346, juice = 350, mead = 459;
			return IsWine(obj) || obj.ParentSheetIndex.IsIn(pale_ale, beer, juice, mead);
		}

		/// <summary>Whether a given object is a gem or mineral.</summary>
		/// <param name="obj">The given object.</param>
		private static bool IsGemOrMineral(SObject obj)
		{
			return obj.Category == SObject.GemCategory || obj.Category == SObject.mineralsCategory;
		}

		/// <summary>Whether a given object is a fish trapped by a crab pot.</summary>
		/// <param name="obj">The given object.</param>
		public static bool IsShellfish(SObject obj)
		{
			return obj.ParentSheetIndex > 714 && obj.ParentSheetIndex < 724;
		}

		/// <summary>Whether a given object is a fish caught with a fishing rod.</summary>
		/// <param name="obj">The given object.</param>
		public static bool IsReeledFish(SObject obj)
		{
			return obj.Category == SObject.FishCategory && !IsShellfish(obj);
		}
	}
}
