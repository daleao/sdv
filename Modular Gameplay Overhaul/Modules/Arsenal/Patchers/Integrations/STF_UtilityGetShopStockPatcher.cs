namespace DaLion.Overhaul.Modules.Arsenal.Patchers.Integrations;

#region using directives

using System.Collections.Generic;
using System.Linq;
using DaLion.Shared.Attributes;
using DaLion.Shared.Extensions.Collections;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;

#endregion using directives

[UsedImplicitly]
[RequiresMod("FlashShifter.StardewValleyExpandedALL")]
internal sealed class STF_UtilityGetShopStockPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="STF_UtilityGetShopStockPatcher"/> class.</summary>
    internal STF_UtilityGetShopStockPatcher()
    {
        this.Target = "ShopTileFramework.ItemPriceAndStock.ItemStock".ToType().RequireMethod("Update");
    }

    #region harmony patches

    /// <summary>Prevents Tempered Galaxy weapons from being sold.</summary>
    [HarmonyPostfix]
    private static void UtilityGetShopStockPostfix(object __instance, Dictionary<ISalable, int[]> __result)
    {
        if (!ArsenalModule.Config.InfinityPlusOne)
        {
            return;
        }

        var shopName = Reflector.GetUnboundFieldGetter<object, string>(__instance, "ShopName").Invoke(__instance);
        if (shopName is not ("AlesiaVendor" or "IsaacVendor"))
        {
            return;
        }

        var toRemove = __result.Keys.Where(key => key.Name.Contains("Tempered Galaxy")).ToArray();
        if (toRemove.Length == 0)
        {
            return;
        }

        toRemove.ForEach(key => __result.Remove(key));
    }

    #endregion harmony patches
}
