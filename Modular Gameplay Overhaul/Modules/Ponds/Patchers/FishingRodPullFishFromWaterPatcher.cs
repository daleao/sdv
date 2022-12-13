namespace DaLion.Overhaul.Modules.Ponds.Patchers;

#region using directives

using System.IO;
using System.Linq;
using System.Reflection;
using DaLion.Overhaul.Modules.Ponds.Extensions;
using DaLion.Overhaul.Modules.Professions.Extensions;
using DaLion.Shared.Extensions;
using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley.Buildings;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class FishingRodPullFishFromWaterPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="FishingRodPullFishFromWaterPatcher"/> class.</summary>
    internal FishingRodPullFishFromWaterPatcher()
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

        var (x, y) = Reflector
            .GetUnboundMethodDelegate<Func<FishingRod, Vector2>>(__instance, "calculateBobberTile")
            .Invoke(__instance);
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

                var seaweedCount = pond.Read<int>(DataFields.SeaweedLivingHere);
                var greenAlgaeCount = pond.Read<int>(DataFields.GreenAlgaeLivingHere);
                var whiteAlgaeCount = pond.Read<int>(DataFields.WhiteAlgaeLivingHere);

                var roll = Game1.random.Next(seaweedCount + greenAlgaeCount + whiteAlgaeCount);
                if (roll < seaweedCount)
                {
                    whichFish = Constants.SeaweedIndex;
                    pond.Write(DataFields.SeaweedLivingHere, (--seaweedCount).ToString());
                }
                else if (roll < seaweedCount + greenAlgaeCount)
                {
                    whichFish = Constants.GreenAlgaeIndex;
                    pond.Write(DataFields.GreenAlgaeLivingHere, (--greenAlgaeCount).ToString());
                }
                else if (roll < seaweedCount + greenAlgaeCount + whiteAlgaeCount)
                {
                    whichFish = Constants.WhiteAlgaeIndex;
                    pond.Write(DataFields.WhiteAlgaeLivingHere, (--whiteAlgaeCount).ToString());
                }

                var total = __instance.Read<int>(DataFields.SeaweedLivingHere) +
                            __instance.Read<int>(DataFields.GreenAlgaeLivingHere) +
                            __instance.Read<int>(DataFields.WhiteAlgaeLivingHere);
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
            pond.Write(DataFields.SeaweedLivingHere, null);
            pond.Write(DataFields.GreenAlgaeLivingHere, null);
            pond.Write(DataFields.WhiteAlgaeLivingHere, null);
            var field = pond.fishType.Value switch
            {
                Constants.SeaweedIndex => DataFields.SeaweedLivingHere,
                Constants.GreenAlgaeIndex => DataFields.GreenAlgaeLivingHere,
                Constants.WhiteAlgaeIndex => DataFields.WhiteAlgaeLivingHere,
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
            var fishQualities = pond.Read(DataFields.FishQualities, $"{pond.FishCount - pond.Read<int>(DataFields.FamilyLivingHere)},0,0,0").ParseList<int>();
            if (fishQualities.Count != 4 || fishQualities.Any(q => q < 0 || q > pond.FishCount + 1)) // FishCount has already been decremented at this point, so we increment 1 to compensate
            {
                ThrowHelper.ThrowInvalidDataException("FishQualities data had incorrect number of values.");
            }

            var lowestFish = fishQualities.FindIndex(i => i > 0);
            if (pond.HasLegendaryFish())
            {
                var familyCount = pond.Read<int>(DataFields.FamilyLivingHere);
                if (fishQualities.Sum() + familyCount != pond.FishCount + 1) // FishCount has already been decremented at this point, so we increment 1 to compensate
                {
                    ThrowHelper.ThrowInvalidDataException("FamilyLivingHere data is invalid.");
                }

                if (familyCount > 0)
                {
                    var familyQualities =
                        pond.Read(DataFields.FamilyQualities, $"{pond.Read<int>(DataFields.FamilyLivingHere)},0,0,0")
                            .ParseList<int>();
                    if (familyQualities.Count != 4 || familyQualities.Sum() != familyCount)
                    {
                        ThrowHelper.ThrowInvalidDataException("FamilyQualities data had incorrect number of values.");
                    }

                    var lowestFamily = familyQualities.FindIndex(i => i > 0);
                    if (lowestFamily < lowestFish || (lowestFamily == lowestFish && Game1.random.NextDouble() < 0.5))
                    {
                        whichFish = Collections.ExtendedFamilyPairs[whichFish];
                        fishQuality = lowestFamily == 3 ? 4 : lowestFamily;
                        familyQualities[lowestFamily]--;
                        pond.Write(DataFields.FamilyQualities, string.Join(",", familyQualities));
                        pond.Increment(DataFields.FamilyLivingHere, -1);
                    }
                    else
                    {
                        fishQuality = lowestFish == 3 ? 4 : lowestFish;
                        fishQualities[lowestFish]--;
                        pond.Write(DataFields.FishQualities, string.Join(",", fishQualities));
                    }
                }
                else
                {
                    fishQuality = lowestFish == 3 ? 4 : lowestFish;
                    fishQualities[lowestFish]--;
                    pond.Write(DataFields.FishQualities, string.Join(",", fishQualities));
                }
            }
            else
            {
                fishQuality = lowestFish == 3 ? 4 : lowestFish;
                fishQualities[lowestFish]--;
                pond.Write(DataFields.FishQualities, string.Join(",", fishQualities));
            }
        }
        catch (InvalidDataException ex)
        {
            Log.W($"{ex}\nThe data will be reset.");
            pond.Write(DataFields.FishQualities, $"{pond.FishCount},0,0,0");
            pond.Write(DataFields.FamilyQualities, null);
            pond.Write(DataFields.FamilyLivingHere, null);
        }
        catch (Exception ex)
        {
            Log.E($"Failed in {MethodBase.GetCurrentMethod()?.Name}:\n{ex}");
        }
    }

    #endregion harmony patches
}
