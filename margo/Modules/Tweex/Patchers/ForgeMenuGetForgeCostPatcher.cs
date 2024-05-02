namespace DaLion.Overhaul.Modules.Tweex.Patchers;

#region using directives

using DaLion.Shared.Constants;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Menus;
using StardewValley.Objects;

#endregion using directives

[UsedImplicitly]
internal sealed class ForgeMenuGetForgeCostPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="ForgeMenuGetForgeCostPatcher"/> class.</summary>
    internal ForgeMenuGetForgeCostPatcher()
    {
        this.Target = this.RequireMethod<ForgeMenu>(nameof(ForgeMenu.GetForgeCost));
    }

    #region harmony patches

    /// <summary>Modify forge cost for Infinity Band.</summary>
    [HarmonyPrefix]
    private static bool ForgeMenuGetForgeCostPrefix(ref int __result, Item left_item, Item right_item)
    {
        if (left_item is not Ring || right_item is not Ring)
        {
            return true; // run original logic
        }

        switch (left_item.ParentSheetIndex)
        {
            case ObjectIds.SmallGlowRing or ObjectIds.SmallMagnetRing when
                right_item.ParentSheetIndex == left_item.ParentSheetIndex:
                __result = 5;
                return false; // don't run original logic
            case ObjectIds.GlowRing when right_item.ParentSheetIndex == ObjectIds.MagnetRing:
            case ObjectIds.MagnetRing when right_item.ParentSheetIndex == ObjectIds.GlowRing:
                __result = 10;
                return false; // don't run original logic
            default:
                return true; // run original logic
        }
    }

    #endregion harmony patches
}
