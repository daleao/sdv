namespace DaLion.Overhaul.Modules.Ponds.Patchers;

#region using directives

using System.Collections.Generic;
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
                PullAlgae(pond, ref whichFish, ref fishQuality);
            }
            else
            {
                HandleFish(pond, ref whichFish, ref fishQuality);
            }
        }
        catch (Exception ex)
        {
            Log.E($"Failed in {MethodBase.GetCurrentMethod()?.Name}:\n{ex}");
        }
    }

    #endregion harmony patches

    #region handlers

    private static void PullAlgae(FishPond pond, ref int index, ref int quality)
    {
        quality = SObject.lowQuality;
        try
        {
            var seaweedCount = pond.Read<int>(DataKeys.SeaweedLivingHere);
            var greenAlgaeCount = pond.Read<int>(DataKeys.GreenAlgaeLivingHere);
            var whiteAlgaeCount = pond.Read<int>(DataKeys.WhiteAlgaeLivingHere);

            var roll = Game1.random.Next(seaweedCount + greenAlgaeCount + whiteAlgaeCount);
            if (roll < seaweedCount)
            {
                index = ItemIDs.Seaweed;
                pond.Write(DataKeys.SeaweedLivingHere, (--seaweedCount).ToString());
            }
            else if (roll < seaweedCount + greenAlgaeCount)
            {
                index = ItemIDs.GreenAlgae;
                pond.Write(DataKeys.GreenAlgaeLivingHere, (--greenAlgaeCount).ToString());
            }
            else if (roll < seaweedCount + greenAlgaeCount + whiteAlgaeCount)
            {
                index = ItemIDs.WhiteAlgae;
                pond.Write(DataKeys.WhiteAlgaeLivingHere, (--whiteAlgaeCount).ToString());
            }

            var total = pond.Read<int>(DataKeys.SeaweedLivingHere) +
                        pond.Read<int>(DataKeys.GreenAlgaeLivingHere) +
                        pond.Read<int>(DataKeys.WhiteAlgaeLivingHere);
            if (total != pond.FishCount)
            {
                ThrowHelper.ThrowInvalidDataException(
                    "Mismatch between algae population data and actual population.");
            }
        }
        catch (InvalidDataException ex)
        {
            Log.W($"[PNDS]: {ex}\nThe data will be reset.");
            pond.Write(DataKeys.SeaweedLivingHere, null);
            pond.Write(DataKeys.GreenAlgaeLivingHere, null);
            pond.Write(DataKeys.WhiteAlgaeLivingHere, null);
            var field = pond.fishType.Value switch
            {
                ItemIDs.Seaweed => DataKeys.SeaweedLivingHere,
                ItemIDs.GreenAlgae => DataKeys.GreenAlgaeLivingHere,
                ItemIDs.WhiteAlgae => DataKeys.WhiteAlgaeLivingHere,
                _ => string.Empty,
            };

            pond.Write(field, pond.FishCount.ToString());
        }
    }

    private static void HandleFish(FishPond pond, ref int index, ref int quality)
    {
        try
        {
            var fishQualities = pond.Read(
                DataKeys.FishQualities,
                $"{pond.FishCount - pond.Read<int>(DataKeys.FamilyLivingHere)},0,0,0").ParseList<int>();
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
                HandleLegendary(pond, ref index, ref quality, fishQualities, lowestFish);
            }
            else
            {
                quality = lowestFish == 3 ? 4 : lowestFish;
                fishQualities[lowestFish]--;
                pond.Write(DataKeys.FishQualities, string.Join(",", fishQualities));
            }
        }
        catch (InvalidDataException ex)
        {
            Log.W($"[PNDS]: {ex}\nThe data will be reset.");
            pond.Write(DataKeys.FishQualities, $"{pond.FishCount},0,0,0");
            pond.Write(DataKeys.FamilyQualities, null);
            pond.Write(DataKeys.FamilyLivingHere, null);
        }
    }

    private static void HandleLegendary(FishPond pond, ref int index, ref int quality, List<int> fishQualities, int lowestFish)
    {
        var familyCount = pond.Read<int>(DataKeys.FamilyLivingHere);
        if (fishQualities.Sum() + familyCount !=
            pond.FishCount +
            1) // FishCount has already been decremented at this point, so we increment 1 to compensate
        {
            ThrowHelper.ThrowInvalidDataException("FamilyLivingHere data is invalid.");
        }

        if (familyCount > 0)
        {
            var familyQualities =
                pond.Read(DataKeys.FamilyQualities, $"{pond.Read<int>(DataKeys.FamilyLivingHere)},0,0,0")
                    .ParseList<int>();
            if (familyQualities.Count != 4 || familyQualities.Sum() != familyCount)
            {
                ThrowHelper.ThrowInvalidDataException("FamilyQualities data had incorrect number of values.");
            }

            var lowestFamily = familyQualities.FindIndex(i => i > 0);
            if (lowestFamily < lowestFish || (lowestFamily == lowestFish && Game1.random.NextDouble() < 0.5))
            {
                index = Collections.ExtendedFamilyPairs[index];
                quality = lowestFamily == 3 ? 4 : lowestFamily;
                familyQualities[lowestFamily]--;
                pond.Write(DataKeys.FamilyQualities, string.Join(",", familyQualities));
                pond.Increment(DataKeys.FamilyLivingHere, -1);
            }
            else
            {
                quality = lowestFish == 3 ? 4 : lowestFish;
                fishQualities[lowestFish]--;
                pond.Write(DataKeys.FishQualities, string.Join(",", fishQualities));
            }
        }
        else
        {
            quality = lowestFish == 3 ? 4 : lowestFish;
            fishQualities[lowestFish]--;
            pond.Write(DataKeys.FishQualities, string.Join(",", fishQualities));
        }
    }

    #endregion handlers
}
