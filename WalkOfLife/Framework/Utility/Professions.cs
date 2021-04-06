using StardewValley;
using StardewValley.Buildings;
using System;
using System.Linq;
using TheLion.Common;
using SObject = StardewValley.Object;

namespace TheLion.AwesomeProfessions
{
	/// <summary>Holds common methods and properties related to specific professions.</summary>
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
			{ "Rancher", Farmer.rancher },				// 0
			{ "Breeder", Farmer.butcher },				// 2 (coopmaster)
			{ "Producer", Farmer.shepherd },			// 3

			{ "Harvester", Farmer.tiller },				// 1
			{ "Brewer", Farmer.artisan },				// 4
			{ "Agriculturist", Farmer.agriculturist },	// 5

			// fishing
			{ "Fisher", Farmer.fisher },				// 6
			{ "Angler", Farmer.angler },				// 8
			{ "Aquarist", Farmer.pirate },				// 9

			{ "Trapper", Farmer.trapper },				// 7
			{ "Luremaster", Farmer.baitmaster },		// 10
			{ "Conservationist", Farmer.mariner },		// 11
			// Note: the game code has mariner and baitmaster ids mixed up

			// foraging
			{ "Lumberjack", Farmer.forester },			// 12
			{ "Arborist", Farmer.lumberjack },			// 14
			{ "Tapper", Farmer.tapper },				// 15

			{ "Forager", Farmer.gatherer },				// 13
			{ "Ecologist", Farmer.botanist },			// 16
			{ "Scavenger", Farmer.tracker },			// 17

			// mining
			{ "Miner", Farmer.miner },					// 18
			{ "Spelunker", Farmer.blacksmith },			// 20
			{ "Prospector", Farmer.burrower },			// 21 (prospector)

			{ "Blaster", Farmer.geologist },			// 19
			{ "Demolitionist", Farmer.excavator },		// 22
			{ "Gemologist", Farmer.gemologist },		// 23

			// combat
			{ "Fighter", Farmer.fighter },				// 24
			{ "Brute", Farmer.brute },					// 26
			{ "Gambit", Farmer.defender },				// 27

			{ "Rascal", Farmer.scout },					// 25
			{ "Slimemaster", Farmer.acrobat },			// 28
			{ "Desperado", Farmer.desperado }			// 29
		};

		/// <summary>Generate unique buff ids from a hash seed.</summary>
		/// <param name="hash">Unique instance hash.</param>
		public static void SetProfessionBuffIDs(int hash)
		{
			SpelunkerBuffID = hash + ProfessionMap.Forward["Spelunker"];
			DemolitionistBuffID = hash + ProfessionMap.Forward["Demolitionist"];
			BruteBuffID = hash - ProfessionMap.Forward["Brute"];
			GambitBuffID = hash - ProfessionMap.Forward["Gambit"];
		}

		/// <summary>Whether the local farmer has a specific profession.</summary>
		/// <param name="professionName">The name of the profession.</param>
		public static bool LocalPlayerHasProfession(string professionName)
		{
			if (!ProfessionMap.Contains(professionName)) return false;
			return Game1.player.professions.Contains(ProfessionMap.Forward[professionName]);
		}

		/// <summary>Whether a farmer has a specific profession.</summary>
		/// <param name="professionName">The name of the profession.</param>
		/// <param name="who">The player.</param>
		public static bool SpecificPlayerHasProfession(string professionName, Farmer who)
		{
			return ProfessionMap.Contains(professionName) && who.professions.Contains(ProfessionMap.Forward[professionName]);
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

			numberOfPlayersWithThisProfession = Game1.getAllFarmers().Count(player => player.isActive() && SpecificPlayerHasProfession(professionName, player));

			return numberOfPlayersWithThisProfession > 0;
		}

		/// <summary>Whether any farmer in a specific game location has a specific profession.</summary>
		/// <param name="professionName">The name of the profession.</param>
		/// <param name="location">The game location to check.</param>
		public static bool AnyPlayerInLocationHasProfession(string professionName, GameLocation location)
		{
			if (!Game1.IsMultiplayer && location.Equals(Game1.currentLocation)) return LocalPlayerHasProfession(professionName);
			return location.farmers.Any(farmer => SpecificPlayerHasProfession(professionName, farmer));
		}

		/// <summary>Initialize mod data and instantiate asset editors and helpers for a profession.</summary>
		/// <param name="whichProfession">The profession index.</param>
		public static void InitializeModData(int whichProfession)
		{
			if (!ProfessionMap.Reverse.TryGetValue(whichProfession, out string professionName)) return;

			switch (professionName)
			{
				case "Conservationist":
					AwesomeProfessions.Data
						.WriteFieldIfNotExists($"{AwesomeProfessions.UniqueID}/WaterTrashCollectedThisSeason", "0")
						.WriteFieldIfNotExists($"{AwesomeProfessions.UniqueID}/ActiveTaxBonusPercent", "0");
					break;

				case "Ecologist":
					AwesomeProfessions.Data
						.WriteFieldIfNotExists($"{AwesomeProfessions.UniqueID}/ItemsForaged", "0");
					break;

				case "Gemologist":
					AwesomeProfessions.Data
						.WriteFieldIfNotExists($"{AwesomeProfessions.UniqueID}/MineralsCollected", "0");
					break;

				case "Brewer":
					AwesomeProfessions.Data
						.WriteFieldIfNotExists($"{AwesomeProfessions.UniqueID}/BrewerFameAccrued", "0")
						.WriteFieldIfNotExists($"{AwesomeProfessions.UniqueID}/BrewerAwardLevel", "0")
						.WriteFieldIfNotExists($"{AwesomeProfessions.UniqueID}/FameNeededForNextAwardLevel", "");
					break;

				case "Prospector":
					AwesomeProfessions.Data
						.WriteFieldIfNotExists($"{AwesomeProfessions.UniqueID}/ProspectorHuntStreak", "0");
					break;

				case "Scavenger":
					AwesomeProfessions.Data
						.WriteFieldIfNotExists($"{AwesomeProfessions.UniqueID}/ScavengerHuntStreak", "0");
					break;

				case "Spelunker":
					AwesomeProfessions.Data
						.WriteFieldIfNotExists($"{AwesomeProfessions.UniqueID}/LowestMineLevelReached", "0");
					break;
			}
		}

		/// <summary>Clear unecessary mod data entries for removed profession.</summary>
		/// <param name="whichProfession">The profession index.</param>
		public static void CleanModData(int whichProfession)
		{
			if (!ProfessionMap.Reverse.TryGetValue(whichProfession, out string professionName)) return;

			switch (professionName)
			{
				case "Brewer":
					AwesomeProfessions.Data
						.WriteField($"{AwesomeProfessions.UniqueID}/BrewerFameAccrued", null)
						.WriteField($"{AwesomeProfessions.UniqueID}/FameNeededForNextAwardLevel", null);
					break;

				case "Scavenger":
					AwesomeProfessions.Data
						.WriteField($"{AwesomeProfessions.UniqueID}/ScavengerHuntStreak", null);
					break;

				case "Prospector":
					AwesomeProfessions.Data
						.WriteField($"{AwesomeProfessions.UniqueID}/ProspectorHuntStreak", null);
					break;

				case "Conservationist":
					AwesomeProfessions.Data
						.WriteField($"{AwesomeProfessions.UniqueID}/WaterTrashCollectedThisSeason", null)
						.WriteField($"{AwesomeProfessions.UniqueID}/ActiveTaxBonusPercent", null);
					break;
			}
		}

		/// <summary>Get the price multiplier for beverages sold by Brewer.</summary>
		public static float GetBrewerPriceBonus()
		{
			uint currentLevel = AwesomeProfessions.Data.ReadField($"{AwesomeProfessions.UniqueID}/BrewerAwardLevel", uint.Parse);
			if (currentLevel > 0) return (float)(1f / Math.Pow(2, 5 - currentLevel));
			return 1f;
		}

		/// <summary>Get the price multiplier for produce sold by Producer.</summary>
		/// <param name="who">The player.</param>
		public static float GetProducerPriceMultiplier(Farmer who)
		{
			return 1f + Game1.getFarm().buildings.Where(b => (b.owner.Value.Equals(who.UniqueMultiplayerID) || !Game1.IsMultiplayer) && b.buildingType.Contains("Deluxe") && (b.indoors.Value as AnimalHouse).isFull()).Sum(b => 0.05f);
		}

		/// <summary>Get the price multiplier for fish sold by Angler.</summary>
		/// <param name="who">The player.</param>
		public static float GetAnglerPriceMultiplier(Farmer who)
		{
			return 1f + _LegendaryFishIds.Where(id => who.fishCaught.ContainsKey(id)).Sum(id => 0.05f);
		}

		/// <summary>Get the price multiplier for items sold by Conservationist.</summary>
		/// <param name="who">The player.</param>
		public static float GetConservationistPriceMultiplier(Farmer who)
		{
			if (!who.IsLocalPlayer) return 1f;

			return 1f + AwesomeProfessions.Data.ReadField($"{AwesomeProfessions.UniqueID}/ActiveTaxBonusPercent", float.Parse);
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
			uint itemsForaged = AwesomeProfessions.Data.ReadField($"{AwesomeProfessions.UniqueID}/ItemsForaged", uint.Parse);
			return itemsForaged < AwesomeProfessions.Config.ForagesNeededForBestQuality ? itemsForaged < AwesomeProfessions.Config.ForagesNeededForBestQuality / 2 ? SObject.medQuality : SObject.highQuality : SObject.bestQuality;
		}

		/// <summary>Facilitates incrementing mod data field in transpilers.</summary>
		public static void IncrementItemsForagedForTranspiler()
		{
			AwesomeProfessions.Data.IncrementField($"{AwesomeProfessions.UniqueID}/ItemsForaged", amount: 1);
		}

		/// <summary>Get the quality of mineral for Gemologist.</summary>
		public static int GetGemologistMineralQuality()
		{
			uint mineralsCollected = AwesomeProfessions.Data.ReadField($"{AwesomeProfessions.UniqueID}/ItemsForaged", uint.Parse);
			return mineralsCollected < AwesomeProfessions.Config.MineralsNeededForBestQuality ? mineralsCollected < AwesomeProfessions.Config.MineralsNeededForBestQuality / 2 ? SObject.medQuality : SObject.highQuality : SObject.bestQuality;
		}

		/// <summary>Get the bonus ladder spawn chance for Spelunker.</summary>
		public static double GetSpelunkerBonusLadderDownChance()
		{
			return 1.0 / (1.0 + Math.Exp(Math.Log(2.0 / 3.0) / 120.0 * AwesomeProfessions.Data.ReadField($"{AwesomeProfessions.UniqueID}/LowestMineLevelReached", uint.Parse))) - 0.5;
		}

		/// <summary>Get the bonus bobber bar height for Aquarist.</summary>
		public static int GetAquaristBonusBobberBarHeight()
		{
			if (!LocalPlayerHasProfession("Aquarist")) return 0;

			return Game1.getFarm().buildings.Where(b => (b.owner.Value.Equals(Game1.player.UniqueMultiplayerID) || !Game1.IsMultiplayer) && b is FishPond
			{
				FishCount: >= 10
			}).Sum(_ => 7);
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
			return (LocalPlayerHasProfession("Scavenger") && ((obj.IsSpawnedObject && !IsForagedMineral(obj)) || obj.ParentSheetIndex == 590))
				|| (LocalPlayerHasProfession("Prospector") && (IsResourceNode(obj) || IsForagedMineral(obj)));
		}
	}
}