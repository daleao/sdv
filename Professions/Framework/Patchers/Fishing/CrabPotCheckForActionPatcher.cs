namespace DaLion.Professions.Framework.Patchers.Fishing;

#region using directives

using System.Reflection;
using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley.Extensions;
using StardewValley.Objects;

#endregion using directives

[UsedImplicitly]
internal sealed class CrabPotCheckForActionPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="CrabPotCheckForActionPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal CrabPotCheckForActionPatcher(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
    {
        this.Target = this.RequireMethod<CrabPot>(nameof(CrabPot.checkForAction));
    }

    #region harmony patches

    /// <summary>Patch to handle Luremaster-caught non-trap fish.</summary>
    [HarmonyPrefix]
    [HarmonyPriority(Priority.First)]
    [UsedImplicitly]
    private static bool CrabPotCheckForActionPrefix(
        ref CrabPot __instance,
        ref bool __result,
        ref int ___ignoreRemovalTimer,
        ref bool ___lidFlapping,
        ref float ___lidFlapTimer,
        ref Vector2 ___shake,
        ref float ___shakeTimer,
        Farmer who,
        bool justCheckingForActivity)
    {
        if (__instance.Location is not { } location || __instance.tileIndexToShow != 714 ||
            justCheckingForActivity || !__instance.HasSpecialLuremasterCatch())
        {
            return true; // run original logic
        }

        try
        {
            var held = __instance.heldObject.Value;
            if (held is not null)
            {
                var item = ItemRegistry.Create(held.QualifiedItemId, held.Stack, held.Quality);
                if (item is SObject && who.stats.Get("Book_Crabbing") != 0)
                {
                    var r = Utility.CreateDaySaveRandom(
                        Game1.uniqueIDForThisGame,
                        Game1.stats.DaysPlayed * 77,
                        (__instance.TileLocation.X * 777f) + __instance.TileLocation.Y);
                    if (r.NextBool(0.25))
                    {
                        item.Stack *= 2;
                    }
                }

                var tileAbove = new Vector2(__instance.TileLocation.X, __instance.TileLocation.Y - 1f);
                if (location.Objects.TryGetValue(tileAbove, out var objAbove) &&
                    objAbove is Chest { SpecialChestType: Chest.SpecialChestTypes.AutoLoader } hopper)
                {
                    // this should always be false
                    if (hopper.GetOwner() != who)
                    {
                        return false; // don't run original logic
                    }

                    if (hopper.addItem(item) != null)
                    {
                        __result = false;
                        return false; // don't run original logic
                    }

                    if (DataLoader.Fish(Game1.content).TryGetValue(item.ItemId, out var rawData1))
                    {
                        var split = rawData1.Split('/');
                        var minFishSize = rawData1.Contains("trap") ? Convert.ToInt32(split[5]) : Convert.ToInt32(split[3]);
                        var maxFishSize = rawData1.Contains("trap") ? Convert.ToInt32(split[6]) : Convert.ToInt32(split[4]) / 2;
                        who.caughtFish(
                            item.QualifiedItemId,
                            Game1.random.Next(minFishSize, maxFishSize + 1),
                            from_fish_pond: false,
                            item.Stack);
                    }

                    who.gainExperience(Skill.Fishing, 5);
                    __instance.bait.Value = null;
                    __instance.heldObject.Value = null;
                    __instance.readyForHarvest.Value = false;
                    __instance.tileIndexToShow = 710;
                    if (hopper.Items.FirstOrDefault(i => i?.Category == SObject.baitCategory) is SObject bait &&
                        __instance.performObjectDropInAction(bait, false, who))
                    {
                        hopper.Items.ReduceId(bait.QualifiedItemId, 1);
                    }

                    if (!location.farmers.Any())
                    {
                        __result = false;
                        return false; // don't run original logic
                    }

                    ___lidFlapping = true;
                    ___lidFlapTimer = 60f;
                    location.playSound("fishingRodBend");
                    DelayedAction.playSoundAfterDelay("coin", 500, location);
                    ___shake = Vector2.Zero;
                    ___shakeTimer = 0f;
                    ___ignoreRemovalTimer = 750;
                    __result = true;
                    return false; // don't run original logic
                }

                var addedToInventory = who.addItemToInventoryBool(item);
                if (who.IsLocalPlayer && !addedToInventory)
                {
                    Game1.showRedMessage(
                        Game1.content.LoadString("Strings\\StringsFromCSFiles:Crop.cs.588"));
                    __result = false;
                    return false; // don't run original logic;
                }

                who.mostRecentlyGrabbedItem = item;
                if (DataLoader.Fish(Game1.content).TryGetValue(item.ItemId, out var rawData2))
                {
                    var split = rawData2.Split('/');
                    var minFishSize = rawData2.Contains("trap") ? Convert.ToInt32(split[5]) : Convert.ToInt32(split[3]);
                    var maxFishSize = rawData2.Contains("trap") ? Convert.ToInt32(split[6]) : Convert.ToInt32(split[4]) / 2;
                    who.caughtFish(
                        item.QualifiedItemId,
                        Game1.random.Next(minFishSize, maxFishSize + 1),
                        from_fish_pond: false,
                        item.Stack);
                }

                who.gainExperience(Skill.Fishing, 5);
            }

            __instance.heldObject.Value = null;
            __instance.bait.Value = null;
            __instance.readyForHarvest.Value = false;
            __instance.tileIndexToShow = 710;
            ___lidFlapping = true;
            ___lidFlapTimer = 60f;
            who.animateOnce(279 + who.FacingDirection);
            location.playSound("fishingRodBend");
            DelayedAction.playSoundAfterDelay("coin", 500);
            ___shake = Vector2.Zero;
            ___shakeTimer = 0f;
            ___ignoreRemovalTimer = 750;
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
