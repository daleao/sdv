namespace DaLion.Stardew.Ponds.Framework.Patches;

#region using directives

using Common;
using Common.Extensions;
using Common.Extensions.Reflection;
using Common.ModData;
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
        if (!fromFishPond || whichFish.IsTrashIndex()) return;

        _CalculateBobberTile ??= typeof(FishingRod).RequireMethod("calculateBobberTile")
            .CompileUnboundDelegate<Func<FishingRod, Vector2>>();
        var (x, y) = _CalculateBobberTile.Invoke(__instance);
        var pond = Game1.getFarm().buildings.OfType<FishPond>().FirstOrDefault(p =>
            x > p.tileX.Value && x < p.tileX.Value + p.tilesWide.Value - 1 &&
            y > p.tileY.Value && y < p.tileY.Value + p.tilesHigh.Value - 1);
        if (pond is null || pond.FishCount < 0) return;

        try
        {
            if (pond.HasAlgae())
            {
                fishQuality = SObject.lowQuality;

                var seaweedCount = ModDataIO.Read<int>(pond, "SeaweedLivingHere");
                var greenAlgaeCount = ModDataIO.Read<int>(pond, "GreenAlgaeLivingHere");
                var whiteAlgaeCount = ModDataIO.Read<int>(pond, "WhiteAlgaeLivingHere");

                var roll = Game1.random.Next(seaweedCount + greenAlgaeCount + whiteAlgaeCount);
                if (roll < seaweedCount)
                {
                    whichFish = Constants.SEAWEED_INDEX_I;
                    ModDataIO.Write(pond, "SeaweedLivingHere", (--seaweedCount).ToString());
                }
                else if (roll < seaweedCount + greenAlgaeCount)
                {
                    whichFish = Constants.GREEN_ALGAE_INDEX_I;
                    ModDataIO.Write(pond, "GreenAlgaeLivingHere", (--greenAlgaeCount).ToString());
                }
                else if (roll < seaweedCount + greenAlgaeCount + whiteAlgaeCount)
                {
                    whichFish = Constants.WHITE_ALGAE_INDEX_I;
                    ModDataIO.Write(pond, "WhiteAlgaeLivingHere", (--whiteAlgaeCount).ToString());
                }

                var total = ModDataIO.Read<int>(__instance, "SeaweedLivingHere") +
                            ModDataIO.Read<int>(__instance, "GreenAlgaeLivingHere") +
                            ModDataIO.Read<int>(__instance, "WhiteAlgaeLivingHere");
                if (total != pond.FishCount)
                    throw new InvalidDataException("Mismatch between algae population data and actual population.");

                return;
            }
        }
        catch (InvalidDataException ex)
        {
            Log.W($"{ex}\nThe data will be reset.");
            ModDataIO.Write(pond, "SeaweedLivingHere", null);
            ModDataIO.Write(pond, "GreenAlgaeLivingHere", null);
            ModDataIO.Write(pond, "WhiteAlgaeLivingHere", null);
#pragma warning disable CS8509
            var field = pond.fishType.Value switch
#pragma warning restore CS8509
            {
                Constants.SEAWEED_INDEX_I => "SeaweedLivingHere",
                Constants.GREEN_ALGAE_INDEX_I => "GreenAlgaeLivingHere",
                Constants.WHITE_ALGAE_INDEX_I => "WhiteAlgaeLivingHere"
            };

            ModDataIO.Write(pond, field, pond.FishCount.ToString());
        }
        catch (Exception ex)
        {
            Log.E($"Failed in {MethodBase.GetCurrentMethod()?.Name}:\n{ex}");
        }

        try
        {
            var fishQualities = ModDataIO.Read(pond, "FishQualities",
                $"{pond.FishCount - ModDataIO.Read<int>(pond, "FamilyLivingHere")},0,0,0").ParseList<int>()!;
            if (fishQualities.Count != 4 || fishQualities.Any(q => 0 > q || q > pond.FishCount + 1)) // FishCount has already been decremented at this point, so we increment 1 to compensate
                throw new InvalidDataException("FishQualities data had incorrect number of values.");

            var lowestFish = fishQualities.FindIndex(i => i > 0);
            if (pond.HasLegendaryFish())
            {
                var familyCount = ModDataIO.Read<int>(pond, "FamilyLivingHere");
                if (fishQualities.Sum() + familyCount != pond.FishCount + 1) // FishCount has already been decremented at this point, so we increment 1 to compensate
                    throw new InvalidDataException("FamilyLivingHere data is invalid.");

                if (familyCount > 0)
                {
                    var familyQualities =
                        ModDataIO.Read(pond, "FamilyQualities", $"{ModDataIO.Read<int>(pond, "FamilyLivingHere")},0,0,0")
                            .ParseList<int>()!;
                    if (familyQualities.Count != 4 || familyQualities.Sum() != familyCount)
                        throw new InvalidDataException("FamilyQualities data had incorrect number of values.");

                    var lowestFamily = familyQualities.FindIndex(i => i > 0);
                    if (lowestFamily < lowestFish || lowestFamily == lowestFish && Game1.random.NextDouble() < 0.5)
                    {
                        whichFish = Utils.ExtendedFamilyPairs[whichFish];
                        fishQuality = lowestFamily == 3 ? 4 : lowestFamily;
                        --familyQualities[lowestFamily];
                        ModDataIO.Write(pond, "FamilyQualities", string.Join(",", familyQualities));
                        ModDataIO.Increment(pond, "FamilyLivingHere", -1);
                    }
                    else
                    {
                        fishQuality = lowestFish == 3 ? 4 : lowestFish;
                        --fishQualities[lowestFish];
                        ModDataIO.Write(pond, "FishQualities", string.Join(",", fishQualities));
                    }
                }
                else
                {
                    fishQuality = lowestFish == 3 ? 4 : lowestFish;
                    --fishQualities[lowestFish];
                    ModDataIO.Write(pond, "FishQualities", string.Join(",", fishQualities));
                }
            }
            else
            {
                fishQuality = lowestFish == 3 ? 4 : lowestFish;
                --fishQualities[lowestFish];
                ModDataIO.Write(pond, "FishQualities", string.Join(",", fishQualities));
            }
        }
        catch (InvalidDataException ex)
        {
            Log.W($"{ex}\nThe data will be reset.");
            ModDataIO.Write(pond, "FishQualities", $"{pond.FishCount},0,0,0");
            ModDataIO.Write(pond, "FamilyQualities", null);
            ModDataIO.Write(pond, "FamilyLivingHere", null);
        }
        catch (Exception ex)
        {
            Log.E($"Failed in {MethodBase.GetCurrentMethod()?.Name}:\n{ex}");
        }
    }

    #endregion harmony patches
}