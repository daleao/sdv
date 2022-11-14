namespace DaLion.Ligo.Modules.Tweex.Patches;

#region using directives

using DaLion.Ligo.Modules.Tweex.Extensions;
using HarmonyLib;
using Shared.Harmony;
using StardewValley.TerrainFeatures;

#endregion using directives

[UsedImplicitly]
internal sealed class TreeUpdateTapperProductPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="TreeUpdateTapperProductPatcher"/> class.</summary>
    internal TreeUpdateTapperProductPatcher()
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
