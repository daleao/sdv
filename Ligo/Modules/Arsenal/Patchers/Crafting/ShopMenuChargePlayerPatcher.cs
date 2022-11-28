namespace DaLion.Ligo.Modules.Arsenal.Patchers;

#region using directives

using System.Linq;
using System.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley;
using StardewValley.Menus;

#endregion using directives

[UsedImplicitly]
internal sealed class ShopMenuChargePlayerPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="ShopMenuChargePlayerPatcher"/> class.</summary>
    internal ShopMenuChargePlayerPatcher()
    {
        this.Target = this.RequireMethod<ShopMenu>(nameof(ShopMenu.chargePlayer));
    }

    #region harmony patches

    /// <summary>Set up Clint's forge shop.</summary>
    [HarmonyPrefix]
    private static bool ShopMenuChargePlayerPrefix(Farmer who, int currencyType, int amount)
    {
        try
        {
            if (currencyType != Globals.DwarvenScrapIndex && currencyType != Constants.DragonToothIndex)
            {
                return true; // run original logic
            }

            var currencies = who.Items.Where(i => i.ParentSheetIndex == currencyType).ToArray();
            if (currencies.Length == 0)
            {
                var currencyName = currencyType == Constants.DragonToothIndex ? "Dragon Tooth" : "Dwarven Scrap";
                Log.E($"{who} was allowed to spend {amount} {currencyName} but didn't have any.");
                return false; // don't run original logic
            }

            var leftover = amount;
            foreach (var currency in currencies)
            {
                var j = who.Items.IndexOf(currency);
                if (currency.Stack >= leftover)
                {
                    currency.Stack -= leftover;
                    if (currency.Stack <= 0)
                    {
                        who.Items[j] = null;
                    }

                    break;
                }

                leftover -= currency.Stack;
                who.Items[j] = null;
            }

            if (leftover > 0)
            {
                var currencyName = currencyType == Constants.DragonToothIndex ? "Dragon Tooth" : "Dwarven Scrap";
                Log.E($"{who} was allowed to spend {amount} {currencyName} but only had {amount - leftover}.");
                return false; // don't run original logic
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
