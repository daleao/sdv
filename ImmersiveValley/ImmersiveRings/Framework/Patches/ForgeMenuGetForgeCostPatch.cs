namespace DaLion.Stardew.Rings.Framework.Patches;

#region using directives

using HarmonyLib;
using JetBrains.Annotations;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Objects;
using Common.Harmony;

using Extensions;

#endregion using directives

[UsedImplicitly]
internal sealed class ForgeMenuGetForgeCostPatch : Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal ForgeMenuGetForgeCostPatch()
    {
        Target = RequireMethod<ForgeMenu>(nameof(ForgeMenu.GetForgeCost));
    }

    #region harmony patches

    /// <summary>Modify forge cost for iridium band.</summary>
    [HarmonyPrefix]
    private static bool ForgeMenuGetForgeCostPrefix(ref int __result, Item left_item, Item right_item)
    {
        if (!ModEntry.Config.TheOneIridiumBand ||
            left_item is not Ring {ParentSheetIndex: Constants.IRIDIUM_BAND_INDEX_I} || right_item is not Ring right ||
            !right.IsGemRing()) return true; // run original logic
        
        __result = 10;
        return false; // don't run original logic
    }

    #endregion harmony patches
}