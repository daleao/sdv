namespace DaLion.Stardew.Ponds.Framework.Patches;

#region using directives

using Common;
using Common.Extensions;
using Common.ModData;
using Extensions;
using HarmonyLib;
using StardewValley.Buildings;
using System;
using System.IO;
using System.Linq;

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
                        ModDataIO.Increment<int>(__instance, "SeaweedLivingHere");
                        break;
                    case Constants.GREEN_ALGAE_INDEX_I:
                        ModDataIO.Increment<int>(__instance, "GreenAlgaeLivingHere");
                        break;
                    case Constants.WHITE_ALGAE_INDEX_I:
                        ModDataIO.Increment<int>(__instance, "WhiteAlgaeLivingHere");
                        break;
                }

                var total = ModDataIO.Read<int>(__instance, "SeaweedLivingHere") +
                            ModDataIO.Read<int>(__instance, "GreenAlgaeLivingHere") +
                            ModDataIO.Read<int>(__instance, "WhiteAlgaeLivingHere");
                if (total != __instance.FishCount)
                    throw new InvalidDataException("Mismatch between algae population data and actual population.");

                return;
            }
        }
        catch (InvalidDataException ex)
        {
            Log.W($"{ex}\nThe data will be reset.");
            ModDataIO.Write(__instance, "SeaweedLivingHere", null);
            ModDataIO.Write(__instance, "GreenAlgaeLivingHere", null);
            ModDataIO.Write(__instance, "WhiteAlgaeLivingHere", null);
            var field = __instance.fishType.Value switch
            {
                Constants.SEAWEED_INDEX_I => "SeaweedLivingHere",
                Constants.GREEN_ALGAE_INDEX_I => "GreenAlgaeLivingHere",
                Constants.WHITE_ALGAE_INDEX_I => "WhiteAlgaeLivingHere",
                _ => string.Empty
            };

            ModDataIO.Write(__instance, field, __instance.FishCount.ToString());
        }

        try
        {
            var forFamily = false;
            var familyCount = 0;
            if (__instance.HasLegendaryFish())
            {
                familyCount = ModDataIO.Read<int>(__instance, "FamilyLivingHere");
                if (0 > familyCount || familyCount > __instance.FishCount)
                    throw new InvalidDataException(
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
            var qualities = ModDataIO
                .Read(__instance, forFamily ? "FamilyQualities" : "FishQualities", @default)
                .ParseList<int>()!;
            if (qualities.Count != 4 ||
                qualities.Sum() != (forFamily ? familyCount : __instance.FishCount - familyCount - 1))
                throw new InvalidDataException("Mismatch between FishQualities data and actual population.");

            if (qualities.Sum() == 0)
            {
                ++qualities[0];
                ModDataIO.Write(__instance, forFamily ? "FamilyQualities" : "FishQualities",
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
            ModDataIO.Write(__instance, forFamily ? "FamilyQualities" : "FishQualities",
                string.Join(',', qualities));
        }
        catch (InvalidDataException ex)
        {
            Log.W($"{ex}\nThe data will be reset.");
            ModDataIO.Write(__instance, "FishQualities", $"{__instance.FishCount},0,0,0");
            ModDataIO.Write(__instance, "FamilyQualities", null);
            ModDataIO.Write(__instance, "FamilyLivingHere", null);
        }
    }

    #endregion harmony patches
}