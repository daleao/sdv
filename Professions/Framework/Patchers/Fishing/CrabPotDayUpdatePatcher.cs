namespace DaLion.Professions.Framework.Patchers.Fishing;

#region using directives

using System.Reflection;
using DaLion.Shared.Enums;
using DaLion.Shared.Extensions;
using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Objects;

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
            var r = new Random(Guid.NewGuid().GetHashCode());
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
                    var isSpecialOceanographerCondition =
                        Game1.IsRainingHere(location) || Game1.IsLightningHere(location) ||
                        Game1.dayOfMonth == 15;
                    if (isSpecialOceanographerCondition || r.NextBool(0.1))
                    {
                        caught = __instance.ChooseTrapFish(false, owner, r);
                    }

                    if (!string.IsNullOrEmpty(caught) && isSpecialOceanographerCondition)
                    {
                        quantity = __instance.GetTrapQuantity(caught, isLuremaster, isSpecialOceanographerCondition, owner, r);
                        quality = (int)__instance.GetTrapQuality(caught, isLuremaster, owner, r).Increment();
                    }
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
            else if (caught[1] is not ('R' or 'W')) // not ring or weapon
            {
                var isSpecialOceanographerCondition = owner.HasProfession(Profession.Conservationist, true) &&
                    (Game1.IsRainingHere(location) || Game1.IsLightningHere(location) ||
                    Game1.dayOfMonth == 15);
                quantity = __instance.GetTrapQuantity(caught, isLuremaster, isSpecialOceanographerCondition, owner, r);
                quality = (int)__instance.GetTrapQuality(caught, isLuremaster, owner, r);
                if (isSpecialOceanographerCondition)
                {
                    quality += 1;
                    if (quality is 3 or > 4)
                    {
                        quality = 4;
                    }
                }
            }
            else
            {
                caught = caught.ReplaceAt(1, "O");
            }

            __instance.heldObject.Value = ItemRegistry.Create<SObject>(caught, amount: quantity, quality: quality);
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
