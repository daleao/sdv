namespace DaLion.Stardew.Professions.Framework.Patches.Fishing;

#region using directives

using System;
using System.Linq;
using System.Reflection;
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
    private static readonly MethodInfo _CalculateBobberTile = typeof(FishingRod).MethodNamed("calculateBobberTile");

    /// <summary>Construct an instance.</summary>
    internal FishingRodPullFishFromWaterPatch()
    {
        Original = RequireMethod<FishingRod>(nameof(FishingRod.pullFishFromWater));
    }

    #region harmony patches

    /// <summary>Patch to decrement total Fish Pond quality rating.</summary>
    [HarmonyPrefix]
    private static bool FishingRodPullFishFromWaterPrefix(FishingRod __instance, int whichFish, ref int fishQuality, bool fromFishPond)
    {
        if (!ModEntry.Config.EnableFishPondRebalance || !fromFishPond || whichFish.IsTrash()) return true; // run original logic

        var (x, y) = (Vector2) _CalculateBobberTile.Invoke(__instance, null)!;
        var pond = Game1.getFarm().buildings.OfType<FishPond>().FirstOrDefault(p =>
            x > p.tileX.Value && x < p.tileX.Value + p.tilesWide.Value - 1 &&
            y > p.tileY.Value && y < p.tileY.Value + p.tilesHigh.Value - 1);
        if (pond is null) return true; // run original logic

        var qualityRating = pond.ReadDataAs<int>("QualityRating");
        var lowestQuality = pond.GetLowestFishQuality();
        qualityRating -= (int) Math.Pow(16, lowestQuality == SObject.bestQuality ? lowestQuality - 1 : lowestQuality);
        pond.WriteData("QualityRating", qualityRating.ToString());

        fishQuality = lowestQuality;
        return true; // run original logic
    }

    #endregion harmony patches
}