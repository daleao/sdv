namespace DaLion.Stardew.Professions.Framework.Patches.Fishing;

#region using directives

using System;
using System.Linq;
using HarmonyLib;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Tools;

using Stardew.Common.Extensions;
using Extensions;

using SObject = StardewValley.Object;

#endregion using directives

[UsedImplicitly]
internal class FishingRodPullFishFromWaterPatch : BasePatch
{
    /// <summary>Construct an instance.</summary>
    internal FishingRodPullFishFromWaterPatch()
    {
        Original = RequireMethod<FishingRod>(nameof(FishingRod.pullFishFromWater));
    }

    #region harmony patches

    /// <summary>Patch to decrement total Fish Pond quality rating.</summary>
    [HarmonyPrefix]
    private static bool FishingRodPullFishFromWaterPrefix(FishingRod __instance, ref int fishQuality, bool fromFishPond)
    {
        if (!ModEntry.Config.EnableFishPondRebalance || !fromFishPond) return true; // run original logic

        var who = __instance.getLastFarmerToUse();
        var (x, y) = ModEntry.ModHelper.Reflection.GetMethod(__instance, "calculateBobberTile").Invoke<Vector2>();
        var pond = Game1.getFarm().buildings.OfType<FishPond>().FirstOrDefault(p =>
            x > p.tileX.Value && x < p.tileX.Value + p.tilesWide.Value - 1 &&
            y > p.tileY.Value && y < p.tileY.Value + p.tilesHigh.Value - 1);
        if (pond is null) return true; // run original logic

        var qualityRatingByFishPond =
            ModData.Read(DataField.QualityRatingByFishPond, who).ToDictionary<int, int>(",", ";");
        var thisFishPond = pond.GetCenterTile().ToString().GetDeterministicHashCode();
        var lowestQuality = pond.GetLowestFishQuality();
        qualityRatingByFishPond.TryGetValue(thisFishPond, out var currentRating);
        qualityRatingByFishPond[thisFishPond] = currentRating -
                                                (int) Math.Pow(16,
                                                    lowestQuality == SObject.bestQuality
                                                        ? lowestQuality - 1
                                                        : lowestQuality);
        ModData.Write(DataField.QualityRatingByFishPond, qualityRatingByFishPond.ToString(",", ";"), who);

        fishQuality = lowestQuality;
        return true; // run original logic
    }

    #endregion harmony patches
}