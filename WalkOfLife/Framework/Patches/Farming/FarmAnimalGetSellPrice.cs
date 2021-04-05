using Harmony;
using StardewValley;

namespace TheLion.AwesomeProfessions
{
	internal class FarmAnimalGetSellPricePatch : BasePatch
	{
		/// <inheritdoc/>
		public override void Apply(HarmonyInstance harmony)
		{
			harmony.Patch(
				AccessTools.Method(typeof(FarmAnimal), nameof(FarmAnimal.getSellPrice)),
				prefix: new HarmonyMethod(GetType(), nameof(FarmAnimalGetSellPricePrefix))
			);
		}

		#region harmony patches

		/// <summary>Patch to adjust Breeder animal sell price.</summary>
		private static bool FarmAnimalGetSellPricePrefix(ref FarmAnimal __instance, ref int __result)
		{
			Farmer owner = Game1.getFarmer(__instance.ownerID.Value);
			if (Utility.SpecificPlayerHasProfession("Breeder", owner))
			{
				double adjustedFriendship = Utility.GetProducerAdjustedFriendship(__instance);
				__result = (int)(__instance.price.Value * adjustedFriendship);
				return false; // don't run original logic
			}

			return true; // run original logic
		}

		#endregion harmony patches
	}
}