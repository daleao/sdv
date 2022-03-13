namespace DaLion.Stardew.Tweaks.Framework.Patches.Foraging;

#region using directives

using HarmonyLib;
using JetBrains.Annotations;
using StardewValley;
using StardewValley.TerrainFeatures;

#endregion using directives

[UsedImplicitly]
internal class FruitTreeDayUpdatePatch : BasePatch
{
    /// <summary>Construct an instance.</summary>
    internal FruitTreeDayUpdatePatch()
    {
        Original = RequireMethod<FruitTree>(nameof(FruitTree.dayUpdate));
    }

    #region harmony patches

    /// <summary>Negatively compensates winter growth.</summary>
    [HarmonyPostfix]
    private static void FruitTreeDayUpdatePostfix(FruitTree __instance)
    {
        if (__instance.growthStage.Value < FruitTree.treeStage && Game1.IsWinter &&
            ModEntry.Config.PreventFruitTreeGrowthInWinter)
            ++__instance.daysUntilMature.Value;
    }

    #endregion harmony patches
}