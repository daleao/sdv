namespace DaLion.Overhaul.Modules.Combat.Patchers.Integration;

#region using directives

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using DaLion.Shared.Attributes;
using DaLion.Shared.Extensions;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
[ModRequirement("FlashShifter.StardewValleyExpandedALL")]
[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1649:File name should match first type name", Justification = "Integration patch specifies the mod in file name but not class to avoid breaking pattern.")]
internal sealed class UtilityGetShopStockPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="UtilityGetShopStockPatcher"/> class.</summary>
    internal UtilityGetShopStockPatcher()
    {
        this.Target = "ShopTileFramework.ItemPriceAndStock.ItemStock".ToType().RequireMethod("Update");
    }

    #region harmony patches

    /// <summary>Prevents Tempered Galaxy weapons from being sold.</summary>
    [HarmonyPostfix]
    private static void UtilityGetShopStockPostfix(object __instance, Dictionary<ISalable, int[]> __result)
    {
        if (!CombatModule.Config.EnableHeroQuest)
        {
            return;
        }

        var shopName = Reflector.GetUnboundFieldGetter<object, string>(__instance, "ShopName").Invoke(__instance);
        if (shopName is not ("AlesiaVendor" or "IsaacVendor"))
        {
            return;
        }

        for (var i = __result.Count - 1; i >= 0; i--)
        {
            var salable = __result.ElementAt(i).Key;
            if (salable is MeleeWeapon or Slingshot && salable.Name.ContainsAnyOf("Galaxy", "Infinity"))
            {
                __result.Remove(salable);
            }
        }
    }

    #endregion harmony patches
}
