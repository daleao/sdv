namespace DaLion.Stardew.Ponds.Framework.Patches;

#region using directives

using Common;
using Common.Extensions;
using Common.Extensions.Stardew;
using Extensions;
using HarmonyLib;
using LinqFasterer;
using StardewValley.Buildings;
using System;
using System.IO;

#endregion using directives

[UsedImplicitly]
internal sealed class FishPondSpawnFishPatch : Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal FishPondSpawnFishPatch()
    {
        Target = RequireMethod<FishPond>(nameof(FishPond.SpawnFish));
    }

    #region harmony patches

    /// <summary>Set the quality of newborn fishes.</summary>
    [HarmonyPostfix]
    private static void FishPondSpawnFishPostfix(FishPond __instance)
    {
        if (__instance.currentOccupants.Value >= __instance.maxOccupants.Value &&
            !__instance.hasSpawnedFish.Value) return;

        var r = new Random(Guid.NewGuid().GetHashCode());
        try
        {
            if (__instance.fishType.Value.IsAlgaeIndex())
            {
                var spawned = Utils.ChooseAlgae(__instance.fishType.Value, r);
                switch (spawned)
                {
                    case Constants.SEAWEED_INDEX_I:
                        __instance.Increment("SeaweedLivingHere");
                        break;
                    case Constants.GREEN_ALGAE_INDEX_I:
                        __instance.Increment("GreenAlgaeLivingHere");
                        break;
                    case Constants.WHITE_ALGAE_INDEX_I:
                        __instance.Increment("WhiteAlgaeLivingHere");
                        break;
                }

                var total = __instance.Read<int>("SeaweedLivingHere") +
                            __instance.Read<int>("GreenAlgaeLivingHere") +
                            __instance.Read<int>("WhiteAlgaeLivingHere");
                if (total != __instance.FishCount)
                    ThrowHelper.ThrowInvalidDataException("Mismatch between algae population data and actual population.");

                return;
            }
        }
        catch (InvalidDataException ex)
        {
            Log.W($"{ex}\nThe data will be reset.");
            __instance.Write("SeaweedLivingHere", null);
            __instance.Write("GreenAlgaeLivingHere", null);
            __instance.Write("WhiteAlgaeLivingHere", null);
            var field = __instance.fishType.Value switch
            {
                Constants.SEAWEED_INDEX_I => "SeaweedLivingHere",
                Constants.GREEN_ALGAE_INDEX_I => "GreenAlgaeLivingHere",
                Constants.WHITE_ALGAE_INDEX_I => "WhiteAlgaeLivingHere",
                _ => string.Empty
            };

            __instance.Write(field, __instance.FishCount.ToString());
        }

        try
        {
            var forFamily = false;
            var familyCount = 0;
            if (__instance.HasLegendaryFish())
            {
                familyCount = __instance.Read<int>("FamilyLivingHere");
                if (0 > familyCount || familyCount > __instance.FishCount)
                    ThrowHelper.ThrowInvalidDataException(
                        "FamilyLivingHere data is negative or greater than actual population.");

                if (familyCount > 0 &&
                    Game1.random.NextDouble() <
                    (double)familyCount /
                    (__instance.FishCount -
                     1)) // fish pond count has already been incremented at this point, so we consider -1;
                    forFamily = true;
            }

            var @default = forFamily
                ? $"{familyCount},0,0,0"
                : $"{__instance.FishCount - familyCount - 1},0,0,0";
            var qualities = __instance
                .Read(forFamily ? "FamilyQualities" : "FishQualities", @default)
                .ParseList<int>();
            if (qualities.Count != 4 ||
                qualities.SumF() != (forFamily ? familyCount : __instance.FishCount - familyCount - 1))
                ThrowHelper.ThrowInvalidDataException("Mismatch between FishQualities data and actual population.");

            if (qualities.SumF() == 0)
            {
                ++qualities[0];
                __instance.Write(forFamily ? "FamilyQualities" : "FishQualities",
                    string.Join(',', qualities));
                return;
            }

            var roll = r.Next(forFamily ? familyCount : __instance.FishCount - familyCount - 1);
            var fishlingQuality = roll < qualities[3]
                ? SObject.bestQuality
                : roll < qualities[3] + qualities[2]
                    ? SObject.highQuality
                    : roll < qualities[3] + qualities[2] + qualities[1]
                        ? SObject.medQuality
                        : SObject.lowQuality;

            ++qualities[fishlingQuality == 4 ? 3 : fishlingQuality];
            __instance.Write(forFamily ? "FamilyQualities" : "FishQualities",
                string.Join(',', qualities));
        }
        catch (InvalidDataException ex)
        {
            Log.W($"{ex}\nThe data will be reset.");
            __instance.Write("FishQualities", $"{__instance.FishCount},0,0,0");
            __instance.Write("FamilyQualities", null);
            __instance.Write("FamilyLivingHere", null);
        }
    }

    #endregion harmony patches
}