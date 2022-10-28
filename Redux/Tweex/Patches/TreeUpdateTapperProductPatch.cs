namespace DaLion.Redux.Tweex.Patches;

#region using directives

using DaLion.Redux.Tweex.Extensions;
using HarmonyLib;
using StardewValley.TerrainFeatures;
using HarmonyPatch = DaLion.Shared.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
internal sealed class TreeUpdateTapperProductPatch : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="TreeUpdateTapperProductPatch"/> class.</summary>
    internal TreeUpdateTapperProductPatch()
    {
        this.Target = this.RequireMethod<Tree>(nameof(Tree.UpdateTapperProduct));
    }

    #region harmony patches

    /// <summary>Adds age quality to tapper product.</summary>
    [HarmonyPostfix]
    private static void TreeUpdateTapperProductPostfix(Tree __instance, SObject? tapper_instance)
    {
        if (tapper_instance is not null)
        {
            tapper_instance.heldObject.Value.Quality = __instance.GetQualityFromAge();
        }
    }

    #endregion harmony patches
}
