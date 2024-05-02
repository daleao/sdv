namespace DaLion.Ponds.Framework.Patchers;

#region using directives

using System.IO;
using System.Linq;
using DaLion.Shared.Constants;
using DaLion.Shared.Extensions;
using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Buildings;
using StardewValley.GameData.FishPonds;

#endregion using directives

[UsedImplicitly]
internal sealed class FishPondAddFishToPondPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="FishPondAddFishToPondPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    internal FishPondAddFishToPondPatcher(Harmonizer harmonizer)
        : base(harmonizer)
    {
        this.Target = this.RequireMethod<FishPond>("addFishToPond");
    }

    #region harmony patches

    /// <summary>Distinguish extended family pairs + increment total Fish Pond quality ratings.</summary>
    [HarmonyPostfix]
    private static void FishPondAddFishToPondPostfix(FishPond __instance, FishPondData ____fishPondData, SObject fish)
    {
        try
        {
            if (Lookups.LegendaryFishes.Contains(fish.QualifiedItemId))
            {
                if (fish.ItemId != __instance.fishType.Value)
                {
                    var familyQualities = Data
                        .Read(
                            __instance,
                            DataKeys.FamilyQualities,
                            $"{Data.ReadAs<int>(__instance, DataKeys.FamilyLivingHere, modId: "DaLion.Professions")},0,0,0")
                        .ParseList<int>();
                    if (familyQualities.Count != 4 ||
                        familyQualities.Sum() != Data.ReadAs<int>(__instance, DataKeys.FamilyLivingHere, modId: "DaLion.Professions"))
                    {
                        ThrowHelper.ThrowInvalidDataException("FamilyQualities data had incorrect number of values.");
                    }

                    familyQualities[fish.Quality == 4 ? 3 : fish.Quality]++;
                    Data.Increment(__instance, DataKeys.FamilyLivingHere, modId: "DaLion.Professions");
                    Data.Write(__instance, DataKeys.FamilyQualities, string.Join(',', familyQualities));
                }
                else
                {
                    var fishQualities = Data
                        .Read(
                            __instance,
                            DataKeys.FishQualities,
                            $"{__instance.FishCount - Data.ReadAs<int>(__instance, DataKeys.FamilyLivingHere, modId: "DaLion.Professions") - 1},0,0,0") // already added at this point, so consider - 1
                        .ParseList<int>();
                    if (fishQualities.Count != 4 || fishQualities.Any(q => q < 0 || q > __instance.FishCount - 1))
                    {
                        ThrowHelper.ThrowInvalidDataException("FishQualities data had incorrect number of values.");
                    }

                    fishQualities[fish.Quality == 4 ? 3 : fish.Quality]++;
                    Data.Write(__instance, DataKeys.FishQualities, string.Join(',', fishQualities));
                }

                // enable reproduction if angler or ms. angler
                if (fish.ItemId is not ("160" or "899") ||
                    Data.ReadAs<int>(__instance, DataKeys.FamilyLivingHere, modId: "DaLion.Professions") is not (var familyCount and > 0))
                {
                    return;
                }

                var mates = Math.Min(__instance.FishCount - familyCount, familyCount);
                ____fishPondData.SpawnTime = 12 / mates;
            }
            else if (fish.IsAlgae())
            {
                switch (fish.QualifiedItemId)
                {
                    case QualifiedObjectIds.Seaweed:
                        Data.Increment(__instance, DataKeys.SeaweedLivingHere);
                        break;
                    case QualifiedObjectIds.GreenAlgae:
                        Data.Increment(__instance, DataKeys.GreenAlgaeLivingHere);
                        break;
                    case QualifiedObjectIds.WhiteAlgae:
                        Data.Increment(__instance, DataKeys.WhiteAlgaeLivingHere);
                        break;
                }
            }
            else
            {
                var fishQualities = Data
                    .Read(
                        __instance,
                        DataKeys.FishQualities,
                        $"{__instance.FishCount - Data.ReadAs<int>(__instance, DataKeys.FamilyLivingHere, modId: "DaLion.Professions") - 1},0,0,0") // already added at this point, so consider - 1
                    .ParseList<int>();
                if (fishQualities.Count != 4 || fishQualities.Any(q => q < 0 || q > __instance.FishCount - 1))
                {
                    ThrowHelper.ThrowInvalidDataException("FishQualities data had incorrect number of values.");
                }

                fishQualities[fish.Quality == 4 ? 3 : fish.Quality]++;
                Data.Write(__instance, DataKeys.FishQualities, string.Join(',', fishQualities));
            }
        }
        catch (InvalidDataException ex)
        {
            Log.W($"{ex}\nThe data will be reset.");
            Data.Write(__instance, DataKeys.FishQualities, $"{__instance.FishCount},0,0,0");
            Data.Write(__instance, DataKeys.FamilyQualities, null);
            Data.Write(__instance, DataKeys.FamilyLivingHere, null);
        }
    }

    #endregion harmony patches
}
