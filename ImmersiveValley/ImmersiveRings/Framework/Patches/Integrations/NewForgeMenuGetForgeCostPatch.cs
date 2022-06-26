namespace DaLion.Stardew.Rings.Framework.Patches;

#region using directives

using Common.Extensions.Reflection;
using Extensions;
using HarmonyLib;
using JetBrains.Annotations;
using StardewValley;
using StardewValley.Objects;

#endregion using directives

[UsedImplicitly]
internal sealed class NewForgeMenuGetForgeCostPatch : Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal NewForgeMenuGetForgeCostPatch()
    {
        try
        {
            Target = "SpaceCore.Interface.NewForgeMenu".ToType().RequireMethod("GetForgeCost");
        }
        catch
        {
            // ignored
        }
    }

    #region harmony patches

    /// <summary>Modify forge cost for iridium band.</summary>
    [HarmonyPrefix]
    private static bool ForgeMenuGetForgeCostPrefix(ref int __result, Item left_item, Item right_item)
    {
        if (!ModEntry.Config.TheOneIridiumBand ||
            left_item is not Ring { ParentSheetIndex: Constants.IRIDIUM_BAND_INDEX_I } || right_item is not Ring right ||
            !right.IsGemRing()) return true; // run original logic

        __result = 10;
        return false; // don't run original logic
    }

    #endregion harmony patches
}