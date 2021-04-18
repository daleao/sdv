using Harmony;
using StardewValley;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.IO;
using TheLion.Common;
using SObject = StardewValley.Object;

namespace TheLion.AwesomeProfessions
{
	internal class CrabPotDayUpdatePatch : BasePatch
	{
		/// <inheritdoc/>
		public override void Apply(HarmonyInstance harmony)
		{
			harmony.Patch(
				original: AccessTools.Method(typeof(CrabPot), nameof(CrabPot.DayUpdate)),
				prefix: new HarmonyMethod(GetType(), nameof(CrabPotDayUpdatePrefix))
			);
		}

		#region harmony patches

		/// <summary>Patch for Trapper fish quality + Luremaster bait mechanics + Conservationist trash collection mechanics.</summary>
		private static bool CrabPotDayUpdatePrefix(ref CrabPot __instance, GameLocation location)
		{
			try
			{
				var who = Game1.getFarmer(__instance.owner.Value);
				if (__instance.bait.Value == null && !Utility.SpecificPlayerHasProfession("Conservationist", who) || __instance.heldObject.Value != null)
					return false; // don't run original logic

				__instance.tileIndexToShow = 714;
				__instance.readyForHarvest.Value = true;

				var r = new Random((int)Game1.stats.DaysPlayed + (int)Game1.uniqueIDForThisGame / 2 + (int)__instance.TileLocation.X * 1000 + (int)__instance.TileLocation.Y);
				var locationData = Game1.content.Load<Dictionary<string, string>>(Path.Combine("Data", "Locations"));
				var fishData = Game1.content.Load<Dictionary<int, string>>(Path.Combine("Data", "Fish"));
				var whichFish = -1;
				if (__instance.bait.Value != null)
				{
					if (Utility.SpecificPlayerHasProfession("Luremaster", who))
					{
						if (Utility.IsUsingMagnet(__instance))
						{
							whichFish = 14;
							//whichFish = Utility.ChoosePirateTreasure(r, who);
						}
						else if (Game1.random.NextDouble() < (Utility.IsUsingMagicBait(__instance) ? 0.25 : 0.1))
						{
							var rawFishData = Utility.IsUsingMagicBait(__instance) ? Utility.GetRawFishDataForAllSeasons(location, locationData) : Utility.GetRawFishDataForThisSeason(location, locationData);
							var rawFishDataWithLocation = Utility.GetRawFishDataWithLocation(rawFishData);
							whichFish = Utility.ChooseFish(__instance, fishData, rawFishDataWithLocation, location, r);
							if (whichFish < 0) whichFish = Utility.ChooseTrapFish(__instance, fishData, location, r, isLuremaster: true);
						}
						else
						{
							whichFish = Utility.ChooseTrapFish(__instance, fishData, location, r, isLuremaster: true);
						}
					}
					else
					{
						whichFish = Utility.ChooseTrapFish(__instance, fishData, location, r, isLuremaster: false);
					}
				}

				if (whichFish.AnyOf(14, 51, 516, 517, 518, 519, 527, 529, 530, 531, 532, 533, 534))
				{
					var equipment = new SObject(whichFish, 1);
					__instance.heldObject.Value = equipment;
					return false; // don't run original logic
				}

				var fishQuality = 0;
				if (whichFish < 0)
				{
					if (__instance.bait.Value != null || Utility.SpecificPlayerHasProfession("Conservationist", who)) whichFish = Utility.GetTrash(r);
				}
				else
				{
					fishQuality = Utility.GetTrapFishQuality(whichFish, who, r, __instance, Utility.SpecificPlayerHasProfession("Luremaster", who));
				}

				var fishQuantity = Utility.GetTrapFishQuantity(__instance, whichFish, who, r);
				__instance.heldObject.Value = new SObject(whichFish, initialStack: fishQuantity, quality: fishQuality);
				return false; // don't run original logic
			}
			catch (Exception ex)
			{
				Monitor.Log($"Failed in {nameof(CrabPotDayUpdatePrefix)}:\n{ex}");
				return true; // default to original logic
			}
		}

		#endregion harmony patches
	}
}