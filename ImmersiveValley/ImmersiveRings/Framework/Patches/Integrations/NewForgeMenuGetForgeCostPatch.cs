namespace DaLion.Stardew.Rings.Framework.Patches;

#region using directives

using DaLion.Common.Attributes;
using DaLion.Common.Extensions.Reflection;
using DaLion.Stardew.Rings.Extensions;
using HarmonyLib;
using StardewValley.Objects;
using HarmonyPatch = DaLion.Common.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
[RequiresMod("spacechase0.SpaceCore")]
internal sealed class NewForgeMenuGetForgeCostPatch : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="NewForgeMenuGetForgeCostPatch"/> class.</summary>
    internal NewForgeMenuGetForgeCostPatch()
    {
        this.Target = "SpaceCore.Interface.NewForgeMenu"
            .ToType()
            .RequireMethod("GetForgeCost");
    }

    #region harmony patches

    /// <summary>Modify forge cost of Infinity Band.</summary>
    [HarmonyPrefix]
    private static bool ForgeMenuGetForgeCostPrefix(ref int __result, Item left_item, Item right_item)
    {
        if (!ModEntry.Config.TheOneIridiumBand ||
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
