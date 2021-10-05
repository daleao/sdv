using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Objects;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using TheLion.Stardew.Common.Extensions;
using TheLion.Stardew.Common.Harmony;
using TheLion.Stardew.Professions.Framework.Extensions;

namespace TheLion.Stardew.Professions.Framework.Patches
{
	internal class CrabPotCheckForActionPatch : BasePatch
	{
		/// <summary>Construct an instance.</summary>
		internal CrabPotCheckForActionPatch()
		{
			Original = typeof(CrabPot).MethodNamed(nameof(CrabPot.checkForAction));
			Prefix = new HarmonyMethod(GetType(), nameof(CrabPotCheckForActionPrefix));
		}

		#region harmony patches

		/// <summary>Patch to handle Luremaster-caught non-trap fish.</summary>
		[HarmonyPrefix]
		private static bool CrabPotCheckForActionPrefix(ref CrabPot __instance, ref bool __result,
			ref bool ___lidFlapping, ref float ___lidFlapTimer, ref Vector2 ___shake, ref float ___shakeTimer,
			Farmer who, bool justCheckingForActivity = false)
		{
			try
			{
				if (__instance.tileIndexToShow != 714 || justCheckingForActivity ||
				    !__instance.HasSpecialLuremasterCatch())
					return true; // run original logic

				var item = __instance.heldObject.Value;
				bool addedToInvetory;
				if (__instance.heldObject.Value.ParentSheetIndex.AnyOf(14, 51)) // caught a weapon
				{
					var weapon = new MeleeWeapon(__instance.heldObject.Value.ParentSheetIndex);
					addedToInvetory = who.addItemToInventoryBool(weapon);
					who.mostRecentlyGrabbedItem = item;
				}
				else if (__instance.heldObject.Value.ParentSheetIndex.AnyOf(516, 517, 518, 519, 527, 529, 530, 531, 532,
					533, 534)) // caught a ring
				{
					var ring = new Ring(__instance.heldObject.Value.ParentSheetIndex);
					addedToInvetory = who.addItemToInventoryBool(ring);
					who.mostRecentlyGrabbedItem = item;
				}
				else
				{
					addedToInvetory = who.addItemToInventoryBool(item);
				}

				__instance.heldObject.Value = null;
				if (who.IsLocalPlayer && !addedToInvetory)
				{
					__instance.heldObject.Value = item;
					Game1.showRedMessage(
						Game1.content.LoadString(Path.Combine("Strings", "StringsFromCSFiles:Crop.cs.588")));
					__result = false;
					return false; // don't run original logic;
				}

				var fishData = Game1.content.Load<Dictionary<int, string>>(Path.Combine("Data", "Fish"));
				if (fishData.TryGetValue(item.ParentSheetIndex, out var specificFishData))
				{
					var fields = specificFishData.Split('/');
					var minFishSize = Convert.ToInt32(fields[3]);
					var maxFishSize = Convert.ToInt32(fields[4]);
					who.caughtFish(item.ParentSheetIndex, Game1.random.Next(minFishSize, maxFishSize + 1));
				}

				__instance.readyForHarvest.Value = false;
				__instance.tileIndexToShow = 710;
				___lidFlapping = true;
				___lidFlapTimer = 60f;
				__instance.bait.Value = null;
				who.animateOnce(279 + who.FacingDirection);
				who.currentLocation.playSound("fishingRodBend");
				DelayedAction.playSoundAfterDelay("coin", 500);
				who.gainExperience(1, 5);
				___shake = Vector2.Zero;
				___shakeTimer = 0f;

				__result = true;
				return false; // don't run original logic
			}
			catch (Exception ex)
			{
				ModEntry.Log($"Failed in {MethodBase.GetCurrentMethod().Name}:\n{ex}", LogLevel.Error);
				return true; // default to original logic
			}
		}

		#endregion harmony patches
	}
}