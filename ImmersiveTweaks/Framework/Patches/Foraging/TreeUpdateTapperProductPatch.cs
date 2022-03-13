namespace DaLion.Stardew.Tweaks.Framework.Patches.Foraging;

#region using directives

using HarmonyLib;
using JetBrains.Annotations;
using StardewValley.TerrainFeatures;

using Extensions;

using SObject = StardewValley.Object;

#endregion using directives

[UsedImplicitly]
internal class TreeUpdateTapperProductPatch : BasePatch
{
    /// <summary>Construct an instance.</summary>
    internal TreeUpdateTapperProductPatch()
    {
        Original = RequireMethod<Tree>(nameof(Tree.UpdateTapperProduct));
    }

    #region harmony patches

    /// <summary>Adds age quality to tapper product.</summary>
    [HarmonyPostfix]
    private static void TreeUpdateTapperProductPostfix(Tree __instance, SObject tapper_instance)
    {
        if (tapper_instance is not null)
            tapper_instance.heldObject.Value.Quality = __instance.GetQualityFromAge();
    }

    #endregion harmony patches
}