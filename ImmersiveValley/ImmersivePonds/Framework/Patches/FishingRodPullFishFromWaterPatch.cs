namespace DaLion.Stardew.Ponds.Framework.Patches;

#region using directives

using System;
using System.IO;
using System.Linq;
using System.Reflection;
using CommunityToolkit.Diagnostics;
using DaLion.Common;
using DaLion.Common.Extensions;
using DaLion.Common.Extensions.Reflection;
using DaLion.Common.Extensions.Stardew;
using DaLion.Stardew.Ponds.Extensions;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley.Buildings;
using StardewValley.Tools;
using HarmonyPatch = DaLion.Common.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
internal sealed class FishingRodPullFishFromWaterPatch : HarmonyPatch
{
    private static readonly Lazy<Func<FishingRod, Vector2>> CalculateBobberTile = new(() =>
        typeof(FishingRod)
            .RequireMethod("calculateBobberTile")
            .CompileUnboundDelegate<Func<FishingRod, Vector2>>());

    /// <summary>Initializes a new instance of the <see cref="FishingRodPullFishFromWaterPatch"/> class.</summary>
    internal FishingRodPullFishFromWaterPatch()
    {
        this.Target = this.RequireMethod<FishingRod>(nameof(FishingRod.pullFishFromWater));
    }

    #region harmony patches

    /// <summary>Decrement total Fish Pond quality ratings.</summary>
    [HarmonyPrefix]
    private static void FishingRodPullFishFromWaterPrefix(
        FishingRod __instance, ref int whichFish, ref int fishQuality, bool fromFishPond)
    {
        if (!fromFishPond || whichFish.IsTrashIndex())
        {
            return;
        }

        var (x, y) = CalculateBobberTile.Value(__instance);
        var pond = Game1.getFarm().buildings.OfType<FishPond>().FirstOrDefault(p =>
            x > p.tileX.Value && x < p.tileX.Value + p.tilesWide.Value - 1 &&
            y > p.tileY.Value && y < p.tileY.Value + p.tilesHigh.Value - 1);
        if (pond is null || pond.FishCount < 0)
        {
            return;
        }

        try
        {
            if (pond.HasAlgae())
            {
                fishQuality = SObject.lowQuality;

                var seaweedCount = pond.Read<int>("SeaweedLivingHere");
                var greenAlgaeCount = pond.Read<int>("GreenAlgaeLivingHere");
                var whiteAlgaeCount = pond.Read<int>("WhiteAlgaeLivingHere");

                var roll = Game1.random.Next(seaweedCount + greenAlgaeCount + whiteAlgaeCount);
                if (roll < seaweedCount)
                {
                    whichFish = Constants.SeaweedIndex;
                    pond.Write("SeaweedLivingHere", (--seaweedCount).ToString());
                }
                else if (roll < seaweedCount + greenAlgaeCount)
                {
                    whichFish = Constants.GreenAlgaeIndex;
                    pond.Write("GreenAlgaeLivingHere", (--greenAlgaeCount).ToString());
                }
                else if (roll < seaweedCount + greenAlgaeCount + whiteAlgaeCount)
                {
                    whichFish = Constants.WhiteAlgaeIndex;
                    pond.Write("WhiteAlgaeLivingHere", (--whiteAlgaeCount).ToString());
                }

                var total = __instance.Read<int>("SeaweedLivingHere") +
                            __instance.Read<int>("GreenAlgaeLivingHere") +
                            __instance.Read<int>("WhiteAlgaeLivingHere");
                if (total != pond.FishCount)
                {
                    ThrowHelper.ThrowInvalidDataException(
                        "Mismatch between algae population data and actual population.");
                }

                return;
            }
        }
        catch (InvalidDataException ex)
        {
            Log.W($"{ex}\nThe data will be reset.");
            pond.Write("SeaweedLivingHere", null);
            pond.Write("GreenAlgaeLivingHere", null);
            pond.Write("WhiteAlgaeLivingHere", null);
            var field = pond.fishType.Value switch
            {
                Constants.SeaweedIndex => "SeaweedLivingHere",
                Constants.GreenAlgaeIndex => "GreenAlgaeLivingHere",
                Constants.WhiteAlgaeIndex => "WhiteAlgaeLivingHere",
                _ => string.Empty,
            };

            pond.Write(field, pond.FishCount.ToString());
        }
        catch (Exception ex)
        {
            Log.E($"Failed in {MethodBase.GetCurrentMethod()?.Name}:\n{ex}");
        }

        try
        {
            var fishQualities = pond.Read("FishQualities", $"{pond.FishCount - pond.Read<int>("FamilyLivingHere")},0,0,0").ParseList<int>();
            if (fishQualities.Count != 4 || fishQualities.Any(q => q < 0 || q > pond.FishCount + 1)) // FishCount has already been decremented at this point, so we increment 1 to compensate
            {
                ThrowHelper.ThrowInvalidDataException("FishQualities data had incorrect number of values.");
            }

            var lowestFish = fishQualities.FindIndex(i => i > 0);
            if (pond.HasLegendaryFish())
            {
                var familyCount = pond.Read<int>("FamilyLivingHere");
                if (fishQualities.Sum() + familyCount != pond.FishCount + 1) // FishCount has already been decremented at this point, so we increment 1 to compensate
                {
                    ThrowHelper.ThrowInvalidDataException("FamilyLivingHere data is invalid.");
                }

                if (familyCount > 0)
                {
                    var familyQualities =
                        pond.Read("FamilyQualities", $"{pond.Read<int>("FamilyLivingHere")},0,0,0")
                            .ParseList<int>();
                    if (familyQualities.Count != 4 || familyQualities.Sum() != familyCount)
                    {
                        ThrowHelper.ThrowInvalidDataException("FamilyQualities data had incorrect number of values.");
                    }

                    var lowestFamily = familyQualities.FindIndex(i => i > 0);
                    if (lowestFamily < lowestFish || (lowestFamily == lowestFish && Game1.random.NextDouble() < 0.5))
                    {
                        whichFish = Utils.ExtendedFamilyPairs[whichFish];
                        fishQuality = lowestFamily == 3 ? 4 : lowestFamily;
                        --familyQualities[lowestFamily];
                        pond.Write("FamilyQualities", string.Join(",", familyQualities));
                        pond.Increment("FamilyLivingHere", -1);
                    }
                    else
                    {
                        fishQuality = lowestFish == 3 ? 4 : lowestFish;
                        --fishQualities[lowestFish];
                        pond.Write("FishQualities", string.Join(",", fishQualities));
                    }
                }
                else
                {
                    fishQuality = lowestFish == 3 ? 4 : lowestFish;
                    --fishQualities[lowestFish];
                    pond.Write("FishQualities", string.Join(",", fishQualities));
                }
            }
            else
            {
                fishQuality = lowestFish == 3 ? 4 : lowestFish;
                --fishQualities[lowestFish];
                pond.Write("FishQualities", string.Join(",", fishQualities));
            }
        }
        catch (InvalidDataException ex)
        {
            Log.W($"{ex}\nThe data will be reset.");
            pond.Write("FishQualities", $"{pond.FishCount},0,0,0");
            pond.Write("FamilyQualities", null);
            pond.Write("FamilyLivingHere", null);
        }
        catch (Exception ex)
        {
            Log.E($"Failed in {MethodBase.GetCurrentMethod()?.Name}:\n{ex}");
        }
    }

    #endregion harmony patches
}
