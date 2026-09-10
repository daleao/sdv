namespace DaLion.Professions.Framework.Patchers.Fishing;

#region using directives

using System.Reflection;
using DaLion.Shared.Enums;
using DaLion.Shared.Extensions;
using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Objects;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class CrabPotDayUpdatePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="CrabPotDayUpdatePatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal CrabPotDayUpdatePatcher(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
    {
        this.Target = this.RequireMethod<CrabPot>(nameof(CrabPot.DayUpdate));
    }

    #region harmony patches

    /// <summary>Patch for Trapper fish quality + Luremaster bait mechanics + Conservationist trash collection mechanics.</summary>
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool CrabPotDayUpdatePrefix(CrabPot __instance)
    {
        if (__instance.heldObject.Value is not null)
        {
            return false; // don't run original logic
        }

        var location = __instance.Location;
        var owner = __instance.GetOwner();
        var isConservationist = owner.HasProfessionOrLax(Profession.Conservationist);
        if (__instance.bait.Value is null && !isConservationist)
        {
            return false; // don't run original logic
        }

        try
        {
            var r = Random.Shared;
            var isLuremaster = false;
            var caught = string.Empty;
            if (__instance.bait.Value is { } bait)
            {
                isLuremaster = bait.GetOwner().HasProfessionOrLax(Profession.Luremaster);
                if (isLuremaster)
                {
                    if (__instance.HasMagnet())
                    {
                        caught = __instance.ChoosePirateTreasure(owner, r);
                    }
                    else if (__instance.HasMagicBait())
                    {
                        caught = __instance.ChooseFish(owner, r);
                    }
                }

                if (string.IsNullOrEmpty(caught))
                {
                    caught = __instance.ChooseTrapFish(isLuremaster, owner, r);
                }
            }

            var quantity = 1;
            var quality = 0;
            if (string.IsNullOrEmpty(caught))
            {
                if (owner.HasProfession(Profession.Conservationist, true))
                {
                    caught = __instance.TryFromPondData(owner, r);
                }

                if (string.IsNullOrEmpty(caught))
                {
                    caught = __instance.GetTrash(r);
                    if (isConservationist && caught.IsTrashId())
                    {
                        Data.Increment(owner, DataKeys.ConservationistTrashCollectedThisSeason);
                        if ((int)Data.ReadAs<float>(owner, DataKeys.ConservationistTrashCollectedThisSeason) %
                            Config.ConservationistTrashNeededPerFriendshipPoint ==
                            0)
                        {
                            Utility.improveFriendshipWithEveryoneInRegion(owner, 1, "Town");
                        }
                    }
                }
            }
            else if (caught[1] == 'O') // not ring or weapon
            {
                quantity = __instance.GetTrapQuantity(caught, isLuremaster, owner, r);
                quality = (int)__instance.GetTrapQuality(caught, isLuremaster, owner, r);
            }
            else if (caught[1] == 'R')
            {
                caught = caught.ReplaceAt(1, "O");
            }

            var caughtItem = ItemRegistry.Create(caught, amount: quantity, quality: quality);
            if (caughtItem is not SObject)
            {
                caughtItem = ItemRegistry.Create<SObject>("(O)0", amount: quantity, quality: quality);
                caughtItem.ItemId = caught[3..];
            }

            __instance.heldObject.Value = (SObject)caughtItem;
            __instance.tileIndexToShow = 714;
            __instance.readyForHarvest.Value = true;
            __instance.onReadyForHarvest();
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
