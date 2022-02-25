using System;

namespace DaLion.Stardew.Professions.Framework.Patches.Fishing;

#region using directives

using HarmonyLib;
using JetBrains.Annotations;
using StardewValley;
using StardewValley.Buildings;

using Stardew.Common.Extensions;
using Extensions;

using SObject = StardewValley.Object;

#endregion using directives

[UsedImplicitly]
internal class FishPondSpawnFishPatch : BasePatch
{
    /// <summary>Construct an instance.</summary>
    internal FishPondSpawnFishPatch()
    {
        Original = RequireMethod<FishPond>(nameof(FishPond.SpawnFish));
    }

    #region harmony patches

    /// <summary>Patch to set the quality of newborn fishes.</summary>
    [HarmonyPostfix]
    private static void FishPondSpawnFishPostfix(FishPond __instance)
    {
        if (!ModEntry.Config.EnableFishPondRebalance) return;

        var qualityRating = __instance.ReadDataAs<int>("QualityRating");
        var (numBestQuality, numHighQuality, numMedQuality) = __instance.GetFishQualities(qualityRating);
        if (numBestQuality == 0 && numHighQuality == 0 && numMedQuality == 0)
        {
            __instance.WriteData("QualityRating", (++qualityRating).ToString());
            return;
        }

        var roll = Game1.random.Next(__instance.FishCount - 1); // fish pond count has already been incremented at this point, so we consider -1;
        var fishlingQuality = roll < numBestQuality
            ? SObject.bestQuality
            : roll < numBestQuality + numHighQuality
                ? SObject.highQuality
                : roll < numBestQuality + numHighQuality + numMedQuality
                    ? SObject.medQuality
                    : SObject.lowQuality;

        qualityRating += (int) Math.Pow(16,
            fishlingQuality == SObject.bestQuality ? fishlingQuality - 1 : fishlingQuality);
        __instance.WriteData("QualityRating", qualityRating.ToString());
    }

    #endregion harmony patches
}