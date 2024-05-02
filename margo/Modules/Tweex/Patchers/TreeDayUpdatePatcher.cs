namespace DaLion.Overhaul.Modules.Tweex.Patchers;

#region using directives

using DaLion.Overhaul.Modules.Tweex.Extensions;
using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.TerrainFeatures;

#endregion using directives

[UsedImplicitly]
internal sealed class TreeDayUpdatePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="TreeDayUpdatePatcher"/> class.</summary>
    internal TreeDayUpdatePatcher()
    {
        this.Target = this.RequireMethod<Tree>(nameof(Tree.dayUpdate));
    }

    #region harmony patches

    /// <summary>Age trees for quality tapper.</summary>
    [HarmonyPostfix]
    private static void TreeDayUpdatePostfix(Tree __instance)
    {
        if (__instance.growthStage.Value >= Tree.treeStage && __instance.CanBeTapped())
        {
            __instance.Increment(DataKeys.Age);
        }
    }

    #endregion harmony patches
}
