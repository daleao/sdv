namespace DaLion.Stardew.Professions.Framework.Patches.Fishing;

#region using directives

using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Objects;
using StardewValley.Tools;

using DaLion.Common.Extensions;
using Extensions;

#endregion using directives

[UsedImplicitly]
internal class CrabPotCheckForActionPatch : BasePatch
{
    /// <summary>Construct an instance.</summary>
    internal CrabPotCheckForActionPatch()
    {
        Original = RequireMethod<CrabPot>(nameof(CrabPot.checkForAction));
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
            if (__instance.heldObject.Value.ParentSheetIndex.IsIn(14, 51)) // caught a weapon
            {
                var weapon = new MeleeWeapon(__instance.heldObject.Value.ParentSheetIndex) {specialItem = true};
                addedToInvetory = who.addItemToInventoryBool(weapon);
                who.mostRecentlyGrabbedItem = weapon;
            }
            else if (__instance.heldObject.Value.ParentSheetIndex.IsIn(516, 517, 518, 519, 527, 529, 530, 531, 532,
                         533, 534)) // caught a ring
            {
                var ring = new Ring(__instance.heldObject.Value.ParentSheetIndex);
                addedToInvetory = who.addItemToInventoryBool(ring);
                who.mostRecentlyGrabbedItem = ring;
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
                    Game1.content.LoadString(
                        PathUtilities.NormalizeAssetName("Strings/StringsFromCSFiles:Crop.cs.588")));
                __result = false;
                return false; // don't run original logic;
            }

            var fishData =
                Game1.content.Load<Dictionary<int, string>>(PathUtilities.NormalizeAssetName("Data/Fish"));
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

            if (!who.HasProfession(Profession.Luremaster, true) || Game1.random.NextDouble() > 0.6)
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
            Log.E($"Failed in {MethodBase.GetCurrentMethod()?.Name}:\n{ex}");
            return true; // default to original logic
        }
    }

    #endregion harmony patches
}