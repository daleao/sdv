namespace DaLion.Stardew.Tweaks.Framework.Patches.Foraging;

#region using directives

using HarmonyLib;
using JetBrains.Annotations;
using StardewValley.TerrainFeatures;

using Extensions;

#endregion using directives

[UsedImplicitly]
internal class TreeDayUpdatePatch : BasePatch
{
    /// <summary>Construct an instance.</summary>
    internal TreeDayUpdatePatch()
    {
        Original = RequireMethod<Tree>(nameof(Tree.dayUpdate));
    }

    #region harmony patches

    /// <summary>Ages tapper trees.</summary>
    [HarmonyPostfix]
    private static void TreeDayUpdatePostfix(Tree __instance, int __state)
    {
        if (__instance.growthStage.Value >= Tree.treeStage && __instance.CanBeTapped() &&
            ModEntry.Config.AgeTapperTrees) __instance.IncrementData<int>("Age");
    }

    #endregion harmony patches
}