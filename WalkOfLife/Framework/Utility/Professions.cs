using StardewModdingAPI;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
			{ "Rancher", Farmer.rancher }, // 0
			{ "Breeder", Farmer.butcher }, // 2 (coopmaster)
			{ "Producer", Farmer.shepherd }, // 3

			{ "Harvester", Farmer.tiller }, // 1
			{ "Artisan", Farmer.artisan }, // 4
			{ "Agriculturist", Farmer.agriculturist }, // 5

			// fishing
			{ "Fisher", Farmer.fisher }, // 6
			{ "Angler", Farmer.angler }, // 8
			{ "Aquarist", Farmer.pirate }, // 9

			{ "Trapper", Farmer.trapper }, // 7
			{ "Luremaster", Farmer.baitmaster }, // 10
			{ "Conservationist", Farmer.mariner }, // 11
			/// Note: the vanilla game code has mariner and baitmaster IDs mixed up; i.e. effectively mariner is 10 and luremaster is 11.
			/// Since we are completely replacing both professions, we take the opportunity to fix this inconsistency.

			// foraging
			{ "Lumberjack", Farmer.forester }, // 12
			{ "Arborist", Farmer.lumberjack }, // 14
			{ "Tapper", Farmer.tapper }, // 15

			{ "Forager", Farmer.gatherer }, // 13
			{ "Ecologist", Farmer.botanist }, // 16
			{ "Scavenger", Farmer.tracker }, // 17

			// mining
			{ "Miner", Farmer.miner }, // 18
			{ "Spelunker", Farmer.blacksmith }, // 20
			{ "Prospector", Farmer.burrower }, // 21 (prospector)

			{ "Blaster", Farmer.geologist }, // 19
			{ "Demolitionist", Farmer.excavator }, // 22
			{ "Gemologist", Farmer.gemologist }, // 23

			// combat
			{ "Fighter", Farmer.fighter }, // 24
			{ "Brute", Farmer.brute }, // 26
			{ "Poacher", Farmer.defender }, // 27

			{ "Rascal", Farmer.scout }, // 25
			{ "Piper", Farmer.acrobat }, // 28
			{ "Desperado", Farmer.desperado } // 29
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
				b.buildingType.Contains("Deluxe") && ((AnimalHouse)b.indoors.Value).isFull()).Sum(_ => 0.05f);
		}

		/// <summary>Get the price multiplier for fish sold by Angler.</summary>
		/// <param name="who">The player.</param>
		public static float GetAnglerPriceMultiplier(Farmer who)
		{
			var fishData = Game1.content.Load<Dictionary<int, string>>(Path.Combine("Data", "Fish"))
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

		/// <summary>Get the bonus critical strike chance that should be applied to Poacher.</summary>
		public static float GetPoacherBonusCritChance()
		{
			//var healthPercent = (double)who.health / who.maxHealth;
			//return ModEntry.IsSuperModeActive && ModEntry.SuperModeIndex == Util.Professions.IndexOf("Poacher") ? 0.5f : (float)Math.Min(0.2 / (healthPercent + 0.2) - 0.2 / 1.2, 0.5f);
			//return ModEntry.IsSuperModeActive && ModEntry.SuperModeIndex == Util.Professions.IndexOf("Poacher") ? 0.1f : (float)(1.0 / 9.0 * healthPercent - 1.0 / 90.0);
			//return ModEntry.IsSuperModeActive && ModEntry.SuperModeIndex == Util.Professions.IndexOf("Poacher") ? 2f : (float)Math.Max(-18.0 / (healthPercent + 3.5) + 6.0, 1f);
			return 0.1f;
		}

		/// <summary>Get the bonus critical strike damage that should be applied to Poacher.</summary>
		/// <param name="who">The player.</param>
		public static float GetPoacherCritDamageMultiplier(Farmer who)
		{
			var healthPercent = (double)who.health / who.maxHealth;
			return ModEntry.IsSuperModeActive && ModEntry.SuperModeIndex == IndexOf("Poacher")
				? 2f
				: (float)Math.Max(-18.0 / (-healthPercent + 4.6) + 6.0, 1f);
		}

		/// <summary>Get the bonus item drop chance for Poacher.</summary>
		/// <param name="who">The player.</param>
		public static float GetPoacherStealChance(Farmer who)
		{
			return (ModEntry.IsSuperModeActive && ModEntry.SuperModeIndex == IndexOf("Poacher")
					   ? 0.25f
					   : ModEntry.SuperModeCounter / 10 * 0.005f) *
				   ((who.CurrentTool as MeleeWeapon)?.type.Value == MeleeWeapon.dagger ? 1.5f : 1f);
		}

		///// <summary>Get the chance to instant-kill an enemy for Poacher.</summary>
		///// <param name="who">The player.</param>
		//public static float GetPoacherAssassinationChance(MeleeWeapon weapon, Farmer who)
		//{
		//	//var critChance = weapon.critChance.Value + (1f + who.critChanceModifier) * Util.Professions.GetPoacherBonusCritChance(who);
		//	var critPower = weapon.critMultiplier.Value * (1f + who.critPowerModifier) * GetPoacherCritDamageMultiplier(who);
		//	return (critPower - 3f) / 6f + 0.1f;
		//}

		/// <summary>Get bonus slingshot damage as function of projectile travel distance.</summary>
		/// <param name="travelDistance">Distance travelled by the projectile.</param>
		public static float GetRascalBonusDamageForTravelTime(int travelDistance)
		{
			const int MAX_DISTANCE = 800;
			if (travelDistance > MAX_DISTANCE) return 1.5f;
			return 1f + 0.5f / MAX_DISTANCE * travelDistance;
		}

		/// <summary>Get the slingshot charge time modifier for Desperado.</summary>
		public static float GetDesperadoChargeTime()
		{
			return 0.3f * GetCooldownOrChargeTimeReduction();
		}

		/// <summary>Get the chance of Desperado double strafe.</summary>
		public static float GetDesperadoDoubleStrafeChance()
		{
			return ModEntry.IsSuperModeActive && ModEntry.SuperModeIndex == IndexOf("Desperado")
				? 0f
				: ModEntry.SuperModeCounter / 10 * 0.005f;
		}

		/// <summary>Get the attack speed multiplier that should be applied to Piper Slimes.</summary>
		public static int GetPiperSlimeSpawnAttempts()
		{
			return ModEntry.SuperModeCounter / 50 + 1;
		}

		/// <summary>Get the attack speed multiplier that should be applied to Piper Slimes.</summary>
		public static float GetPiperSlimeAttackSpeedModifier()
		{
			return ModEntry.IsSuperModeActive && ModEntry.SuperModeIndex == IndexOf("Piper")
				? 0.15f
				: ModEntry.SuperModeCounter / 10 * 0.003f;
		}

		/// <summary>Get the cooldown reduction multiplier that should be applied to Brute or Poacher cooldown reductions and Desperado charge time.</summary>
		public static float GetCooldownOrChargeTimeReduction()
		{
			return ModEntry.IsSuperModeActive ? 0.5f : 1f - ModEntry.SuperModeCounter / 10 * 0.01f;
		}

		#endregion public methods
	}
}