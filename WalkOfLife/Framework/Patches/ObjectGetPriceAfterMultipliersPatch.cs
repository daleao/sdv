using Harmony;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Buildings;
using System;
using System.Collections.Generic;
using System.Linq;
using TheLion.Common.Extensions;
using SObject = StardewValley.Object;

namespace TheLion.AwesomeProfessions.Framework.Patches
{
	internal class ObjectGetPriceAfterMultipliersPatch : BasePatch
	{
		#region private fields
		/// <summary>Set of id's corresponding to animal produce or derived artisan goods.</summary>
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

		/// <summary>Set of id's corresponding to legendary fish.</summary>
		private static readonly IEnumerable<int> _legendaryFishIds = new HashSet<int>
		{
			159,	// crimsonfish
			160,	// angler
			163,	// legend
			682,	// mutant carp
			775,	// glacierfish
			898,	// son of crimsonfish
			899,	// ms. angler
			900,	// legend ii
			901,	// radioactive carp
			902		// glacierfish jr.
		};
		#endregion  private fields

		/// <summary>Construct an instance.</summary>
		/// <param name="monitor">Interface for writing to the SMAPI console.</param>
		internal ObjectGetPriceAfterMultipliersPatch(IMonitor monitor)
		: base(monitor) { }

		/// <summary>Apply internally-defined Harmony patches.</summary>
		/// <param name="harmony">The Harmony instance for this mod.</param>
		protected internal override void Apply(HarmonyInstance harmony)
		{
			harmony.Patch(
				AccessTools.Method(typeof(SObject), name: "getPriceAfterMultipliers"),
				prefix: new HarmonyMethod(GetType(), nameof(ObjectGetPriceAfterMultipliersPrefix))
			);
		}

		#region harmony patches
		/// <summary>Patch to modify price multipliers for various modded professions.</summary>
		protected static bool ObjectGetPriceAfterMultipliersPrefix(ref SObject __instance, ref float __result, float startPrice, long specificPlayerID)
		{
			float saleMultiplier = 1f;

			foreach (Farmer player in Game1.getAllFarmers())
			{
				if (Game1.player.useSeparateWallets)
				{
					if (specificPlayerID == -1)
					{
						if (player.UniqueMultiplayerID != Game1.player.UniqueMultiplayerID || !player.isActive()) continue;
					}
					else if (player.UniqueMultiplayerID != specificPlayerID) continue;
				}
				else if (!player.isActive()) continue;

				float multiplier = 1f;

				// professions
				if (Globals.SpecificPlayerHasProfession("oenologist", player) && _IsWineOrBeverage(__instance))
					multiplier *= _GetPriceMultiplierForOenologist(player);
				else if (Globals.SpecificPlayerHasProfession("producer", player) && _IsAnimalProduct(__instance))
					multiplier *= _GetPriceMultiplierForProducer(player);
				else if (Globals.SpecificPlayerHasProfession("angler", player) && _IsReeledFish(__instance))
					multiplier *= _GetPriceMultiplierForAngler(player);

				// events
				else if (player.eventsSeen.Contains(2120303) && _IsWildBerry(__instance))
					multiplier *= 3f;
				else if (player.eventsSeen.Contains(3910979) && __instance.ParentSheetIndex == 399) // spring onion
					multiplier *= 5f;

				if (Globals.SpecificPlayerHasProfession("conservationist", player))
					multiplier *= _GetPriceMultiplierForConservationist(player);

				saleMultiplier = Math.Max(saleMultiplier, multiplier);
			}

			__result = startPrice * saleMultiplier;
			return false; // don't run original logic
		}
		#endregion harmony patches

		#region private methods
		/// <summary>Get the price multiplier for wine sold by Oenologist.</summary>
		/// <param name="who">The player.</param>
		private static float _GetPriceMultiplierForOenologist(Farmer who)
		{
			if (!who.IsLocalPlayer) return 1f;

			float multiplier = 1f;
			if (AwesomeProfessions.Data.WineFameAccrued >= 5000) multiplier += 1f;
			else if (AwesomeProfessions.Data.WineFameAccrued >= 3125) multiplier += 0.5f;
			else if (AwesomeProfessions.Data.WineFameAccrued >= 1250) multiplier += 0.25f;
			else if (AwesomeProfessions.Data.WineFameAccrued >= 500) multiplier += 0.1f;
			else if (AwesomeProfessions.Data.WineFameAccrued >= 200) multiplier += 0.05f;

			return multiplier;
		}

		/// <summary>Get the price multiplier for produce sold by Producer.</summary>
		/// <param name="who">The player.</param>
		private static float _GetPriceMultiplierForProducer(Farmer who)
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
		private static float _GetPriceMultiplierForAngler(Farmer who)
		{
			float multiplier = 1f;
			foreach (int id in _legendaryFishIds)
			{
				if (who.fishCaught.ContainsKey(id))
					multiplier += 0.05f;
			}

			return multiplier;
		}

		/// <summary>Get the price multiplier for items sold by Conservationist.</summary>
		/// <param name="who">The player.</param>
		private static float _GetPriceMultiplierForConservationist(Farmer who)
		{
			if (!who.IsLocalPlayer) return 1f;

			return 1f + AwesomeProfessions.Data.OceanTrashCollectedThisSeason % Globals.TrashNeededForNextTaxLevel / 100f;
		}

		/// <summary>Whether a given object is one of wine, juice, beer, mead or pale ale.</summary>
		/// <param name="obj">The given object.</param>
		private static bool _IsWineOrBeverage(SObject obj)
		{
			int wine = 348, pale_ale = 303, beer = 346, juice = 350, mead = 459;
			return obj != null && obj.ParentSheetIndex.AnyOf(wine, pale_ale, beer, juice, mead);
		}

		/// <summary>Whether a given object is an animal produce or derived artisan good.</summary>
		/// <param name="obj">The given object.</param>
		private static bool _IsAnimalProduct(SObject obj)
		{
			return obj != null && _animalProductIds.Contains(obj.ParentSheetIndex);
		}

		/// <summary>Whether a given object is a fish trapped by a crab pot.</summary>
		/// <param name="obj">The given object.</param>
		private static bool _IsTrapFish(SObject obj)
		{
			return obj.ParentSheetIndex > 714 && obj.ParentSheetIndex < 724;
		}

		/// <summary>Whether a given object is a fish caught with a fishing rod.</summary>
		/// <param name="obj">The given object.</param>
		private static bool _IsReeledFish(SObject obj)
		{
			return obj.Category == SObject.FishCategory && !_IsTrapFish(obj);
		}

		/// <summary>Whether a given object is salmonberry or blackberry.</summary>
		/// <param name="obj">The given object.</param>
		private static bool _IsWildBerry(SObject obj)
		{
			return obj?.ParentSheetIndex == 296 || obj?.ParentSheetIndex == 410;
		}
		#endregion private methods
	}
}
