namespace DaLion.Ponds.Framework.Patchers;

#region using directives

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using DaLion.Shared.Classes;
using DaLion.Shared.Constants;
using DaLion.Shared.Extensions;
using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Harmony;
using DaLion.Shared.Reflection;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley.Buildings;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class FishingRodPullFishFromWaterPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="FishingRodPullFishFromWaterPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    internal FishingRodPullFishFromWaterPatcher(Harmonizer harmonizer)
    : base(harmonizer)
    {
        this.Target = this.RequireMethod<FishingRod>(nameof(FishingRod.pullFishFromWater));
    }

    #region harmony patches

    /// <summary>Decrement total Fish Pond quality ratings.</summary>
    [HarmonyPrefix]
    private static void FishingRodPullFishFromWaterPrefix(
        FishingRod __instance, ref string fishId, ref int fishQuality, bool fromFishPond)
    {
        if (!fromFishPond || fishId.IsTrashId())
        {
            return;
        }

        try
        {
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

            if (pond.HasAlgae())
            {
                PullAlgae(pond, ref fishId, ref fishQuality);
            }
            else
            {
                PullFish(pond, ref fishId, ref fishQuality);
            }
        }
        catch (Exception ex)
        {
            Log.E($"Failed in {MethodBase.GetCurrentMethod()?.Name}:\n{ex}");
        }
    }

    #endregion harmony patches

    #region handlers

    private static void PullAlgae(FishPond pond, ref string id, ref int quality)
    {
        quality = SObject.lowQuality;
        try
        {
            var seaweedCount = Data.ReadAs<int>(pond, DataKeys.SeaweedLivingHere);
            var greenAlgaeCount = Data.ReadAs<int>(pond, DataKeys.GreenAlgaeLivingHere);
            var whiteAlgaeCount = Data.ReadAs<int>(pond, DataKeys.WhiteAlgaeLivingHere);
            var roll = Game1.random.Next(seaweedCount + greenAlgaeCount + whiteAlgaeCount);
            if (roll < seaweedCount)
            {
                id = QualifiedObjectIds.Seaweed;
                Data.Write(pond, DataKeys.SeaweedLivingHere, (--seaweedCount).ToString());
            }
            else if (roll < seaweedCount + greenAlgaeCount)
            {
                id = QualifiedObjectIds.GreenAlgae;
                Data.Write(pond, DataKeys.GreenAlgaeLivingHere, (--greenAlgaeCount).ToString());
            }
            else if (roll < seaweedCount + greenAlgaeCount + whiteAlgaeCount)
            {
                id = QualifiedObjectIds.WhiteAlgae;
                Data.Write(pond, DataKeys.WhiteAlgaeLivingHere, (--whiteAlgaeCount).ToString());
            }

            var total = Data.ReadAs<int>(pond, DataKeys.SeaweedLivingHere) +
                        Data.ReadAs<int>(pond, DataKeys.GreenAlgaeLivingHere) +
                        Data.ReadAs<int>(pond, DataKeys.WhiteAlgaeLivingHere);
            if (total != pond.FishCount)
            {
                ThrowHelper.ThrowInvalidDataException(
                    "Mismatch between algae population data and actual population.");
            }
        }
        catch (InvalidDataException ex)
        {
            Log.W($"{ex}\nThe data will be reset.");
            Data.Write(pond, DataKeys.SeaweedLivingHere, null);
            Data.Write(pond, DataKeys.GreenAlgaeLivingHere, null);
            Data.Write(pond, DataKeys.WhiteAlgaeLivingHere, null);
            var field = pond.fishType.Value switch
            {
                QualifiedObjectIds.Seaweed => DataKeys.SeaweedLivingHere,
                QualifiedObjectIds.GreenAlgae => DataKeys.GreenAlgaeLivingHere,
                QualifiedObjectIds.WhiteAlgae => DataKeys.WhiteAlgaeLivingHere,
                _ => string.Empty,
            };

            Data.Write(pond, field, pond.FishCount.ToString());
        }
    }

    private static void PullFish(FishPond pond, ref string id, ref int quality)
    {
        try
        {
            var fishQualities = Data.Read(
                pond,
                DataKeys.FishQualities,
                $"{pond.FishCount - Data.ReadAs<int>(pond, DataKeys.FamilyLivingHere, modId: "DaLion.Professions")},0,0,0").ParseList<int>();
            if (fishQualities.Count != 4 ||
                fishQualities.Any(q =>
                    q < 0 || q > pond.FishCount +
                    1)) // FishCount has already been decremented at this point, so we increment 1 to compensate
            {
                ThrowHelper.ThrowInvalidDataException("FishQualities data had incorrect number of values.");
            }

            var lowestFish = fishQualities.FindIndex(i => i > 0);
            if (pond.HasLegendaryFish())
            {
                PullLegendary(pond, ref id, ref quality, fishQualities, lowestFish);
            }
            else
            {
                quality = lowestFish == 3 ? 4 : lowestFish;
                fishQualities[lowestFish]--;
                Data.Write(pond, DataKeys.FishQualities, string.Join(",", fishQualities));
            }
        }
        catch (InvalidDataException ex)
        {
            Log.W($"{ex}\nThe data will be reset.");
            Data.Write(pond, DataKeys.FishQualities, $"{pond.FishCount},0,0,0");
            Data.Write(pond, DataKeys.FamilyQualities, null);
            Data.Write(pond, DataKeys.FamilyLivingHere, null);
        }
    }

    private static void PullLegendary(FishPond pond, ref string id, ref int quality, List<int> fishQualities, int lowestFish)
    {
        var familyCount = Data.ReadAs<int>(pond, DataKeys.FamilyLivingHere, modId: "DaLion.Professions");
        if (fishQualities.Sum() + familyCount !=
            pond.FishCount +
            1) // FishCount has already been decremented at this point, so we increment 1 to compensate
        {
            ThrowHelper.ThrowInvalidDataException("FamilyLivingHere data is invalid.");
        }

        if (familyCount > 0)
        {
            var familyQualities =
                Data.Read(pond, DataKeys.FamilyQualities, $"{Data.ReadAs<int>(pond, DataKeys.FamilyLivingHere, modId: "DaLion.Professions")},0,0,0")
                    .ParseList<int>();
            if (familyQualities.Count != 4 || familyQualities.Sum() != familyCount)
            {
                ThrowHelper.ThrowInvalidDataException("FamilyQualities data had incorrect number of values.");
            }

            var lowestFamily = familyQualities.FindIndex(i => i > 0);
            if (lowestFamily < lowestFish || (lowestFamily == lowestFish && Game1.random.NextDouble() < 0.5))
            {
                id = Lookups.FamilyPairs.TryGet(id, out var pairId) && !string.IsNullOrEmpty(pairId) ? pairId : id;
                quality = lowestFamily == 3 ? 4 : lowestFamily;
                familyQualities[lowestFamily]--;
                Data.Write(pond, DataKeys.FamilyQualities, string.Join(",", familyQualities));
                Data.Increment(pond, DataKeys.FamilyLivingHere, -1);
            }
            else
            {
                quality = lowestFish == 3 ? 4 : lowestFish;
                fishQualities[lowestFish]--;
                Data.Write(pond, DataKeys.FishQualities, string.Join(",", fishQualities));
            }
        }
        else
        {
            quality = lowestFish == 3 ? 4 : lowestFish;
            fishQualities[lowestFish]--;
            Data.Write(pond, DataKeys.FishQualities, string.Join(",", fishQualities));
        }
    }

    #endregion handlers
}
