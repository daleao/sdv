namespace DaLion.Overhaul.Modules.Professions.Patchers.Fishing;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using DaLion.Overhaul.Modules.Professions.Extensions;
using DaLion.Shared.Extensions;
using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewModdingAPI.Utilities;
using StardewValley.Objects;

#endregion using directives

[UsedImplicitly]
internal sealed class CrabPotDayUpdatePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="CrabPotDayUpdatePatcher"/> class.</summary>
    internal CrabPotDayUpdatePatcher()
    {
        this.Target = this.RequireMethod<CrabPot>(nameof(CrabPot.DayUpdate));
    }

    #region harmony patches

    /// <summary>Patch for Trapper fish quality + Luremaster bait mechanics + Conservationist trash collection mechanics.</summary>
    [HarmonyPrefix]
    private static bool CrabPotDayUpdatePrefix(CrabPot __instance, GameLocation location)
    {
        try
        {
            var owner = __instance.GetOwner();
            var isConservationist = ProfessionsModule.Config.LaxOwnershipRequirements
                ? Game1.game1.DoesAnyPlayerHaveProfession(Profession.Conservationist, out _)
                : owner.HasProfession(Profession.Conservationist);
            if ((__instance.bait.Value is null && !isConservationist) || __instance.heldObject.Value is not null)
            {
                return false; // don't run original logic
            }

            var r = new Random(Guid.NewGuid().GetHashCode());
            var fishData =
                Game1.content.Load<Dictionary<int, string>>(PathUtilities.NormalizeAssetName("Data/Fish"));
            var isLuremaster = ProfessionsModule.Config.LaxOwnershipRequirements
                ? Game1.game1.DoesAnyPlayerHaveProfession(Profession.Luremaster, out _)
                : owner.HasProfession(Profession.Luremaster);
            var whichFish = -1;
            if (__instance.bait.Value is not null)
            {
                if (isLuremaster)
                {
                    if (__instance.HasMagnet())
                    {
                        whichFish = __instance.ChoosePirateTreasure(owner, r);
                    }
                    else if (Game1.random.NextDouble() < 0.25)
                    {
                        whichFish = __instance.ChooseFish(fishData, location, r);
                        if (whichFish < 0)
                        {
                            whichFish = __instance.ChooseTrapFish(fishData, location, r, true);
                        }
                    }
                    else
                    {
                        whichFish = __instance.ChooseTrapFish(fishData, location, r, true);
                    }
                }
                else
                {
                    whichFish = __instance.ChooseTrapFish(fishData, location, r, false);
                }
            }

            var fishQuality = 0;
            var fishQuantity = 1;
            if (whichFish < 0)
            {
                if (__instance.bait.Value is not null || isConservationist)
                {
                    whichFish = __instance.GetTrash(location, r);
                    if (isConservationist && whichFish.IsTrashIndex())
                    {
                        var isPrestiged = owner.HasProfession(Profession.Conservationist, true);
                        owner.Increment(DataKeys.ConservationistTrashCollectedThisSeason, isPrestiged ? 1.5f : 1f);
                        if ((int)owner.Read<float>(DataKeys.ConservationistTrashCollectedThisSeason) %
                            ProfessionsModule.Config.TrashNeededPerFriendshipPoint ==
                            0)
                        {
                            Utility.improveFriendshipWithEveryoneInRegion(owner, 1, 2);
                        }
                    }
                }
                else
                {
                    return false; // don't run original logic
                }
            }
            else if (!whichFish
                         .IsIn(14, 51, 516, 517, 518, 519, 527, 529, 530, 531, 532, 533, 534))
            {
                // not ring or weapon
                fishQuality = __instance.GetTrapQuality(whichFish, owner, r, isLuremaster);
                fishQuantity = __instance.GetTrapQuantity(whichFish, owner, r);
            }

            __instance.heldObject.Value = new SObject(whichFish, fishQuantity, quality: fishQuality);
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
