using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Buildings;
using System;
using System.IO;
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
			{ "slimemaster", Farmer.acrobat },			// 28
			{ "desperado", Farmer.desperado }			// 29
		};

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
		public static bool LocalFarmerHasProfession(string professionName)
		{
			return Game1.player.professions.Contains(ProfessionMap.Forward[professionName]);
		}

		/// <summary>Whether a farmer has a specific profession.</summary>
		/// <param name="professionName">The name of the profession.</param>
		/// <param name="who">The player.</param>
		public static bool SpecificFarmerHasProfession(string professionName, Farmer who)
		{
			return who.professions.Contains(ProfessionMap.Forward[professionName]);
		}

		/// <summary>Whether any farmer in the current multiplayer session has a specific profession.</summary>
		/// <param name="professionName">The name of the profession.</param>
		/// <param name="numberOfPlayersWithThisProfession">How many players have this profession.</param>
		public static bool AnyFarmerHasProfession(string professionName, out int numberOfPlayersWithThisProfession)
		{
			if (!Game1.IsMultiplayer)
			{
				if (LocalFarmerHasProfession(professionName))
				{
					numberOfPlayersWithThisProfession = 1;
					return true;
				}
			}

			numberOfPlayersWithThisProfession = 0;
			foreach (Farmer player in Game1.getAllFarmers())
			{
				if (player.isActive() && SpecificFarmerHasProfession(professionName, player))
					++numberOfPlayersWithThisProfession;
			}

			return numberOfPlayersWithThisProfession > 0;
		}

		/// <summary>Whether any farmer in a specific game location has a specific profession.</summary>
		/// <param name="professionName">The name of the profession.</param>
		/// <param name="where">The game location to check.</param>
		public static bool AnyFarmerInLocationHasProfession(string professionName, GameLocation location)
		{
			if (!Game1.IsMultiplayer && location == Game1.currentLocation) return LocalFarmerHasProfession(professionName);
			return location.farmers.Any(farmer => SpecificFarmerHasProfession(professionName, farmer));
		}

		/// <summary>Initialize mod data and instantiate asset editors and helpers for a profession.</summary>
		/// <param name="whichProfession">The profession index.</param>
		public static void InitializeProfession(int whichProfession)
		{
			if (!ProfessionMap.Reverse.TryGetValue(whichProfession, out string professionName)) return;
			
			if (professionName.Equals("conservationist"))
			{
				AwesomeProfessions.Content.AssetEditors.Add(new FRSMailEditor());
				AwesomeProfessions.Data
					.WriteFieldIfNotExists($"{AwesomeProfessions.UniqueID}/WaterTrashCollectedThisSeason", "0")
					.WriteFieldIfNotExists($"{AwesomeProfessions.UniqueID}/ActiveTaxBonus", "0");
			}
			else if (professionName.Equals("ecologist"))
			{
				AwesomeProfessions.Data
					.WriteFieldIfNotExists($"{AwesomeProfessions.UniqueID}/ItemsForaged", "0");
			}
			else if (professionName.Equals("gemologist"))
			{
				AwesomeProfessions.Data
					.WriteFieldIfNotExists($"{AwesomeProfessions.UniqueID}/MineralsCollected", "0");
			}
			else if (professionName.Equals("oenologist"))
			{
				AwesomeProfessions.Content.AssetEditors.Add(new SWAMailEditor());
				AwesomeProfessions.Data
					.WriteFieldIfNotExists($"{AwesomeProfessions.UniqueID}/OenologyFameAccrued", "0")
					.WriteFieldIfNotExists($"{AwesomeProfessions.UniqueID}/OenologyAwardLevel", "0")
					.WriteFieldIfNotExists($"{AwesomeProfessions.UniqueID}/FameNeededForNextAwardLevel", "");
			}
			else if (professionName.Equals("prospector"))
			{
				if (AwesomeProfessions.ProspectorHunt == null)
					AwesomeProfessions.ProspectorHunt = new ProspectorHunt(AwesomeProfessions.I18n.Get("prospector.huntstarted"), AwesomeProfessions.I18n.Get("prospector.huntfailed"), AwesomeProfessions.Content.Load<Texture2D>(Path.Combine("assets", "prospector.png")));

				if (ArrowPointer == null) ArrowPointer = new ArrowPointer(AwesomeProfessions.Content.Load<Texture2D>(Path.Combine("assets", "cursor.png")));

				AwesomeProfessions.Data
					.WriteFieldIfNotExists($"{AwesomeProfessions.UniqueID}/ProspectorHuntStreak", "0");
			}
			else if (professionName.Equals("scavenger"))
			{
				if (AwesomeProfessions.ScavengerHunt == null)
					AwesomeProfessions.ScavengerHunt = new ScavengerHunt(AwesomeProfessions.I18n.Get("scavenger.huntstarted"), AwesomeProfessions.I18n.Get("scavenger.huntfailed"), AwesomeProfessions.Content.Load<Texture2D>(Path.Combine("assets", "scavenger.png")));

				if (ArrowPointer == null) ArrowPointer = new ArrowPointer(AwesomeProfessions.Content.Load<Texture2D>(Path.Combine("assets", "cursor.png")));

				AwesomeProfessions.Data
					.WriteFieldIfNotExists($"{AwesomeProfessions.UniqueID}/ScavengerHuntStreak", "0");
			}
			else if (professionName.Equals("spelunker"))
			{
				AwesomeProfessions.Data
					.WriteFieldIfNotExists($"{AwesomeProfessions.UniqueID}/LowestMineLevelReached", "0");
			}
		}

		/// <summary>Clear unecessary mod data entries for removed profession.</summary>
		/// <param name="whichProfession">The profession index.</param>
		public static void CleanProfessionModData(int whichProfession)
		{
			if (!ProfessionMap.Reverse.TryGetValue(whichProfession, out string professionName)) return;

			if (professionName.Equals("oenologist"))
			{
				AwesomeProfessions.Data
					.WriteField($"{AwesomeProfessions.UniqueID}/OenologyFameAccrued", null)
					.WriteField($"{AwesomeProfessions.UniqueID}/FameNeededForNextAwardLevel", null);
			}
			else if (professionName.Equals("scavenger"))
			{
				AwesomeProfessions.Data
					.WriteField($"{AwesomeProfessions.UniqueID}/ScavengerHuntStreak", null);
			}
			else if (professionName.Equals("prospector"))
			{
				AwesomeProfessions.Data
					.WriteField($"{AwesomeProfessions.UniqueID}/ProspectorHuntStreak", null);
			}
			else if (professionName.Equals("conservationist"))
			{
				AwesomeProfessions.Data
					.WriteField($"{AwesomeProfessions.UniqueID}/WaterTrashCollectedThisSeason", null)
					.WriteField($"{AwesomeProfessions.UniqueID}/ActiveTaxBonus", null);
			}
		}

		/// <summary>Get the price multiplier for wine and beverages sold by Oenologist.</summary>
		public static float GetOenologistPriceBonus()
		{
			uint currentLevel = AwesomeProfessions.Data.ReadField($"{AwesomeProfessions.UniqueID}/OenologyAwardLevel", uint.Parse);
			if (currentLevel > 0) return (float)(1f / Math.Pow(2, 5 - currentLevel));
			return 1f;
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
			foreach (int id in _LegendaryFishIds.Where(id => who.fishCaught.ContainsKey(id))) multiplier += 0.05f;
			return multiplier;
		}

		/// <summary>Get the price multiplier for items sold by Conservationist.</summary>
		/// <param name="who">The player.</param>
		public static float GetConservationistPriceMultiplier(Farmer who)
		{
			if (!who.IsLocalPlayer) return 1f;

			return 1f + AwesomeProfessions.Data.ReadField($"{AwesomeProfessions.UniqueID}/ActiveTaxBonus", float.Parse) / 100f;
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
			if (!LocalFarmerHasProfession("aquarist")) return 0;

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
			return (LocalFarmerHasProfession("scavenger") && ((obj.IsSpawnedObject && !IsForagedMineral(obj)) || obj.ParentSheetIndex == 590))
				|| (LocalFarmerHasProfession("prospector") && (IsResourceNode(obj) || IsForagedMineral(obj)));
		}
	}
}