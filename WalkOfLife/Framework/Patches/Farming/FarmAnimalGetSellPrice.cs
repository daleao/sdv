﻿using Harmony;
using StardewValley;
using StardewModdingAPI;
using System;

namespace TheLion.AwesomeProfessions.Framework.Patches
{
	internal class FarmAnimalGetSellPricePatch : BasePatch
	{
		/// <summary>Construct an instance.</summary>
		/// <param name="config">The mod settings.</param>
		/// <param name="monitor">Interface for writing to the SMAPI console.</param>
		internal FarmAnimalGetSellPricePatch(ProfessionsConfig config, IMonitor monitor)
		: base(config, monitor) { }

		/// <summary>Apply internally-defined Harmony patches.</summary>
		/// <param name="harmony">The Harmony instance for this mod.</param>
		protected internal override void Apply(HarmonyInstance harmony)
		{
			harmony.Patch(
				AccessTools.Method(typeof(FarmAnimal), nameof(FarmAnimal.getSellPrice)),
				prefix: new HarmonyMethod(GetType(), nameof(FarmAnimalGetSellPricePrefix))
			);
		}

		/// <summary>Patch to adjust Breeder animal sell price.</summary>
		protected static bool FarmAnimalGetSellPricePrefix(ref FarmAnimal __instance, ref int __result)
		{
			Farmer who = Game1.getFarmer(__instance.ownerID.Value);
			if (Utils.SpecificPlayerHasProfession("breeder", who))
			{
				double adjustedFriendship = Math.Pow(Math.Sqrt(2) * __instance.friendshipTowardFarmer.Value / 1000, 2) + 0.5;
				__result = (int)(__instance.price.Value * adjustedFriendship);
				return false; // don't run original logic
			}

			return true; // run original logic
		}
	}
}