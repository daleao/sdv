namespace DaLion.Professions.Framework.Patchers.Fishing;

#region using directives

using System.Reflection;
using DaLion.Shared.Enums;
using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Extensions;
using StardewValley.Objects;

#endregion using directives

[UsedImplicitly]
internal sealed class CrabPotDayUpdatePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="CrabPotDayUpdatePatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    internal CrabPotDayUpdatePatcher(Harmonizer harmonizer)
        : base(harmonizer)
    {
        this.Target = this.RequireMethod<CrabPot>(nameof(CrabPot.DayUpdate));
    }

    #region harmony patches

    /// <summary>Patch for Trapper fish quality + Luremaster bait mechanics + Conservationist trash collection mechanics.</summary>
    [HarmonyPrefix]
    private static bool CrabPotDayUpdatePrefix(CrabPot __instance)
    {
        try
        {
            var location = __instance.Location;
            var owner = __instance.GetOwner();
            var isConservationist = owner.HasProfessionOrLax(Profession.Conservationist);
            if (__instance.bait.Value is null && !isConservationist)
            {
                return false; // don't run original logic
            }

            var (tileX, tileY) = __instance.TileLocation;
            var (offsetX, offsetY) = __instance.directionOffset.Value;
            var r = Utility.CreateDaySaveRandom(tileX * 1000f, tileY * 255f, (offsetX * 1000f) + offsetY);
            var isLuremaster = false;
            var caught = string.Empty;
            if (__instance.bait.Value is { } bait)
            {
                isLuremaster = bait.GetOwner().HasProfessionOrLax(Profession.Luremaster);
                if (isLuremaster)
                {
                    if (__instance.heldObject.Value is { } held)
                    {
                        Data.Append(
                            __instance,
                            DataKeys.TrappedHaul,
                            $"{held.QualifiedItemId}/{held.Stack}/{held.Quality}");
                        __instance.heldObject.Value = null;
                    }

                    if (__instance.HasMagnet())
                    {
                        caught = __instance.ChoosePirateTreasure(owner, r);
                    }
                    else if (__instance.HasMagicBait())
                    {
                        caught = __instance.ChooseFish(owner, r);
                    }
                }
                else
                {
                    return false; // don't run original logic
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
                if (__instance.bait.Value is not null || isConservationist)
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
                else
                {
                    return false; // don't run original logic
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
                    quantity += 1;
                    if (quality is 3 or > 4)
                    {
                        quality = 4;
                    }
                }
            }
            else if (caught[1] == 'R')
            {
                caught = caught.Replace('R', 'O');
            }

            __instance.heldObject.Value = ItemRegistry.Create<SObject>(caught, amount: quantity, quality: quality);
            __instance.tileIndexToShow = 714;
            __instance.readyForHarvest.Value = true;
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
