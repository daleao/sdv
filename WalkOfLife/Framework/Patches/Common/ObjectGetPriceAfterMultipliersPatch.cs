using Harmony;
using StardewValley;
using System;
using SObject = StardewValley.Object;

namespace TheLion.AwesomeProfessions
{
	internal class ObjectGetPriceAfterMultipliersPatch : BasePatch
	{
		/// <inheritdoc/>
		public override void Apply(HarmonyInstance harmony)
		{
			harmony.Patch(
				original: AccessTools.Method(typeof(SObject), name: "getPriceAfterMultipliers"),
				prefix: new HarmonyMethod(GetType(), nameof(ObjectGetPriceAfterMultipliersPrefix))
			);
		}

		#region harmony patches

		/// <summary>Patch to modify price multipliers for various modded professions.</summary>
		// ReSharper disable once RedundantAssignment
		private static bool ObjectGetPriceAfterMultipliersPrefix(ref SObject __instance, ref float __result, float startPrice, long specificPlayerID)
		{
			var saleMultiplier = 1f;
			try
			{
				foreach (var player in Game1.getAllFarmers())
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

					var multiplier = 1f;

					// professions
					if (player.IsLocalPlayer && Utility.LocalPlayerHasProfession("Artisan") && Utility.IsArtisanGood(__instance))
						multiplier *= Utility.GetArtisanPriceMultiplier();
					else if (Utility.SpecificPlayerHasProfession("Producer", player) && Utility.IsAnimalProduct(__instance))
						multiplier *= Utility.GetProducerPriceMultiplier(player);
					else if (Utility.SpecificPlayerHasProfession("Angler", player) && Utility.IsFish(__instance))
						multiplier *= Utility.GetAnglerPriceMultiplier(player);

					// events
					else if (player.eventsSeen.Contains(2120303) && Utility.IsWildBerry(__instance))
						multiplier *= 3f;
					else if (player.eventsSeen.Contains(3910979) && Utility.IsSpringOnion(__instance))
						multiplier *= 5f;

					// tax bonus
					if (Utility.LocalPlayerHasProfession("Conservationist"))
						multiplier *= Utility.GetConservationistPriceMultiplier(player);

					saleMultiplier = Math.Max(saleMultiplier, multiplier);
				}
			}
			catch (Exception ex)
			{
				Monitor.Log($"Failed in {nameof(ObjectGetPriceAfterMultipliersPrefix)}:\n{ex}");
				return true; // default to original logic
			}
			
			__result = startPrice * saleMultiplier;
			return false; // don't run original logic
		}

		#endregion harmony patches
	}
}