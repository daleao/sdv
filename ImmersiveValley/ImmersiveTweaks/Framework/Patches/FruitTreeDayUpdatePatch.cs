namespace DaLion.Stardew.Tweex.Framework.Patches;

#region using directives

using HarmonyLib;
using JetBrains.Annotations;
using StardewValley;
using StardewValley.TerrainFeatures;

#endregion using directives

[UsedImplicitly]
internal sealed class FruitTreeDayUpdatePatch : Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal FruitTreeDayUpdatePatch()
    {
        Target = RequireMethod<FruitTree>(nameof(FruitTree.dayUpdate));
    }

    #region harmony patches

    /// <summary>Negatively compensates winter growth.</summary>
    [HarmonyPostfix]
    private static void FruitTreeDayUpdatePostfix(FruitTree __instance)
    {
        if (__instance.growthStage.Value < FruitTree.treeStage && Game1.IsWinter &&
            !__instance.currentLocation.IsGreenhouse && ModEntry.Config.PreventFruitTreeGrowthInWinter)
            ++__instance.daysUntilMature.Value;
    }

    #endregion harmony patches
}