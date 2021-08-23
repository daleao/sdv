using HarmonyLib;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Reflection;
using TheLion.Stardew.Common.Harmony;
using TheLion.Stardew.Professions.Framework.Extensions;
using SObject = StardewValley.Object;

namespace TheLion.Stardew.Professions.Framework.Patches
{
	internal class ObjectGetPriceAfterMultipliersPatch : BasePatch
	{
		/// <summary>Construct an instance.</summary>
		internal ObjectGetPriceAfterMultipliersPatch()
		{
			Original = typeof(SObject).MethodNamed(name: "getPriceAfterMultipliers");
			Prefix = new HarmonyMethod(GetType(), nameof(ObjectGetPriceAfterMultipliersPrefix));
		}

		#region harmony patches

		/// <summary>Patch to modify price multipliers for various modded professions.</summary>
		// ReSharper disable once RedundantAssignment
		[HarmonyPrefix]
		private static bool ObjectGetPriceAfterMultipliersPrefix(SObject __instance, ref float __result, float startPrice, long specificPlayerID)
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
					if (player.IsLocalPlayer && Game1.player.HasProfession("Artisan") && Util.Objects.IsArtisanGood(__instance))
						multiplier *= Util.Professions.GetArtisanPriceMultiplier();
					else if (player.HasProfession("Producer") && Util.Objects.IsAnimalProduct(__instance))
						multiplier *= Util.Professions.GetProducerPriceMultiplier(player);
					else if (player.HasProfession("Angler") && Util.Objects.IsFish(__instance))
						multiplier *= Util.Professions.GetAnglerPriceMultiplier(player);

					// events
					else if (player.eventsSeen.Contains(2120303) && Util.Objects.IsWildBerry(__instance))
						multiplier *= 3f;
					else if (player.eventsSeen.Contains(3910979) && Util.Objects.IsSpringOnion(__instance))
						multiplier *= 5f;

					// tax bonus
					if (Game1.player.HasProfession("Conservationist"))
						multiplier *= Util.Professions.GetConservationistPriceMultiplier(player);

					saleMultiplier = Math.Max(saleMultiplier, multiplier);
				}
			}
			catch (Exception ex)
			{
				ModEntry.Log($"Failed in {MethodBase.GetCurrentMethod().Name}:\n{ex}", LogLevel.Error);
				return true; // default to original logic
			}

			__result = startPrice * saleMultiplier;
			return false; // don't run original logic
		}

		#endregion harmony patches
	}
}