namespace DaLion.Ligo.Modules.Tweex.Patches;

#region using directives

using DaLion.Ligo.Modules.Tweex.Extensions;
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

    /// <summary>Ages tapper trees.</summary>
    [HarmonyPostfix]
    private static void TreeDayUpdatePostfix(Tree __instance)
    {
        if (__instance.growthStage.Value >= Tree.treeStage && __instance.CanBeTapped())
        {
            __instance.Increment(DataFields.Age);
        }
    }

    #endregion harmony patches
}
