namespace DaLion.Stardew.Ponds.Framework.Patches;

#region using directives

using System;
using System.IO;
using System.Linq;
using HarmonyLib;
using JetBrains.Annotations;
using StardewValley;
using StardewValley.Buildings;

using Common;
using Common.Extensions;
using Common.Harmony;
using Extensions;

using SObject = StardewValley.Object;

#endregion using directives

[UsedImplicitly]
internal sealed class FishPondSpawnFishPatch : BasePatch
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
        if (__instance.fishType.Value.IsAlgae())
        {
            var spawned = r.NextDouble() > 0.25
                ? r.Next(Constants.SEAWEED_INDEX_I, Constants.GREEN_ALGAE_INDEX_I + 1)
                : 157;
            switch (spawned)
            {
                case Constants.SEAWEED_INDEX_I:
                    __instance.IncrementData<int>("SeaweedLivingHere");
                    break;
                case Constants.GREEN_ALGAE_INDEX_I:
                    __instance.IncrementData<int>("GreenAlgaeLivingHere");
                    break;
                case Constants.WHITE_ALGAE_INDEX_I:
                    __instance.IncrementData<int>("WhiteAlgaeLivingHere");
                    break;
            }

            return;
        }

        try
        {
            var forFamily = false;
            var familyCount = 0;
            if (__instance.IsLegendaryPond())
            {
                familyCount = __instance.ReadDataAs<int>("FamilyLivingHere");
                if (0 > familyCount || familyCount > __instance.FishCount)
                    throw new InvalidDataException("FamilyLivingHere data is invalid.");

                if (familyCount > 0 &&
                    Game1.random.NextDouble() <
                    (double) familyCount /
                    (__instance.FishCount -
                     1)) // fish pond count has already been incremented at this point, so we consider -1;
                    forFamily = true;
            }

            var @default = forFamily
                ? $"{familyCount},0,0,0"
                : $"{__instance.FishCount - familyCount - 1},0,0,0";
            var qualities = __instance.ReadData(forFamily ? "FamilyQualities" : "FishQualities", @default)
                .ParseList<int>()!;
            if (qualities.Count != 4 ||
                qualities.Sum() != (forFamily ? familyCount : __instance.FishCount - familyCount - 1))
                throw new InvalidDataException("FishQualities data had incorrect number of values.");

            if (qualities.Sum() == 0)
            {
                ++qualities[0];
                __instance.WriteData(forFamily ? "FamilyQualities" : "FishQualities",
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
            __instance.WriteData(forFamily ? "FamilyQualities" : "FishQualities", string.Join(',', qualities));
        }
        catch (InvalidDataException ex)
        {
            Log.W($"{ex}\nThe data will be reset.");
            __instance.WriteData("FishQualities", $"{__instance.FishCount},0,0,0");
            __instance.WriteData("FamilyQualities", null);
            __instance.WriteData("FamilyLivingHere", null);
        }
    }

    #endregion harmony patches
}