namespace DaLion.Stardew.Tweex.Framework.Patches;

#region using directives

using Extensions;
using HarmonyLib;
using JetBrains.Annotations;
using StardewValley.TerrainFeatures;
using SObject = StardewValley.Object;

#endregion using directives

[UsedImplicitly]
internal sealed class TreeUpdateTapperProductPatch : Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal TreeUpdateTapperProductPatch()
    {
        Target = RequireMethod<Tree>(nameof(Tree.UpdateTapperProduct));
    }

    #region harmony patches

    /// <summary>Adds age quality to tapper product.</summary>
    [HarmonyPostfix]
    private static void TreeUpdateTapperProductPostfix(Tree __instance, SObject? tapper_instance)
    {
        if (tapper_instance is not null)
            tapper_instance.heldObject.Value.Quality = __instance.GetQualityFromAge();
    }

    #endregion harmony patches
}