namespace DaLion.Overhaul.Modules.Combat.Patchers.Rings;

using DaLion.Overhaul.Modules.Combat.Extensions;

#region using directives

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
        if (!CombatModule.Config.EnableInfinityBand || !Globals.InfinityBandIndex.HasValue || left_item is not Ring left)
        {
            return true; // run original logic
        }

        if (left.ParentSheetIndex == Globals.InfinityBandIndex.Value && right_item is Ring right && right.IsGemRing())
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
