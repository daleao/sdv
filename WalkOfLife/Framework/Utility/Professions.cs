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

namespace TheLion.Stardew.Professions.Framework.Utility
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

		/// <summary>Affects the price of produce sold by Producer.</summary>
		/// <param name="who">The player.</param>
		public static float GetProducerPriceMultiplier(Farmer who)
		{
			return 1f + Game1.getFarm().buildings.Where(b =>
				(b.owner.Value == who.UniqueMultiplayerID || !Context.IsMultiplayer) &&
				b.buildingType.Contains("Deluxe") && ((AnimalHouse) b.indoors.Value).isFull()).Sum(_ => 0.05f);
		}

		/// <summary>Affects the price of fish sold by Angler.</summary>
		/// <param name="who">The player.</param>
		public static float GetAnglerPriceMultiplier(Farmer who)
		{
			var fishData = Game1.content.Load<Dictionary<int, string>>(PathUtilities.NormalizeAssetName("Data/Fish"))
				.Where(p => !p.Key.IsAnyOf(152, 152, 157) && !p.Value.Contains("trap"))
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

		/// <summary>Affects the price all items sold by Conservationist.</summary>
		public static float GetConservationistPriceMultiplier()
		{
			return 1f + ModEntry.Data.Read<float>("ActiveTaxBonusPercent");
		}

		/// <summary>Affects the price of animals sold by Breeder.</summary>
		/// <param name="a">Farm animal instance.</param>
		public static double GetProducerAdjustedFriendship(FarmAnimal a)
		{
			return Math.Pow(Math.Sqrt(2) * a.friendshipTowardFarmer.Value / 1000, 2) + 0.5;
		}

		/// <summary>Affects the quality of items foraged by Ecologist.</summary>
		public static int GetEcologistForageQuality()
		{
			var itemsForaged = ModEntry.Data.Read<uint>("ItemsForaged");
			return itemsForaged < ModEntry.Config.ForagesNeededForBestQuality
				? itemsForaged < ModEntry.Config.ForagesNeededForBestQuality / 2
					? SObject.medQuality
					: SObject.highQuality
				: SObject.bestQuality;
		}

		/// <summary>Affects the quality of minerals collected by Gemologist.</summary>
		public static int GetGemologistMineralQuality()
		{
			var mineralsCollected = ModEntry.Data.Read<uint>("MineralsCollected");
			return mineralsCollected < ModEntry.Config.MineralsNeededForBestQuality
				? mineralsCollected < ModEntry.Config.MineralsNeededForBestQuality / 2
					? SObject.medQuality
					: SObject.highQuality
				: SObject.bestQuality;
		}

		/// <summary>Affects that chance that a ladder or shaft will spawn for Spelunker.</summary>
		public static double GetSpelunkerBonusLadderDownChance()
		{
			return ModState.SpelunkerLadderStreak * 0.01;
		}

		/// <summary>Affects the size of the green fishing bar for Aquarist.</summary>
		public static int GetAquaristBonusBobberBarHeight()
		{
			return Game1.getFarm().buildings.Where(b =>
				(b.owner.Value == Game1.player.UniqueMultiplayerID || !Context.IsMultiplayer) && b is FishPond
				{
					FishCount: >= 12
				}).Sum(_ => 6);
		}

		/// <summary>Affects the raw damage dealt by Brute.</summary>
		/// <param name="who">The player.</param>
		public static float GetBruteBonusDamageMultiplier(Farmer who)
		{
			return 1.15f +
			       (who.IsLocalPlayer && ModState.IsSuperModeActive && ModState.SuperModeIndex == IndexOf("Brute")
				       ? 0.65f + who.attackIncreaseModifier +
				         (who.CurrentTool is not null
					         ? who.CurrentTool.GetEnchantmentLevel<RubyEnchantment>() * 0.1f
					         : 0f)
				       : ModState.SuperModeGaugeValue / 10 * 0.005f) *
			       ((who.CurrentTool as MeleeWeapon)?.type.Value == MeleeWeapon.club ? 1.5f : 1f);
		}

		//public static float GetPoacherBonusCritChance()
		//{
		//	var healthPercent = (double)who.health / who.maxHealth;
		//	var bonusCrit = (float)Math.Max(-1.8 / (healthPercent - 4.6) - 0.4, 0f);
		//}

		/// <summary>Affecsts the powerof critical strikes performed by Poacher.</summary>
		public static float GetPoacherCritDamageMultiplier()
		{
			//var healthPercent = (double) who.health / who.maxHealth;
			//var multiplier = (float)Math.Min(-18.0 / (-healthPercent + 4.6) + 6.0, 2f);
			return 1f + (ModState.IsSuperModeActive ? 2f : ModState.SuperModeGaugeValue / 10 * 0.04f);
		}

		/// <summary>Affects the damage of projectiles fired by Rascal.</summary>
		/// <param name="travelDistance">Distance travelled by the projectile.</param>
		public static float GetRascalBonusDamageForTravelTime(int travelDistance)
		{
			const int MAX_DISTANCE_I = 800;
			if (travelDistance > MAX_DISTANCE_I) return 1.5f;
			return 1f + 0.5f / MAX_DISTANCE_I * travelDistance;
		}

		/// <summary>Affects the chance to shoot twice consecutively for Desperado.</summary>
		/// <param name="who">The player.</param>
		public static float GetDesperadoDoubleStrafeChance(Farmer who)
		{
			var healthPercent = (double) who.health / who.maxHealth;
			return (float) Math.Min(2 / (healthPercent + 1.5) - 0.75, 0.5f);
		}

		/// <summary>Affects projectile velocity, knockback, hitbox size and pierce chance for Desperado.</summary>
		public static float GetDesperadoBulletPower()
		{
			return 1f + (ModState.IsSuperModeActive
				? 1f
				: ModState.SuperModeGaugeValue / 10 * 0.01f);
		}

		/// <summary>Affects the time to prepare a shot for Desperado.</summary>
		public static float GetDesperadoChargeTime()
		{
			return 0.3f * GetCooldownOrChargeTimeReduction();
		}

		/// <summary>Affects the maximum number of bonus Slimes that can be attracted by Piper.</summary>
		public static int GetPiperSlimeSpawnAttempts()
		{
			return ModState.IsSuperModeActive
				? 11
				: ModState.SuperModeGaugeValue / 50 + 1;
		}

		/// <summary>Affects the attack frequency of Slimes under Piper influence towards other enemies.</summary>
		/// <returns>Returns a number between 0 (when <see cref="ModState.SuperModeGaugeValue" /> is 0, and 0.15 when it is full.</returns>
		public static float GetPiperSlimeAttackSpeedModifier()
		{
			return ModState.IsSuperModeActive
				? 0.75f
				: 1f - ModState.SuperModeGaugeValue / 10 * 0.003f;
		}

		/// <summary>
		///     Affects the cooldown of Club or Hammer special attacks for Brute and Poacher, or the pull-back time of shots
		///     for Desperado.
		/// </summary>
		/// <returns>Returns a number between 1 (when <see cref="ModState.SuperModeGaugeValue" /> is 0, and 0.5 when it is full.</returns>
		public static float GetCooldownOrChargeTimeReduction()
		{
			return ModState.IsSuperModeActive
				? 0.5f
				: 1f - ModState.SuperModeGaugeValue / 10 * 0.01f;
		}

		#endregion public methods
	}
}