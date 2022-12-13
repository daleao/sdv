namespace DaLion.Overhaul.Modules.Professions.Patchers.Fishing;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using DaLion.Overhaul.Modules.Professions.Extensions;
using DaLion.Shared.Extensions;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Utilities;
using StardewValley.Objects;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class CrabPotCheckForActionPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="CrabPotCheckForActionPatcher"/> class.</summary>
    internal CrabPotCheckForActionPatcher()
    {
        this.Target = this.RequireMethod<CrabPot>(nameof(CrabPot.checkForAction));
    }

    #region harmony patches

    /// <summary>Patch to handle Luremaster-caught non-trap fish.</summary>
    [HarmonyPrefix]
    private static bool CrabPotCheckForActionPrefix(
        ref CrabPot __instance,
        ref bool __result,
        ref bool ___lidFlapping,
        ref float ___lidFlapTimer,
        ref Vector2 ___shake,
        ref float ___shakeTimer,
        Farmer who,
        bool justCheckingForActivity = false)
    {
        try
        {
            if (__instance.tileIndexToShow != 714 || justCheckingForActivity ||
                !__instance.HasSpecialLuremasterCatch())
            {
                return true; // run original logic
            }

            var item = __instance.heldObject.Value;
            bool addedToInventory;
            if (__instance.heldObject.Value.ParentSheetIndex.IsIn(14, 51))
            {
                // caught a weapon
                var weapon = new MeleeWeapon(__instance.heldObject.Value.ParentSheetIndex) { specialItem = true };
                addedToInventory = who.addItemToInventoryBool(weapon);
                who.mostRecentlyGrabbedItem = weapon;
            }
            else if (__instance.heldObject.Value.ParentSheetIndex
                     .IsIn(516, 517, 518, 519, 527, 529, 530, 531, 532, 533, 534))
            {
                // caught a ring
                var ring = new Ring(__instance.heldObject.Value.ParentSheetIndex);
                addedToInventory = who.addItemToInventoryBool(ring);
                who.mostRecentlyGrabbedItem = ring;
            }
            else
            {
                addedToInventory = who.addItemToInventoryBool(item);
            }

            __instance.heldObject.Value = null;
            if (who.IsLocalPlayer && !addedToInventory)
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
            {
                __instance.bait.Value = null;
            }

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
