namespace DaLion.Overhaul.Modules.Rings.Patchers.Integration;

#region using directives

using DaLion.Overhaul.Modules.Rings.Extensions;
using DaLion.Shared.Attributes;
using DaLion.Shared.Harmony;
using HarmonyLib;
using SpaceCore.Interface;
using StardewValley.Objects;

#endregion using directives

[UsedImplicitly]
[RequiresMod("spacechase0.SpaceCore")]
internal sealed class NewForgeMenuGetForgeCostPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="NewForgeMenuGetForgeCostPatcher"/> class.</summary>
    internal NewForgeMenuGetForgeCostPatcher()
    {
        this.Target = this.RequireMethod<NewForgeMenu>("GetForgeCost");
    }

    #region harmony patches

    /// <summary>Modify forge cost of Infinity Band.</summary>
    [HarmonyPrefix]
    private static bool NewForgeMenuGetForgeCostPrefix(ref int __result, Item left_item, Item right_item)
    {
        if (!RingsModule.Config.TheOneInfinityBand || left_item is not Ring left)
        {
            return true; // run original logic
        }

        if (left.ParentSheetIndex == Globals.InfinityBandIndex && right_item is Ring right && right.IsGemRing())
        {
            __result = 10;
            return false; // don't run original logic
        }

        if (left.ParentSheetIndex == ItemIDs.IridiumBand &&
            right_item.ParentSheetIndex == ItemIDs.GalaxySoul)
        {
            __result = 20;
            return false; // don't run original logic
        }

        return true; // run original logic
    }

    #endregion harmony patches
}
