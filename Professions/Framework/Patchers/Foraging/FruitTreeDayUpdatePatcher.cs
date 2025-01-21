namespace DaLion.Professions.Framework.Patchers.Foraging;

#region using directives

using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.TerrainFeatures;

#endregion using directives

[UsedImplicitly]
internal sealed class FruitTreeDayUpdatePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="FruitTreeDayUpdatePatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal FruitTreeDayUpdatePatcher(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
    {
        this.Target = this.RequireMethod<FruitTree>(nameof(FruitTree.dayUpdate));
    }

    #region harmony patches

    /// <summary>Record growth stage.</summary>
    [HarmonyPrefix]
    [UsedImplicitly]
    private static void FruitTreeDayUpdatePrefix(FruitTree __instance, ref (int DaysUntilMature, int GrowthStage) __state)
    {
        __state.DaysUntilMature = __instance.daysUntilMature.Value;
        __state.GrowthStage = __instance.growthStage.Value;
    }

    /// <summary>Patch to increase Arborist fruit tree growth speed.</summary>
    [HarmonyPostfix]
    [HarmonyPriority(Priority.HigherThanNormal)]
    [UsedImplicitly]
    private static void FruitTreeDayUpdatePostfix(FruitTree __instance, (int DaysUntilMature, int GrowthStage) __state)
    {
        if (!Data.ReadAs<bool>(__instance, DataKeys.PlantedByArborist) || __instance.daysUntilMature.Value % 4 != 0)
        {
            return;
        }

        __instance.daysUntilMature.Value--;
        __instance.growthStage.Value = FruitTree.DaysUntilMatureToGrowthStage(__instance.daysUntilMature.Value);
    }

    #endregion harmony patches
}
