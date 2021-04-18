using Harmony;
using StardewValley;
using System;

namespace TheLion.AwesomeProfessions
{
	internal class FarmAnimalGetSellPricePatch : BasePatch
	{
		/// <inheritdoc/>
		public override void Apply(HarmonyInstance harmony)
		{
			harmony.Patch(
				original: AccessTools.Method(typeof(FarmAnimal), nameof(FarmAnimal.getSellPrice)),
				prefix: new HarmonyMethod(GetType(), nameof(FarmAnimalGetSellPricePrefix))
			);
		}

		#region harmony patches

		/// <summary>Patch to adjust Breeder animal sell price.</summary>
		private static bool FarmAnimalGetSellPricePrefix(ref FarmAnimal __instance, ref int __result)
		{
			double adjustedFriendship;
			try
			{
				var owner = Game1.getFarmer(__instance.ownerID.Value);
				if (!Utility.SpecificPlayerHasProfession("Breeder", owner)) return true; // run original logic

				adjustedFriendship = Utility.GetProducerAdjustedFriendship(__instance);
			}
			catch (Exception ex)
			{
				Monitor.Log($"Failed in {nameof(FarmAnimalGetSellPricePrefix)}:\n{ex}");
				return true; // default to original logic
			}

			__result = (int)(__instance.price.Value * adjustedFriendship);
			return false; // don't run original logic
		}

		#endregion harmony patches
	}
}