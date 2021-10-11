using System;
using System.Reflection;
using HarmonyLib;
using StardewModdingAPI;
using StardewValley;
using TheLion.Stardew.Common.Harmony;
using TheLion.Stardew.Professions.Framework.Extensions;

namespace TheLion.Stardew.Professions.Framework.Patches
{
	internal class FarmAnimalGetSellPricePatch : BasePatch
	{
		/// <summary>Construct an instance.</summary>
		internal FarmAnimalGetSellPricePatch()
		{
			Original = typeof(FarmAnimal).MethodNamed(nameof(FarmAnimal.getSellPrice));
			Prefix = new HarmonyMethod(GetType(), nameof(FarmAnimalGetSellPricePrefix));
		}

		#region harmony patches

		/// <summary>Patch to adjust Breeder animal sell price.</summary>
		[HarmonyPrefix]
		private static bool FarmAnimalGetSellPricePrefix(FarmAnimal __instance, ref int __result)
		{
			double adjustedFriendship;
			try
			{
				var owner = Game1.getFarmer(__instance.ownerID.Value);
				if (!owner.HasProfession("Breeder")) return true; // run original logic

				adjustedFriendship = Util.Professions.GetProducerAdjustedFriendship(__instance);
			}
			catch (Exception ex)
			{
				ModEntry.Log($"Failed in {MethodBase.GetCurrentMethod().Name}:\n{ex}", LogLevel.Error);
				return true; // default to original logic
			}

			__result = (int) (__instance.price.Value * adjustedFriendship);
			return false; // don't run original logic
		}

		#endregion harmony patches
	}
}