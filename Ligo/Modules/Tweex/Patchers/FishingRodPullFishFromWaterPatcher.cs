namespace DaLion.Ligo.Modules.Tweex.Patchers;

#region using directives

using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Harmony;
using HarmonyLib;
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

    /// <summary>Adds age quality to tapper product.</summary>
    [HarmonyPrefix]
    private static void FishingRodPullFishFromWaterPrefix(int whichFish, ref int fishQuality)
    {
        if (whichFish.IsLegendaryFishIndex() && TweexModule.Config.LegendaryFishAlwaysBestQuality)
        {
            fishQuality = SObject.bestQuality;
        }
    }

    #endregion harmony patches
}
