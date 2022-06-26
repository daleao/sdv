namespace DaLion.Stardew.Ponds.Framework.Patches;

#region using directives

using Common;
using Common.Data;
using Common.Extensions;
using Common.Extensions.Reflection;
using Extensions;
using HarmonyLib;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Tools;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using SObject = StardewValley.Object;

#endregion using directives

[UsedImplicitly]
internal sealed class FishingRodPullFishFromWaterPatch : Common.Harmony.HarmonyPatch
{
    private static Func<FishingRod, Vector2>? _CalculateBobberTile;

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

        _CalculateBobberTile ??= typeof(FishingRod).RequireMethod("calculateBobberTile")
            .CompileUnboundDelegate<Func<FishingRod, Vector2>>();
        var (x, y) = _CalculateBobberTile.Invoke(__instance);
        var pond = Game1.getFarm().buildings.OfType<FishPond>().FirstOrDefault(p =>
            x > p.tileX.Value && x < p.tileX.Value + p.tilesWide.Value - 1 &&
            y > p.tileY.Value && y < p.tileY.Value + p.tilesHigh.Value - 1);
        if (pond is null || pond.FishCount < 0) return;

        if (pond.IsAlgaePond())
        {
            fishQuality = SObject.lowQuality;

            var seaweedCount = ModDataIO.ReadDataAs<int>(pond, "SeaweedLivingHere");
            var greenAlgaeCount = ModDataIO.ReadDataAs<int>(pond, "GreenAlgaeLivingHere");
            var whiteAlgaeCount = ModDataIO.ReadDataAs<int>(pond, "WhiteAlgaeLivingHere");

            var roll = Game1.random.Next(seaweedCount + greenAlgaeCount + whiteAlgaeCount);
            if (roll < seaweedCount)
            {
                whichFish = Constants.SEAWEED_INDEX_I;
                ModDataIO.WriteData(pond, "SeaweedLivingHere", (--seaweedCount).ToString());
            }
            else if (roll < seaweedCount + greenAlgaeCount)
            {
                whichFish = Constants.GREEN_ALGAE_INDEX_I;
                ModDataIO.WriteData(pond, "GreenAlgaeLivingHere", (--greenAlgaeCount).ToString());
            }
            else if (roll < seaweedCount + greenAlgaeCount + whiteAlgaeCount)
            {
                whichFish = Constants.WHITE_ALGAE_INDEX_I;
                ModDataIO.WriteData(pond, "WhiteAlgaeLivingHere", (--whiteAlgaeCount).ToString());
            }

            return;
        }

        try
        {
            var fishQualities = ModDataIO.ReadData(pond, "FishQualities",
                $"{pond.FishCount - ModDataIO.ReadDataAs<int>(pond, "FamilyLivingHere")},0,0,0").ParseList<int>()!;
            if (fishQualities.Count != 4 || fishQualities.Any(q => 0 > q || q > pond.FishCount + 1)) // FishCount has already been decremented at this point, so we increment 1 to compensate
                throw new InvalidDataException("FishQualities data had incorrect number of values.");

            var lowestFish = fishQualities.FindIndex(i => i > 0);
            if (pond.IsLegendaryPond())
            {
                var familyCount = ModDataIO.ReadDataAs<int>(pond, "FamilyLivingHere");
                if (fishQualities.Sum() + familyCount != pond.FishCount + 1) // FishCount has already been decremented at this point, so we increment 1 to compensate
                    throw new InvalidDataException("FamilyLivingHere data is invalid.");

                if (familyCount > 0)
                {
                    var familyQualities =
                        ModDataIO.ReadData(pond, "FamilyQualities", $"{ModDataIO.ReadDataAs<int>(pond, "FamilyLivingHere")},0,0,0")
                            .ParseList<int>()!;
                    if (familyQualities.Count != 4 || familyQualities.Sum() != familyCount)
                        throw new InvalidDataException("FamilyQualities data had incorrect number of values.");

                    var lowestFamily = familyQualities.FindIndex(i => i > 0);
                    if (lowestFamily < lowestFish || lowestFamily == lowestFish && Game1.random.NextDouble() < 0.5)
                    {
                        whichFish = Utils.ExtendedFamilyPairs[whichFish];
                        fishQuality = lowestFamily == 3 ? 4 : lowestFamily;
                        --familyQualities[lowestFamily];
                        ModDataIO.WriteData(pond, "FamilyQualities", string.Join(",", familyQualities));
                        ModDataIO.IncrementData(pond, "FamilyLivingHere", -1);
                    }
                    else
                    {
                        fishQuality = lowestFish == 3 ? 4 : lowestFish;
                        --fishQualities[lowestFish];
                        ModDataIO.WriteData(pond, "FishQualities", string.Join(",", fishQualities));
                    }
                }
                else
                {
                    fishQuality = lowestFish == 3 ? 4 : lowestFish;
                    --fishQualities[lowestFish];
                    ModDataIO.WriteData(pond, "FishQualities", string.Join(",", fishQualities));
                }
            }
            else
            {
                fishQuality = lowestFish == 3 ? 4 : lowestFish;
                --fishQualities[lowestFish];
                ModDataIO.WriteData(pond, "FishQualities", string.Join(",", fishQualities));
            }
        }
        catch (InvalidDataException ex)
        {
            Log.W($"{ex}\nThe data will be reset.");
            ModDataIO.WriteData(pond, "FishQualities", $"{pond.FishCount},0,0,0");
            ModDataIO.WriteData(pond, "FamilyQualities", null);
            ModDataIO.WriteData(pond, "FamilyLivingHere", null);
        }
        catch (Exception ex)
        {
            Log.E($"Failed in {MethodBase.GetCurrentMethod()?.Name}:\n{ex}");
        }
    }

    #endregion harmony patches
}