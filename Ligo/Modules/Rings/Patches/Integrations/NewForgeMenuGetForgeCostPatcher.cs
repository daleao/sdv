namespace DaLion.Ligo.Modules.Rings.Patches;

#region using directives

using DaLion.Ligo.Modules.Rings.Extensions;
using DaLion.Shared.Attributes;
using HarmonyLib;
using Shared.Harmony;
using SpaceCore.Interface;
using StardewValley.Objects;

#endregion using directives

[UsedImplicitly]
[Integration("spacechase0.SpaceCore")]
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
    private static bool ForgeMenuGetForgeCostPrefix(ref int __result, Item left_item, Item right_item)
    {
        if (!ModEntry.Config.Rings.TheOneInfinityBand || left_item is not Ring left)
        {
            return true; // run original logic
        }

        if (left.ParentSheetIndex == Globals.InfinityBandIndex && right_item is Ring right && right.IsGemRing())
        {
            __result = 10;
            return false; // don't run original logic
        }

        if (left.ParentSheetIndex == Constants.IridiumBandIndex &&
            right_item.ParentSheetIndex == Constants.GalaxySoulIndex)
        {
            __result = 20;
            return false; // don't run original logic
        }

        return true; // run original logic
    }

    #endregion harmony patches
}
