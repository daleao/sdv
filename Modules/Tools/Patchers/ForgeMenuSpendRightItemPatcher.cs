namespace DaLion.Overhaul.Modules.Tools.Patchers;

#region using directives

using DaLion.Shared.Constants;
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

        var item = __instance.rightIngredientSpot.item;
        if (item.ParentSheetIndex is not (ObjectIds.CopperBar or ObjectIds.IronBar or ObjectIds.GoldBar
                or ObjectIds.IridiumBar or ObjectIds.RadioactiveBar) &&
            (item.ParentSheetIndex != 1720 ||
             Reflector.GetUnboundPropertyGetter<object, string>(item, "FullId").Invoke(item) != "spacechase0.MoonMisadventures/Mythicite Bar"))
        {
            return true; // run original logic
        }

        __instance.rightIngredientSpot.item.Stack -= 5;
        if (__instance.rightIngredientSpot.item.Stack <= 0)
        {
            __instance.rightIngredientSpot.item = null;
        }

        return false; // don't run original logic
    }

    #endregion harmony patches
}
