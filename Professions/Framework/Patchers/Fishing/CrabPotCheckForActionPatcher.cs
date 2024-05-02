namespace DaLion.Professions.Framework.Patchers.Fishing;

#region using directives

using System.Reflection;
using DaLion.Shared.Extensions;
using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley.Objects;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class CrabPotCheckForActionPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="CrabPotCheckForActionPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    internal CrabPotCheckForActionPatcher(Harmonizer harmonizer)
        : base(harmonizer)
    {
        this.Target = this.RequireMethod<CrabPot>(nameof(CrabPot.checkForAction));
    }

    #region harmony patches

    /// <summary>Patch to handle Luremaster-caught non-trap fish.</summary>
    [HarmonyPrefix]
    [HarmonyPriority(Priority.High)]
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
            if (__instance.Location is not { } location || __instance.tileIndexToShow != 714 ||
                justCheckingForActivity || !__instance.HasSpecialLuremasterCatch())
            {
                return true; // run original logic
            }

            var held = __instance.heldObject.Value;
            if (held is not null)
            {
                var item = ItemRegistry.Create(held.QualifiedItemId);
                if (item is SObject)
                {
                    item.Stack = held.Stack;
                    item.Quality = held.Quality;
                }

                var addedToInventory = who.addItemToInventoryBool(item);
                if (who.IsLocalPlayer && !addedToInventory)
                {
                    Game1.showRedMessage(
                        Game1.content.LoadString("Strings\\StringsFromCSFiles:Crop.cs.588"));
                    __result = false;
                    return false; // don't run original logic;
                }

                if (addedToInventory && item is MeleeWeapon or Ring)
                {
                    who.mostRecentlyGrabbedItem = item;
                }

                if (Data.Read(__instance, DataKeys.TrappedHaul).ParseList<string>() is { Count: > 0 } haul)
                {
                    __instance.heldObject.Value = parseHeldObjectData(haul[0]);
                    Data.Write(__instance, DataKeys.TrappedHaul, string.Join(',', haul.Skip(1)));
                }
                else
                {
                    __instance.heldObject.Value = null;
                }

                if (DataLoader.Fish(Game1.content).TryGetValue(item.ItemId, out var rawDataStr))
                {
                    var rawData = rawDataStr.Split('/');
                    var minFishSize = rawData.Length <= 5 ? 1 : Convert.ToInt32(value: rawData[5]);
                    var maxFishSize = rawData.Length > 5 ? Convert.ToInt32(rawData[6]) : 10;
                    who.caughtFish(
                        item.QualifiedItemId,
                        Game1.random.Next(minFishSize, maxFishSize + 1),
                        from_fish_pond: false);
                }

                who.gainExperience(1, 5);
            }

            __instance.readyForHarvest.Value = false;
            __instance.tileIndexToShow = 710;
            ___lidFlapping = true;
            ___lidFlapTimer = 60f;
            if (Data.Read(__instance, DataKeys.Overbait).ParseList<string>() is { Count: > 0 } overbait)
            {
                __instance.bait.Value = ItemRegistry.Create<SObject>(overbait[0]);
                Data.Write(__instance, DataKeys.Overbait, string.Join(',', overbait.Skip(1)));
            }
            else
            {
                __instance.bait.Value = null;
            }

            who.animateOnce(279 + who.FacingDirection);
            location.playSound("fishingRodBend");
            DelayedAction.playSoundAfterDelay("coin", 500);
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

        SObject? parseHeldObjectData(string? data)
        {
            if (string.IsNullOrEmpty(data))
            {
                return null;
            }

            var split = data.Split('/');
            return ItemRegistry.Create<SObject>(split[0], amount: int.Parse(split[1]), quality: int.Parse(split[2]));
        }
    }

    #endregion harmony patches

    #region injections

    private static double GetBaitPreservationChance(SObject? caught)
    {
        const int maxValue = 700;
        var cappedValue = caught?.IsTrash() == true
            ? 0d
            : Math.Min(caught?.salePrice() ?? 0, maxValue);

        //     30g -> ~26.5% chance
        //     700g -> ~5.3% <-- capped here
        const double a = 335d / 4d;
        const double b = 275d / 2d;
        return a / (cappedValue + b);
    }

    #endregion injections
}
