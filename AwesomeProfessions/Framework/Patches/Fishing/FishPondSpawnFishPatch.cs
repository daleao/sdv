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

        var owner = Game1.getFarmerMaybeOffline(__instance.owner.Value) ?? Game1.MasterPlayer;
        var qualityRatingByFishPond =
            ModData.Read(DataField.QualityRatingByFishPond, owner).ToDictionary<int, int>(",", ";");
        var thisFishPond = __instance.GetCenterTile().ToString().GetDeterministicHashCode();
        
        var (numBestQuality, numHighQuality, numMedQuality) = __instance.GetAllFishQualities();
        if (numBestQuality == 0 && numHighQuality == 0 && numMedQuality == 0)
        {
            ++qualityRatingByFishPond[thisFishPond];
            ModData.Write(DataField.QualityRatingByFishPond, qualityRatingByFishPond.ToString(",", ";"), owner);
            return;
        }

        var roll = Game1.random.Next(__instance.FishCount + 1);
        var fishlingQuality = roll < numBestQuality
            ? SObject.bestQuality
            : roll < numBestQuality + numHighQuality
                ? SObject.highQuality
                : roll < numBestQuality + numHighQuality + numMedQuality
                    ? SObject.medQuality
                    : SObject.lowQuality;
        qualityRatingByFishPond[thisFishPond] += (int) Math.Pow(16,
            fishlingQuality == SObject.bestQuality ? fishlingQuality - 1 : fishlingQuality);
        ModData.Write(DataField.QualityRatingByFishPond, qualityRatingByFishPond.ToString(",", ";"), owner);
    }

    #endregion harmony patches
}