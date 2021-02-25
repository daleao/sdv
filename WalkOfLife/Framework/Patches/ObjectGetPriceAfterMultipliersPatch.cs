using Harmony;
using StardewModdingAPI;
using StardewValley;
using System;
using SObject = StardewValley.Object;

using static TheLion.AwesomeProfessions.Framework.Utils;

namespace TheLion.AwesomeProfessions.Framework.Patches
{
	internal class ObjectGetPriceAfterMultipliersPatch : BasePatch
	{
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
				if (PlayerHasProfession("producer", player) && IsAnimalProduct(__instance))
				{
					// implement
				}
				if (PlayerHasProfession("oenologist", player) && IsWineOrBeverage(__instance))
				{
					// implement
				}
				if (PlayerHasProfession("angler", player) && IsReeledFish(__instance))
				{
					// implement
				}
				if (PlayerHasProfession("conservationist", player))
				{
					// implement
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
	}
}
