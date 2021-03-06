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

		/// <summary>Construct an instance.</summary>
		/// <param name="config">The mod settings.</param>
		/// <param name="monitor">Interface for writing to the SMAPI console.</param>
		internal ObjectGetPriceAfterMultipliersPatch(ModConfig config, IMonitor monitor)
		: base(config, monitor) { }

		/// <summary>Apply internally-defined Harmony patches.</summary>
		/// <param name="harmony">The Harmony instance for this mod.</param>
		protected internal override void Apply(HarmonyInstance harmony)
		{
			harmony.Patch(
				AccessTools.Method(typeof(SObject), name: "getPriceAfterMultipliers"),
				prefix: new HarmonyMethod(GetType(), nameof(ObjectGetPriceAfterMultipliersPrefix))
			);
		}

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
						if (player.UniqueMultiplayerID != Game1.player.UniqueMultiplayerID || !player.isActive())
						{
							continue;
						}
					}
					else if (player.UniqueMultiplayerID != specificPlayerID)
					{
						continue;
					}
				}
				else if (!player.isActive())
				{
					continue;
				}

				float multiplier = 1f;

				// professions
				if (Utils.PlayerHasProfession("producer", player) && _IsAnimalProduct(__instance))
				{
					multiplier *= _GetMultiplierForProducer();
				}
				if (Utils.PlayerHasProfession("oenologist", player) && _IsWineOrBeverage(__instance))
				{
					multiplier *= _GetMultiplierForProducer();
				}
				if (Utils.PlayerHasProfession("angler", player) && IsReeledFish(__instance))
				{
					multiplier *= _GetMultiplierForProducer();
				}
				if (Utils.PlayerHasProfession("conservationist", player))
				{
					multiplier *= _GetMultiplierForProducer();
				}

				// events
				if (player.eventsSeen.Contains(2120303) && (__instance.ParentSheetIndex == 296 || __instance.ParentSheetIndex == 410))
				{
					multiplier *= 3f;
				}
				if (player.eventsSeen.Contains(3910979) && __instance.ParentSheetIndex == 399)
				{
					multiplier *= 5f;
				}

				saleMultiplier = Math.Max(saleMultiplier, multiplier);
			}

			__result = startPrice * saleMultiplier;
			return false; // don't run original logic
		}

		private static float _GetMultiplierForProducer()
		{
			float multiplier = 1;
			foreach (Building b in Game1.getFarm().buildings)
			{
				if ((b.owner.Equals(Game1.player.UniqueMultiplayerID) || !Game1.IsMultiplayer) && b.buildingType.Contains("Deluxe") && (b.indoors.Value as AnimalHouse).isFull())
				{
					multiplier += 0.05f;
				}
			}

			return multiplier;
		}

		private static float _GetMultiplierForOenologist()
		{
			float multiplier = 1;
			if (ModEntry.Data.WineFameAccrued >= 1000)
			{
				multiplier *= 1.5f;
			}
			else if (ModEntry.Data.WineFameAccrued >= 800)
			{
				multiplier *= 1.3f;
			}
			else if (ModEntry.Data.WineFameAccrued >= 600)
			{
				multiplier *= 1.2f;
			}
			else if (ModEntry.Data.WineFameAccrued >= 400)
			{
				multiplier *= 1.15f;
			}
			else if (ModEntry.Data.WineFameAccrued >= 200)
			{
				multiplier *= 1.1f;
			}
			return multiplier;
		}

		private static float _GetMultiplierForAngler()
		{
			float multiplier = 1;

			return multiplier;
		}

		private static float _GetMultiplierForConservationist()
		{
			float multiplier = 1;

			return multiplier;
		}

		/// <summary>Whether a given object is one of wine, juice, beer, mead or pale ale.</summary>
		/// <param name="obj">The given object.</param>
		private static bool _IsWineOrBeverage(SObject obj)
		{
			int wine = 348, pale_ale = 303, beer = 346, juice = 350, mead = 459;
			return obj.ParentSheetIndex.AnyOf(wine, pale_ale, beer, juice, mead);
		}

		/// <summary>Whether a given object is an animal produce or derived artisan good.</summary>
		/// <param name="obj">The given object.</param>
		private static bool _IsAnimalProduct(SObject obj)
		{
			return _animalProductIds.Contains(obj.ParentSheetIndex);
		}
	}
}
