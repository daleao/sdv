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
				AccessTools.Method(typeof(CrabPot), nameof(CrabPot.DayUpdate)),
				prefix: new HarmonyMethod(GetType(), nameof(CrabPotDayUpdatePrefix))
			);
		}

		#region harmony patches

		/// <summary>Patch for Trapper fish quality + Luremaster bait mechanics + Conservationist trash collection mechanics.</summary>
		private static bool CrabPotDayUpdatePrefix(ref CrabPot __instance, GameLocation location)
		{
			Farmer who = Game1.getFarmer(__instance.owner.Value);
			if (__instance.bait.Value == null && !Utility.SpecificFarmerHasProfession("conservationist", who) || __instance.heldObject.Value != null)
				return false; // don't run original logic

			__instance.tileIndexToShow = 714;
			__instance.readyForHarvest.Value = true;

			Random r = new Random((int)Game1.stats.DaysPlayed + (int)Game1.uniqueIDForThisGame / 2 + (int)__instance.TileLocation.X * 1000 + (int)__instance.TileLocation.Y);
			Dictionary<string, string> locationData = Game1.content.Load<Dictionary<string, string>>(Path.Combine("Data", "Locations"));
			Dictionary<int, string> fishData = Game1.content.Load<Dictionary<int, string>>(Path.Combine("Data", "Fish"));
			int whichFish = -1;
			if (__instance.bait.Value != null)
			{
				if (Utility.SpecificFarmerHasProfession("luremaster", who))
				{
					if (!Utility.IsUsingMagnet(__instance))
					{
						var rawFishData = Utility.IsUsingMagicBait(__instance) ? Utility.GetRawFishDataForAllSeasons(location, locationData) : Utility.GetRawFishDataForThisSeason(location, locationData);
						var rawFishDataWithLocation = Utility.GetRawFishDataWithLocation(rawFishData);
						whichFish = Utility.ChooseFish(__instance, fishData, rawFishDataWithLocation, location, r);
						if (whichFish < 0) whichFish = Utility.ChooseTrapFish(__instance, fishData, location, r, isLuremaster: true);
					}
					else
					{
						whichFish = Utility.ChoosePirateTreasure(r, who);
					}
				}
				else
				{
					whichFish = Utility.ChooseTrapFish(__instance, fishData, location, r, isLuremaster: false);
				}
			}

			if (whichFish.AnyOf(14, 51, 516, 517, 518, 519, 527, 529, 530, 531, 532, 533, 534))
			{
				SObject equipment = new SObject(whichFish, 1);
				__instance.heldObject.Value = equipment;
				return false; // don't run original logic
			}

			int fishQuality = 0;
			if (whichFish < 0)
			{
				bool playerIsConservationist = Utility.SpecificFarmerHasProfession("conservationist", who);
				if (__instance.bait.Value != null || playerIsConservationist) whichFish = Utility.GetTrash(r);
			}
			else fishQuality = Utility.GetTrapFishQuality(whichFish, who, r);

			int fishQuantity = Utility.GetTrapFishQuantity(__instance, whichFish, who, r);
			__instance.heldObject.Value = new SObject(whichFish, initialStack: fishQuantity, quality: fishQuality);
			return false; // don't run original logic
		}

		#endregion harmony patches
	}
}