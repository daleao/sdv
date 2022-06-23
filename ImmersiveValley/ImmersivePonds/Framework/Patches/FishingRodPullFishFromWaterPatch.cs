namespace DaLion.Stardew.Ponds.Framework.Patches;

#region using directives

using System;
using System.IO;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Tools;

using Common;
using Common.Harmony;
using Common.Extensions;
using Common.Extensions.Reflection;
using Extensions;

using SObject = StardewValley.Object;

#endregion using directives

[UsedImplicitly]
internal sealed class FishingRodPullFishFromWaterPatch : BasePatch
{
    private static readonly MethodInfo _CalculateBobberTile = typeof(FishingRod).RequireMethod("calculateBobberTile");

    /// <summary>Construct an instance.</summary>
    internal FishingRodPullFishFromWaterPatch()
    {
        Target = RequireMethod<FishingRod>(nameof(FishingRod.pullFishFromWater));
    }

    #region harmony patches

    /// <summary>Decrement total Fish Pond quality ratings.</summary>
    [HarmonyPrefix]
    private static void FishingRodPullFishFromWaterPrefix(FishingRod __instance, ref int whichFish, ref int fishQuality, bool fromFishPond)
    {
        if (!fromFishPond || whichFish.IsTrash()) return;

        var (x, y) = (Vector2) _CalculateBobberTile.Invoke(__instance, null)!;
        var pond = Game1.getFarm().buildings.OfType<FishPond>().FirstOrDefault(p =>
            x > p.tileX.Value && x < p.tileX.Value + p.tilesWide.Value - 1 &&
            y > p.tileY.Value && y < p.tileY.Value + p.tilesHigh.Value - 1);
        if (pond is null || pond.FishCount < 1) return;

        if (pond.IsAlgaePond())
        {
            fishQuality = SObject.lowQuality;

            var seaweedCount = pond.ReadDataAs<int>("SeaweedLivingHere");
            var greenAlgaeCount = pond.ReadDataAs<int>("GreenAlgaeLivingHere");
            var whiteAlgaeCount = pond.ReadDataAs<int>("WhiteAlgaeLivingHere");

            var roll = Game1.random.Next(seaweedCount + greenAlgaeCount + whiteAlgaeCount);
            if (roll < seaweedCount)
            {
                whichFish = Constants.SEAWEED_INDEX_I;
                pond.WriteData("SeaweedLivingHere", (--seaweedCount).ToString());
            }
            else if (roll < seaweedCount + greenAlgaeCount)
            {
                whichFish = Constants.GREEN_ALGAE_INDEX_I;
                pond.WriteData("GreenAlgaeLivingHere", (--greenAlgaeCount).ToString());
            }
            else if (roll < seaweedCount + greenAlgaeCount + whiteAlgaeCount)
            {
                whichFish = Constants.WHITE_ALGAE_INDEX_I;
                pond.WriteData("WhiteAlgaeLivingHere", (--whiteAlgaeCount).ToString());
            }

            return;
        }

        try
        {
            var fishQualities = pond.ReadData("FishQualities",
                $"{pond.FishCount - pond.ReadDataAs<int>("FamilyLivingHere")},0,0,0").ParseList<int>()!;
            if (fishQualities.Count != 4 || fishQualities.Any(q => 0 > q || q > pond.FishCount + 1)) // FishCount has already been decremented at this point, so we increment 1 to compensate
                throw new InvalidDataException("FishQualities data had incorrect number of values.");

            var lowestFish = fishQualities.FindIndex(i => i > 0);
            if (pond.IsLegendaryPond())
            {
                var familyCount = pond.ReadDataAs<int>("FamilyLivingHere");
                if (fishQualities.Sum() + familyCount != pond.FishCount + 1) // FishCount has already been decremented at this point, so we increment 1 to compensate
                    throw new InvalidDataException("FamilyLivingHere data is invalid.");

                if (familyCount > 0)
                {
                    var familyQualities =
                        pond.ReadData("FamilyQualities", $"{pond.ReadDataAs<int>("FamilyLivingHere")},0,0,0")
                            .ParseList<int>()!;
                    if (familyQualities.Count != 4 || familyQualities.Sum() != familyCount)
                        throw new InvalidDataException("FamilyQualities data had incorrect number of values.");

                    var lowestFamily = familyQualities.FindIndex(i => i > 0);
                    if (lowestFamily < lowestFish || lowestFamily == lowestFish && Game1.random.NextDouble() < 0.5)
                    {
                        whichFish = Ponds.Framework.Utils.ExtendedFamilyPairs[whichFish];
                        fishQuality = lowestFamily == 3 ? 4 : lowestFamily;
                        --familyQualities[lowestFamily];
                        pond.WriteData("FamilyQualities", string.Join(",", familyQualities));
                        pond.IncrementData("FamilyLivingHere", -1);
                    }
                    else
                    {
                        fishQuality = lowestFish == 3 ? 4 : lowestFish;
                        --fishQualities[lowestFish];
                        pond.WriteData("FishQualities", string.Join(",", fishQualities));
                    }
                }
                else
                {
                    fishQuality = lowestFish == 3 ? 4 : lowestFish;
                    --fishQualities[lowestFish];
                    pond.WriteData("FishQualities", string.Join(",", fishQualities));
                }
            }
            else
            {
                fishQuality = lowestFish == 3 ? 4 : lowestFish;
                --fishQualities[lowestFish];
                pond.WriteData("FishQualities", string.Join(",", fishQualities));
            }
        }
        catch (InvalidDataException ex)
        {
            Log.W($"{ex}\nThe data will be reset.");
            pond.WriteData("FishQualities", $"{pond.FishCount},0,0,0");
            pond.WriteData("FamilyQualities", null);
            pond.WriteData("FamilyLivingHere", null);
        }
        catch (Exception ex)
        {
            Log.E($"Failed in {MethodBase.GetCurrentMethod()?.Name}:\n{ex}");
        }
    }

    #endregion harmony patches
}