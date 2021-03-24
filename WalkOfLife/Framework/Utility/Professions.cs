using StardewValley;
using StardewValley.Buildings;
using System;
using TheLion.Common.Classes;
using SObject = StardewValley.Object;

namespace TheLion.AwesomeProfessions
{
	public static partial class Utility
	{
		public static int SpelunkerBuffID { get; private set; }
		public static int DemolitionistBuffID { get; private set; }
		public static int BruteBuffID { get; private set; }
		public static int GambitBuffID { get; private set; }

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

		private enum OenologyAwardLevel
		{
			NULL,
			Copper,
			Iron,
			Gold,
			Iridium,
			Stardrop
		}

		/// <summary>Generate unique buff ids from a hash seed.</summary>
		/// <param name="hash">Unique instance hash.</param>
		public static void SetProfessionBuffIDs(int hash)
		{
			SpelunkerBuffID = hash + ProfessionMap.Forward["spelunker"];
			DemolitionistBuffID = hash + ProfessionMap.Forward["demolitionist"];
			BruteBuffID = hash - ProfessionMap.Forward["brute"];
			GambitBuffID = hash - ProfessionMap.Forward["gambit"];
		}

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

		/// <summary>Get the oenology award level corresponding to the local player's oenology fame accrued.</summary>
		public static uint GetLocalPlayerOenologyAwardLevel()
		{
			OenologyAwardLevel awardLevel;
			if (Data.OenologyFameAccrued >= Config.OenologyFameNeededForMaxValue) awardLevel = OenologyAwardLevel.Stardrop;
			else if (Data.OenologyFameAccrued >= (uint)(0.625 * Config.OenologyFameNeededForMaxValue)) awardLevel = OenologyAwardLevel.Iridium;
			else if (Data.OenologyFameAccrued >= (uint)(0.25 * Config.OenologyFameNeededForMaxValue)) awardLevel = OenologyAwardLevel.Gold;
			else if (Data.OenologyFameAccrued >= (uint)(0.1 * Config.OenologyFameNeededForMaxValue)) awardLevel = OenologyAwardLevel.Iron;
			else if (Data.OenologyFameAccrued >= (uint)(0.04 * Config.OenologyFameNeededForMaxValue)) awardLevel = OenologyAwardLevel.Copper;
			else awardLevel = OenologyAwardLevel.NULL;

			return (uint)awardLevel;
		}

		/// <summary>Get the price multiplier for wine and beverages sold by Oenologist.</summary>
		public static float GetOenologistPriceBonus()
		{
			return (OenologyAwardLevel)Data.HighestOenologyAwardEarned switch
			{
				OenologyAwardLevel.Stardrop => 1f,
				OenologyAwardLevel.Iridium => 0.5f,
				OenologyAwardLevel.Gold => 0.2f,
				OenologyAwardLevel.Iron => 0.1f,
				OenologyAwardLevel.Copper => 0.05f,
				_ => 0f
			};
		}

		/// <summary>Get the award name for Oenologist's current award level.</summary>
		public static string GetOenologyAwardName()
		{
			return (OenologyAwardLevel)Data.HighestOenologyAwardEarned switch
			{
				OenologyAwardLevel.Stardrop => "Best in Show",
				OenologyAwardLevel.Iridium => "Iridium",
				OenologyAwardLevel.Gold => "Gold",
				OenologyAwardLevel.Iron => "Iron",
				OenologyAwardLevel.Copper => "Copper",
				_ => ""
			};
		}

		/// <summary>Get the price multiplier for produce sold by Producer.</summary>
		/// <param name="who">The player.</param>
		public static float GetProducerPriceMultiplier(Farmer who)
		{
			float multiplier = 1f;
			foreach (Building b in Game1.getFarm().buildings)
			{
				if ((b.owner.Equals(who.UniqueMultiplayerID) || !Game1.IsMultiplayer) && b.buildingType.Contains("Deluxe") && (b.indoors.Value as AnimalHouse).isFull())
					multiplier += 0.05f;
			}

			return multiplier;
		}

		/// <summary>Get the price multiplier for fish sold by Angler.</summary>
		/// <param name="who">The player.</param>
		public static float GetAnglerPriceMultiplier(Farmer who)
		{
			float multiplier = 1f;
			foreach (int id in _LegendaryFishIds)
			{
				if (who.fishCaught.ContainsKey(id))
					multiplier += 0.05f;
			}

			return multiplier;
		}

		/// <summary>Get the price multiplier for items sold by Conservationist.</summary>
		/// <param name="who">The player.</param>
		public static float GetConservationistPriceMultiplier(Farmer who)
		{
			if (!who.IsLocalPlayer) return 1f;

			return 1f + Data.ConservationistTaxBonusThisSeason / 100f;
		}


		/// <summary>Get adjusted friendship for calculating the value of Breeder-owned farm animal.</summary>
		/// <param name="a">Farm animal instance.</param>
		public static double GetProducerAdjustedFriendship(FarmAnimal a)
		{
			return Math.Pow(Math.Sqrt(2) * a.friendshipTowardFarmer.Value / 1000, 2) + 0.5;
		}

		/// <summary>Get the quality of forage for Ecologist.</summary>
		public static int GetEcologistForageQuality()
		{
			return Data.ItemsForaged < Config.ForagesNeededForBestQuality ? (Data.ItemsForaged < Config.ForagesNeededForBestQuality / 2 ? SObject.medQuality : SObject.highQuality) : SObject.bestQuality;
		}

		/// <summary>Get the quality of mineral for Gemologist.</summary>
		public static int GetGemologistMineralQuality()
		{
			return Data.MineralsCollected < Config.MineralsNeededForBestQuality ? (Data.MineralsCollected < Config.MineralsNeededForBestQuality / 2 ? SObject.medQuality : SObject.highQuality) : SObject.bestQuality;
		}

		/// <summary>Get the bonus ladder spawn chance for Spelunker.</summary>
		public static double GetSpelunkerBonusLadderDownChance()
		{
			return 1.0 / (1.0 + Math.Exp(Math.Log(2.0 / 3.0) / 120.0 * Data.LowestMineLevelReached)) - 0.5;
		}

		/// <summary>Get the bonus bobber bar height for Aquarist.</summary>
		public static int GetAquaristBonusBobberBarHeight()
		{
			if (!LocalPlayerHasProfession("aquarist")) return 0;

			int bonusBobberHeight = 0;
			foreach (Building b in Game1.getFarm().buildings)
			{
				if ((b.owner.Equals(Game1.player.UniqueMultiplayerID) || !Game1.IsMultiplayer) && b is FishPond && (b as FishPond).FishCount >= 10)
					bonusBobberHeight += 7;
			}

			return bonusBobberHeight;
		}

		/// <summary>Get the bonus critical strike chance that should be applied to Gambit.</summary>
		public static float GetBruteBonusDamageMultiplier()
		{
			return (float)(1.0 + AwesomeProfessions.bruteKillStreak * 0.005);
		}

		/// <summary>Get the bonus critical strike chance that should be applied to Gambit.</summary>
		/// <param name="who">The player.</param>
		public static float GetGambitBonusCritChance(Farmer who)
		{
			double healthPercent = (double)who.health / who.maxHealth;
			return (float)(0.2 / (healthPercent + 0.2) - 0.2 / 1.2);
		}

		/// <summary>Get bonus slingshot damage as function of projectile travel distance.</summary>
		/// <param name="travelDistance">Distance travelled by the projectile.</param>
		public static float GetRascalBonusDamageForTravelTime(int travelDistance)
		{
			int maxDistance = 800;
			if (travelDistance > maxDistance) return 1.5f;
			return 0.5f / maxDistance * travelDistance + 1f;
		}

		/// <summary>Whether the player should track a given object.</summary>
		/// <param name="obj">The given object.</param>
		public static bool ShouldPlayerTrackObject(SObject obj)
		{
			return (LocalPlayerHasProfession("scavenger") && ((obj.IsSpawnedObject && !IsForagedMineral(obj)) || obj.ParentSheetIndex == 590))
				|| (LocalPlayerHasProfession("prospector") && (IsResourceNode(obj) || IsForagedMineral(obj)));
		}
	}
}
