using Harmony;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using SObject = StardewValley.Object;

namespace TheLion.AwesomeProfessions
{
	internal class CrabPotCheckForActionPatch : BasePatch
	{
		/// <summary>Construct an instance.</summary>
		internal CrabPotCheckForActionPatch() { }

		/// <summary>Apply internally-defined Harmony patches.</summary>
		/// <param name="harmony">The Harmony instance for this mod.</param>
		protected internal override void Apply(HarmonyInstance harmony)
		{
			harmony.Patch(
				AccessTools.Method(typeof(CrabPot), nameof(CrabPot.checkForAction)),
				prefix: new HarmonyMethod(GetType(), nameof(CrabPotCheckForActionPrefix))
			);
		}

		#region harmony patches
		/// <summary>Patch to handle Luremaster-caught non-trap fish.</summary>
		protected static bool CrabPotCheckForActionPrefix(ref CrabPot __instance, ref bool __result, ref bool ___lidFlapping, ref float ___lidFlapTimer, ref Vector2 ___shake, ref float ___shakeTimer, Farmer who, bool justCheckingForActivity = false)
		{
			if (__instance.tileIndexToShow != 714 || justCheckingForActivity || !Utility.IsFishButNotTrapFish(__instance.heldObject.Value))
				return true; // run original logic

			SObject item = __instance.heldObject.Value;
			__instance.heldObject.Value = null;
			if (who.IsLocalPlayer && !who.addItemToInventoryBool(item))
			{
				__instance.heldObject.Value = item;
				Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Crop.cs.588"));
				__result = false;
				return false; // don't run original logic;
			}

			Dictionary<int, string> data = Game1.content.Load<Dictionary<int, string>>("Data\\Fish");
			if (data.ContainsKey(item.ParentSheetIndex))
			{
				string[] rawData = data[item.ParentSheetIndex].Split('/');
				int minFishSize = Convert.ToInt32(rawData[3]);
				int maxFishSize = Convert.ToInt32(rawData[4]);
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
		#endregion harmony patches
	}
}
