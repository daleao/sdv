namespace DaLion.Stardew.Tweex.Framework.Patches;

#region using directives

using HarmonyLib;
using JetBrains.Annotations;
using StardewValley.TerrainFeatures;

using Common.Data;
using Common.Harmony;
using Extensions;

#endregion using directives

[UsedImplicitly]
internal sealed class TreeDayUpdatePatch : BasePatch
{
    /// <summary>Construct an instance.</summary>
    internal TreeDayUpdatePatch()
    {
        Target = RequireMethod<Tree>(nameof(Tree.dayUpdate));
    }

    #region harmony patches

    /// <summary>Ages tapper trees.</summary>
    [HarmonyPostfix]
    private static void TreeDayUpdatePostfix(Tree __instance)
    {
        if (__instance.growthStage.Value >= Tree.treeStage && __instance.CanBeTapped() &&
            ModEntry.Config.AgeSapTrees) ModDataIO.IncrementData<int>(__instance, "Age");
    }

    #endregion harmony patches
}