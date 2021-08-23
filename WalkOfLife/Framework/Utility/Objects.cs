using System.Collections.Generic;
using System.Linq;
using TheLion.Stardew.Common.Extensions;
using SObject = StardewValley.Object;

namespace TheLion.Stardew.Professions.Framework.Util
{
	/// <summary>Holds common methods and properties related to objects.</summary>
	public static class Objects
	{
		#region look-up tables

		/// <summary>Look-up table for different types of bait by id.</summary>
		public static Dictionary<int, string> BaitById { get; } = new()
		{
			{ 685, "Bait" },
			{ 703, "Magnet" },
			{ 774, "Wild Bait" },
			{ 908, "Magic Bait" }
		};

		/// <summary>Look-up table for what resource should spawn from a given stone.</summary>
		public static Dictionary<int, int> ResourceFromStoneId { get; } = new()
		{
			// stone
			{ 668, 390 },
			{ 670, 390 },
			{ 845, 390 },
			{ 846, 390 },
			{ 847, 390 },

			// ores
			{ 751, 378 },
			{ 849, 378 },
			{ 290, 380 },
			{ 850, 380 },
			{ 764, 384 },
			{ 765, 386 },
			{ 95, 909 },

			// geodes
			{ 75, 535 },
			{ 76, 536 },
			{ 77, 537 },
			{ 819, 749 },

			// gems
			{ 8, 66 },
			{ 10, 68 },
			{ 12, 60 },
			{ 14, 62 },
			{ 6, 70 },
			{ 4, 64 },
			{ 2, 72 },

			// other
			{ 25, 719 },
			{ 816, 881 },
			{ 817, 881 },
			{ 818, 330 },
			{ 843, 848 },
			{ 844, 848 }
		};

		/// <summary>Look-up table for trappable treasure items using magnet.</summary>
		public static Dictionary<int, string[]> PirateTreasureTable { get; } = new()
		{
			{ 14, new[] { "1.003", "1", "1" } },     // neptune's glaive
			{ 51, new[] { "1.003", "1", "1" } },     // broken trident
			{ 166, new[] { "0.03", "1", "1" } },     // treasure chest
			{ 109, new[] { "0.009", "1", "1" } },    // ancient sword
			{ 110, new[] { "0.009", "1", "1" } },    // rusty spoon
			{ 111, new[] { "0.009", "1", "1" } },    // rusty spur
			{ 112, new[] { "0.009", "1", "1" } },    // rusty cog
			{ 117, new[] { "0.009", "1", "1" } },    // anchor
			{ 378, new[] { "0.39", "1", "24" } },    // copper ore
			{ 380, new[] { "0.24", "1", "24" } },    // iron ore
			{ 384, new[] { "0.12", "1", "24" } },    // gold ore
			{ 386, new[] { "0.065", "1", "2" } },    // iridium ore
			{ 516, new[] { "0.024", "1", "1" } },    // small glow ring
			{ 517, new[] { "1.009", "1", "1" } },    // glow ring
			{ 518, new[] { "0.024", "1", "1" } },    // small magnet ring
			{ 519, new[] { "1.009", "1", "1" } },    // magnet ring
			{ 527, new[] { "0.005", "1", "1" } },    // iridium band
			{ 529, new[] { "0.005", "1", "1" } },    // amethyst ring
			{ 530, new[] { "0.005", "1", "1" } },    // topaz ring
			{ 531, new[] { "0.005", "1", "1" } },    // aquamarine ring
			{ 532, new[] { "0.005", "1", "1" } },    // jade ring
			{ 533, new[] { "0.005", "1", "1" } },    // emerald ring
			{ 534, new[] { "0.005", "1", "1" } },    // ruby ring
			{ 890, new[] { "0.03", "1", "3" } }      // qi bean
		};

		#endregion look-up tables

		#region hash sets

		/// <summary>Hash list of artisan machines.</summary>
		private static readonly IEnumerable<string> ArtisanMachines = new HashSet<string>
		{
			"Alembic",
			"Butter Churn",
			"Cheese Press",
			"Dehydrator",
			"Drying Rack",
			"Espresso Machine",
			"Extruder",
			"Foreign Cask",
			"Glass Jar",
			"Grinder",
			"Ice Cream Machine",
			"Infuser",
			"Juicer",
			"Keg",
			"Loom",
			"Mayonnaise Machine",
			"Oil Maker",
			"Pepper Blender",
			"Preserves Jar",
			"Smoker",
			"Soap Press",
			"Sorbet Machine",
			"Still",
			"Vinegar Cask",
			"Wax Barrel",
			"Yogurt Jar",
		};

		/// <summary>Hash list of ids corresponding to animal produce or derived artisan goods.</summary>
		private static readonly IEnumerable<int> AnimalDerivedProductIDs = new HashSet<int>
		{
			107,	// dinosaur egg
			306,	// mayonnaise
			307,	// duck mayonnaise
			308,	// void mayonnaise
			340,	// honey
			424,	// cheese
			426,	// goat cheese
			428,	// cloth
			807,	// dinosaur mayonnaise
		};

		/// <summary>Hash list of ammunition ids.</summary>
		private static readonly IEnumerable<int> MineralAmmunitionIDs = new HashSet<int>
		{
			SObject.copper + 1,
			SObject.iron + 1,
			SObject.coal + 1,
			SObject.gold + 1,
			SObject.iridium + 1,
			SObject.stone + 1,
		};

		/// <summary>Hash list of stone ids corresponding to resource nodes.</summary>
		private static readonly IEnumerable<int> ResourceNodeIDs = new HashSet<int>
		{
			// ores
			751,	// copper node
			849,	// copper ?
			290,	// silver node
			850,	// silver ?
			764,	// gold node
			765,	// iridium node
			95,		// radioactive node

			// geodes
			75,		// geode node
			76,		// frozen geode node
			77,		// magma geode node
			819,	// omni geode node

			// gems
			8,		// amethyst node
			10,		// topaz node
			12,		// emerald node
			14,		// aquamarine node
			6,		// jade node
			4,		// ruby node
			2,		// diamond node
			44,		// gem node

			// other
			25,		// mussel node
			816,	// bone node
			817,	// bone node
			818,	// clay node
			843,	// cinder shard node
			844,	// cinder shard node
			46		// mystic stone
		};

		#endregion hash sets

		#region public methods

		/// <summary>Whether a given object is an artisan good.</summary>
		/// <param name="obj">The given object.</param>
		public static bool IsArtisanGood(SObject obj)
		{
			return obj?.Category == SObject.artisanGoodsCategory;
		}

		/// <summary>Whether a given object is an artisan good.</summary>
		/// <param name="obj">The given object.</param>
		public static bool IsArtisanMachine(SObject obj)
		{
			return ArtisanMachines.Contains(obj?.Name);
		}

		/// <summary>Whether a given object is an animal produce or derived artisan good.</summary>
		/// <param name="obj">The given object.</param>
		public static bool IsAnimalProduct(SObject obj)
		{
			return obj != null && (obj.Category.AnyOf(SObject.EggCategory, SObject.MilkCategory, SObject.sellAtPierresAndMarnies)
				|| AnimalDerivedProductIDs.Contains(obj.ParentSheetIndex));
		}

		/// <summary>Whether a given object is salmonberry or blackberry.</summary>
		/// <param name="obj">The given object.</param>
		public static bool IsWildBerry(SObject obj)
		{
			return obj?.ParentSheetIndex == 296 || obj?.ParentSheetIndex == 410;
		}

		/// <summary>Whether a given object is a spring onion.</summary>
		/// <param name="obj">The given object.</param>
		public static bool IsSpringOnion(SObject obj)
		{
			return obj?.ParentSheetIndex == 399;
		}

		/// <summary>Whether a given object is a gem or mineral.</summary>
		/// <param name="obj">The given object.</param>
		public static bool IsGemOrMineral(SObject obj)
		{
			return obj?.Category.AnyOf(SObject.GemCategory, SObject.mineralsCategory) == true;
		}

		/// <summary>Whether a given object is a foraged mineral.</summary>
		/// <param name="obj">The given object.</param>
		public static bool IsForagedMineral(SObject obj)
		{
			return obj.Name.AnyOf("Quartz", "Earth Crystal", "Frozen Tear", "Fire Quartz");
		}

		/// <summary>Whether a given object is a resource node or foraged mineral.</summary>
		/// <param name="obj">The given object.</param>
		public static bool IsResourceNode(SObject obj)
		{
			return IsStone(obj) && ResourceNodeIDs.Contains(obj.ParentSheetIndex);
		}

		/// <summary>Whether a given object is a stone.</summary>
		/// <param name="obj">The world object.</param>
		public static bool IsStone(SObject obj)
		{
			return obj?.Name == "Stone";
		}

		/// <summary>Whether a given item index corresponds to mineral ammunition.</summary>
		/// <param name="index">An item index.</param>
		public static bool IsMineralAmmunition(int index)
		{
			return MineralAmmunitionIDs.Contains(index);
		}

		/// <summary>Whether a given item index corresponds to algae or seaweed.</summary>
		/// <param name="index">The given object.</param>
		public static bool IsAlgae(int index)
		{
			return index.AnyOf(152, 152, 157);
		}

		/// <summary>Whether a given object is a fish caught with a fishing rod.</summary>
		/// <param name="obj">The given object.</param>
		public static bool IsFish(SObject obj)
		{
			return obj?.Category == SObject.FishCategory;
		}

		/// <summary>Whether the specific fish data corresponds to a legendary fish.</summary>
		/// <param name="fishName">The name of the fish.</param>
		public static bool IsLegendaryFish(string fishName)
		{
			return fishName.AnyOf("Crimsonfish", "Angler", "Legend", "Glacierfish", "Mutant Carp", "Son of Crimsonfish", "Ms. Angler", "Legend II", "Glacierfish Jr.", "Radioactive Carp");
		}

		/// <summary>Whether a given object is a crab pot fish.</summary>
		/// <param name="obj">The given object.</param>
		public static bool IsTrapFish(SObject obj)
		{
			return IsFish(obj) && obj.ParentSheetIndex > 714 && obj.ParentSheetIndex < 724;
		}

		/// <summary>Whether a given object is a trash.</summary>
		/// <param name="obj">The given object.</param>
		public static bool IsTrash(SObject obj)
		{
			return obj?.Category == SObject.junkCategory;
		}

		/// <summary>Whether a given object is typically found in pirate treasure.</summary>
		/// <param name="obj">The given object.</param>
		public static bool IsPirateTreasure(SObject obj)
		{
			return PirateTreasureTable.ContainsKey(obj.ParentSheetIndex);
		}

		#endregion public methods
	}
}