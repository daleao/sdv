using DaLion.Common.Extensions.Stardew;

namespace DaLion.Stardew.Tweex.Framework.Patches;

#region using directives

using HarmonyLib;
using StardewValley.TerrainFeatures;

#endregion using directives

[UsedImplicitly]
internal sealed class FruitTreeDayUpdatePatch : Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal FruitTreeDayUpdatePatch()
    {
        Target = RequireMethod<FruitTree>(nameof(FruitTree.dayUpdate));
        Prefix!.before = new[] { "DaLion.ImmersiveProfessions", "atravita.MoreFertilizers" };
        Postfix!.after = new[] { "DaLion.ImmersiveProfessions", "atravita.MoreFertilizers" };
    }

    #region harmony patches

    /// <summary>Record growth stage.</summary>
    [HarmonyPrefix]
    [HarmonyBefore("DaLion.ImmersiveProfessions", "atravita.MoreFertilizers")]
    private static void FruitTreeDayUpdatePrefix(FruitTree __instance, ref (int, int) __state)
    {
        __state.Item1 = __instance.daysUntilMature.Value;
        __state.Item2 = __instance.growthStage.Value;
    }

    /// <summary>Undo growth during winter.</summary>
    [HarmonyPostfix]
    [HarmonyAfter("DaLion.ImmersiveProfessions", "atravita.MoreFertilizers")]
    private static void FruitTreeDayUpdatePostfix(FruitTree __instance, (int, int) __state)
    {
        if (!ModEntry.Config.PreventFruitTreeGrowthInWinter || __instance.growthStage.Value >= FruitTree.treeStage ||
            !Game1.IsWinter || __instance.currentLocation.IsGreenhouse || __instance.Read<int>("atravita.MoreFertilizer.FruitTree") > 0) return;

        __instance.daysUntilMature.Value = __state.Item1;
        __instance.growthStage.Value = __state.Item2;
    }

    #endregion harmony patches
}