using System;
using System.Collections.Generic;
using System.Linq;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Tools;
using TheLion.Stardew.Common.Classes;
using TheLion.Stardew.Common.Extensions;
using SObject = StardewValley.Object;

// ReSharper disable PossibleLossOfFraction

namespace TheLion.Stardew.Professions.Framework.Util
{
	/// <summary>Holds common methods and properties related to specific professions.</summary>
	public static class Professions
	{
		#region look-up table

		public static BiMap<string, int> IndexByName { get; } = new()
		{
			// farming
			{"Rancher", Farmer.rancher}, // 0
			{"Breeder", Farmer.butcher}, // 2 (coopmaster)
			{"Producer", Farmer.shepherd}, // 3

			{"Harvester", Farmer.tiller}, // 1
			{"Artisan", Farmer.artisan}, // 4
			{"Agriculturist", Farmer.agriculturist}, // 5

			// fishing
			{"Fisher", Farmer.fisher}, // 6
			{"Angler", Farmer.angler}, // 8
			{"Aquarist", Farmer.pirate}, // 9

			{"Trapper", Farmer.trapper}, // 7
			{"Luremaster", Farmer.baitmaster}, // 10
			{"Conservationist", Farmer.mariner}, // 11
			/// Note: the vanilla game code has mariner and baitmaster IDs mixed up; i.e. effectively mariner is 10 and luremaster is 11.
			/// Since we are completely replacing both professions, we take the opportunity to fix this inconsistency.

			// foraging
			{"Lumberjack", Farmer.forester}, // 12
			{"Arborist", Farmer.lumberjack}, // 14
			{"Tapper", Farmer.tapper}, // 15

			{"Forager", Farmer.gatherer}, // 13
			{"Ecologist", Farmer.botanist}, // 16
			{"Scavenger", Farmer.tracker}, // 17

			// mining
			{"Miner", Farmer.miner}, // 18
			{"Spelunker", Farmer.blacksmith}, // 20
			{"Prospector", Farmer.burrower}, // 21 (prospector)

			{"Blaster", Farmer.geologist}, // 19
			{"Demolitionist", Farmer.excavator}, // 22
			{"Gemologist", Farmer.gemologist}, // 23

			// combat
			{"Fighter", Farmer.fighter}, // 24
			{"Brute", Farmer.brute}, // 26
			{"Poacher", Farmer.defender}, // 27

			{"Rascal", Farmer.scout}, // 25
			{"Piper", Farmer.acrobat}, // 28
			{"Desperado", Farmer.desperado} // 29
		};

		#endregion look-up table

		#region public methods

		/// <summary>Get the index of a given profession by name.</summary>
		/// <param name="professionName">Case-sensitive profession name.</param>
		public static int IndexOf(string professionName)
		{
			if (IndexByName.Forward.TryGetValue(professionName, out var professionIndex)) return professionIndex;
			throw new ArgumentException($"Profession {professionName} does not exist.");
		}

		/// <summary>Get the name of a given profession by index.</summary>
		/// <param name="professionIndex">The index of the profession.</param>
		public static string NameOf(int professionIndex)
		{
			if (IndexByName.Reverse.TryGetValue(professionIndex, out var professionName)) return professionName;
			throw new IndexOutOfRangeException($"Index {professionIndex} is not a valid profession index.");
		}

		/// <summary>Get the price multiplier for produce sold by Producer.</summary>
		/// <param name="who">The player.</param>
		public static float GetProducerPriceMultiplier(Farmer who)
		{
			return 1f + Game1.getFarm().buildings.Where(b =>
				(b.owner.Value == who.UniqueMultiplayerID || !Context.IsMultiplayer) &&
				b.buildingType.Contains("Deluxe") && ((AnimalHouse) b.indoors.Value).isFull()).Sum(_ => 0.05f);
		}

		/// <summary>Get the price multiplier for fish sold by Angler.</summary>
		/// <param name="who">The player.</param>
		public static float GetAnglerPriceMultiplier(Farmer who)
		{
			var fishData = Game1.content.Load<Dictionary<int, string>>(PathUtilities.NormalizeAssetName("Data/Fish"))
				.Where(p => !p.Key.AnyOf(152, 152, 157) && !p.Value.Contains("trap"))
				.ToDictionary(p => p.Key, p => p.Value);
			var multiplier = 1f;
			foreach (var p in who.fishCaught.Pairs)
			{
				if (!fishData.TryGetValue(p.Key, out var specificFishData)) continue;

				var dataFields = specificFishData.Split('/');
				if (Objects.LegendaryFishNames.Contains(dataFields[0]))
					multiplier += 0.05f;
				else if (p.Value[1] >= Convert.ToInt32(dataFields[4]))
					multiplier += 0.01f;
			}

			return multiplier;
		}

		/// <summary>Get the price multiplier for items sold by Conservationist.</summary>
		public static float GetConservationistPriceMultiplier()
		{
			return 1f + ModEntry.Data.ReadField<float>("ActiveTaxBonusPercent");
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
			var itemsForaged = ModEntry.Data.ReadField<uint>("ItemsForaged");
			return itemsForaged < ModEntry.Config.ForagesNeededForBestQuality
				? itemsForaged < ModEntry.Config.ForagesNeededForBestQuality / 2 ? SObject.medQuality :
				SObject.highQuality
				: SObject.bestQuality;
		}

		/// <summary>Get the quality of mineral for Gemologist.</summary>
		public static int GetGemologistMineralQuality()
		{
			var mineralsCollected = ModEntry.Data.ReadField<uint>("MineralsCollected");
			return mineralsCollected < ModEntry.Config.MineralsNeededForBestQuality
				? mineralsCollected < ModEntry.Config.MineralsNeededForBestQuality / 2
					? SObject.medQuality
					: SObject.highQuality
				: SObject.bestQuality;
		}

		/// <summary>Get the bonus ladder spawn chance for Spelunker.</summary>
		public static double GetSpelunkerBonusLadderDownChance()
		{
			return ModEntry.SpelunkerLadderStreak * 0.01;
		}

		/// <summary>Get the bonus bobber bar height for Aquarist.</summary>
		public static int GetAquaristBonusBobberBarHeight()
		{
			return Game1.getFarm().buildings.Where(b =>
				(b.owner.Value == Game1.player.UniqueMultiplayerID || !Context.IsMultiplayer) && b is FishPond
				{
					FishCount: >= 12
				}).Sum(_ => 6);
		}

		/// <summary>Get the bonus raw damage that should be applied to Brute.</summary>
		/// <param name="who">The player.</param>
		public static float GetBruteBonusDamageMultiplier(Farmer who)
		{
			return 1.15f +
			       (who.IsLocalPlayer && ModEntry.IsSuperModeActive && ModEntry.SuperModeIndex == IndexOf("Brute")
				       ? 0.65f + who.attackIncreaseModifier +
				         (who.CurrentTool != null ? who.CurrentTool.GetEnchantmentLevel<RubyEnchantment>() * 0.1f : 0f)
				       : ModEntry.SuperModeCounter / 10 * 0.005f) *
			       ((who.CurrentTool as MeleeWeapon)?.type.Value == MeleeWeapon.club ? 1.5f : 1f);
		}

		///// <summary>Get the bonus critical strike chance that should be applied to Poacher.</summary>
		///// <param name="who">The player.</param>
		//public static float GetPoacherBonusCritChance()
		//{
		//	var healthPercent = (double)who.health / who.maxHealth;
		//	var bonusCrit = (float)Math.Max(-1.8 / (healthPercent - 4.6) - 0.4, 0f);
		//	if (!who.IsLocalPlayer || ModEntry.SuperModeIndex != IndexOf("Poacher") || healthPercent >= 0.5) return bonusCrit;

		//	bonusCrit += ModEntry.SuperModeCounter * 0.0005f;
		//	return bonusCrit;
		//}

		/// <summary>Get the bonus critical strike damage that should be applied to Poacher.</summary>
		/// <param name="who">The player.</param>
		public static float GetPoacherCritDamageMultiplier()
		{
			//var healthPercent = (double) who.health / who.maxHealth;
			//var multiplier = (float)Math.Min(-18.0 / (-healthPercent + 4.6) + 6.0, 2f);
			//if (!who.IsLocalPlayer || ModEntry.SuperModeIndex != IndexOf("Poacher") || healthPercent < 0.5) return multiplier;

			//multiplier += ModEntry.SuperModeCounter * 0.0005f;
			//return multiplier;

			return ModEntry.IsSuperModeActive ? 3f : ModEntry.SuperModeCounter / 10 * 0.06f;
		}

		/// <summary>Get bonus slingshot damage as function of projectile travel distance.</summary>
		/// <param name="travelDistance">Distance travelled by the projectile.</param>
		public static float GetRascalBonusDamageForTravelTime(int travelDistance)
		{
			const int MAX_DISTANCE_I = 800;
			if (travelDistance > MAX_DISTANCE_I) return 1.5f;
			return 1f + 0.5f / MAX_DISTANCE_I * travelDistance;
		}

		/// <summary>Get the multiplier that will be applied to Desperado bullet speed, knockback and hitbox.</summary>
		public static float GetDesperadoBulletPower()
		{
			return 1f + (ModEntry.IsSuperModeActive
				? 1f
				: ModEntry.SuperModeCounter / 10 * 0.01f);
		}

		/// <summary>Get the slingshot charge time modifier for Desperado.</summary>
		public static float GetDesperadoChargeTime()
		{
			return 0.3f * GetCooldownOrChargeTimeReduction();
		}

		/// <summary>Get the maximum number of bonus Slimes attracted by Piper.</summary>
		public static int GetPiperSlimeSpawnAttempts()
		{
			return ModEntry.IsSuperModeActive
				? 11
				: ModEntry.SuperModeCounter / 50 + 1;
		}

		/// <summary>Get the attack speed multiplier that should be applied to Piper Slimes.</summary>
		public static float GetPiperSlimeAttackSpeedModifier()
		{
			return ModEntry.IsSuperModeActive
				? 0.15f
				: ModEntry.SuperModeCounter / 10 * 0.003f;
		}

		/// <summary>
		///     Get the cooldown reduction multiplier that should be applied to Brute or Poacher cooldown reductions and
		///     Desperado charge time.
		/// </summary>
		public static float GetCooldownOrChargeTimeReduction()
		{
			return ModEntry.IsSuperModeActive
				? 0.5f
				: 1f - ModEntry.SuperModeCounter / 10 * 0.01f;
		}

		#endregion public methods
	}
}