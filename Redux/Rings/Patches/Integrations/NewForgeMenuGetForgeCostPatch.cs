namespace DaLion.Redux.Rings.Patches;

#region using directives

using DaLion.Redux.Rings.Extensions;
using DaLion.Shared.Attributes;
using DaLion.Shared.Extensions.Reflection;
using HarmonyLib;
using StardewValley.Objects;
using HarmonyPatch = DaLion.Shared.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
[Integration("spacechase0.SpaceCore")]
internal sealed class NewForgeMenuGetForgeCostPatch : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="NewForgeMenuGetForgeCostPatch"/> class.</summary>
    internal NewForgeMenuGetForgeCostPatch()
    {
        this.Target = typeof(SpaceCore.Interface.NewForgeMenu)
            .RequireMethod("GetForgeCost");
    }

    #region harmony patches

    /// <summary>Modify forge cost of Infinity Band.</summary>
    [HarmonyPrefix]
    private static bool ForgeMenuGetForgeCostPrefix(ref int __result, Item left_item, Item right_item)
    {
        if (!ModEntry.Config.Rings.TheOneIridiumBand ||
            left_item is not Ring { ParentSheetIndex: Constants.IridiumBandIndex } ||
            right_item is not Ring right ||
            !right.IsGemRing())
        {
            return true; // run original logic
        }

        __result = 10;
        return false; // don't run original logic
    }

    #endregion harmony patches
}
