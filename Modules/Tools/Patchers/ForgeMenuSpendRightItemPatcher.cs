namespace DaLion.Overhaul.Modules.Tools.Patchers;

#region using directives

using DaLion.Shared.Extensions;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Menus;

#endregion using directives

[UsedImplicitly]
internal sealed class ForgeMenuSpendRightItemPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="ForgeMenuSpendRightItemPatcher"/> class.</summary>
    internal ForgeMenuSpendRightItemPatcher()
    {
        this.Target = this.RequireMethod<ForgeMenu>(nameof(ForgeMenu.SpendRightItem));
    }

    #region harmony patches

    /// <summary>Allow forge upgrades.</summary>
    [HarmonyPrefix]
    private static bool ForgeMenuSpendRightItemPrefix(ForgeMenu __instance)
    {
        if (!ToolsModule.Config.EnableForgeUpgrading || __instance.rightIngredientSpot.item is null)
        {
            return true; // run original logic
        }

        if (__instance.rightIngredientSpot.item.ParentSheetIndex is not (SObject.copperBar or SObject.ironBar
                or SObject.goldBar or SObject.iridiumBar or ItemIDs.RadioactiveBar) &&
            __instance.rightIngredientSpot.item.ParentSheetIndex !=
            "spacechase0.MoonMisadventures/Mythicite Bar".GetDeterministicHashCode())
        {
            return true; // run original logic
        }

        __instance.rightIngredientSpot.item.Stack -= 10;
        if (__instance.rightIngredientSpot.item.Stack <= 0)
        {
            __instance.rightIngredientSpot.item = null;
        }

        return false; // don't run original logic
    }

    #endregion harmony patches
}
