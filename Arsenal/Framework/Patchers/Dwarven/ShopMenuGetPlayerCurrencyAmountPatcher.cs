namespace DaLion.Arsenal.Framework.Patchers.Dwarven;

#region using directives

using System.Reflection;
using DaLion.Arsenal.Framework.Integrations;
using DaLion.Shared.Constants;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley;
using StardewValley.Menus;

#endregion using directives

[UsedImplicitly]
internal sealed class ShopMenuGetPlayerCurrencyAmountPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="ShopMenuGetPlayerCurrencyAmountPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    internal ShopMenuGetPlayerCurrencyAmountPatcher(Harmonizer harmonizer)
        : base(harmonizer)
    {
        this.Target = this.RequireMethod<ShopMenu>(nameof(ShopMenu.getPlayerCurrencyAmount));
    }

    #region harmony patches

    /// <summary>Set up Clint's forge shop.</summary>
    [HarmonyPrefix]
    private static bool ShopMenuGetPlayerCurrencyAmountPrefix(ref int __result, Farmer who, int currencyType)
    {
        try
        {
            if (currencyType != ObjectIds.DragonTooth &&
                (!JsonAssetsIntegration.DwarvenScrapIndex.HasValue ||
                 currencyType != JsonAssetsIntegration.DwarvenScrapIndex.Value) &&
                (!JsonAssetsIntegration.ElderwoodIndex.HasValue ||
                 currencyType != JsonAssetsIntegration.ElderwoodIndex.Value))
            {
                return true; // run original logic
            }

            __result = 0;
            for (var i = 0; i < who.Items.Count; i++)
            {
                var item = who.Items[i];
                if (item.ParentSheetIndex != currencyType)
                {
                    continue;
                }

                __result += item.Stack;
            }

            return false; // don't run original logic
        }
        catch (Exception ex)
        {
            Log.E($"Failed in {MethodBase.GetCurrentMethod()?.Name}:\n{ex}");
            return true; // default to original logic
        }
    }

    #endregion harmony patches
}
