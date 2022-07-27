namespace DaLion.Stardew.Tweex.Framework.Patches;

#region using directives

using Common.ModData;
using Extensions;
using HarmonyLib;
using JetBrains.Annotations;
using StardewValley.TerrainFeatures;

#endregion using directives

[UsedImplicitly]
internal sealed class TreeDayUpdatePatch : Common.Harmony.HarmonyPatch
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
            ModEntry.Config.AgeImprovesTreeSap) ModDataIO.Increment<int>(__instance, "Age");
    }

    #endregion harmony patches
}