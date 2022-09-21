namespace DaLion.Stardew.Tweex.Framework.Patches;

#region using directives

using DaLion.Common.Extensions.Stardew;
using DaLion.Stardew.Tweex.Extensions;
using HarmonyLib;
using StardewValley.TerrainFeatures;
using HarmonyPatch = DaLion.Common.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
internal sealed class TreeDayUpdatePatch : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="TreeDayUpdatePatch"/> class.</summary>
    internal TreeDayUpdatePatch()
    {
        this.Target = this.RequireMethod<Tree>(nameof(Tree.dayUpdate));
    }

    #region harmony patches

    /// <summary>Ages tapper trees.</summary>
    [HarmonyPostfix]
    private static void TreeDayUpdatePostfix(Tree __instance)
    {
        if (__instance.growthStage.Value >= Tree.treeStage && __instance.CanBeTapped() &&
            ModEntry.Config.AgeImprovesTreeSap)
        {
            __instance.Increment("Age");
        }
    }

    #endregion harmony patches
}
