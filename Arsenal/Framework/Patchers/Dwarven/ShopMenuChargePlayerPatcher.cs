namespace DaLion.Arsenal.Framework.Patchers.Dwarven;

#region using directives

using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using DaLion.Overhaul.Modules.Combat.Integrations;
using DaLion.Shared.Constants;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley;
using StardewValley.Menus;

#endregion using directives

[UsedImplicitly]
internal sealed class ShopMenuChargePlayerPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="ShopMenuChargePlayerPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    internal ShopMenuChargePlayerPatcher(Harmonizer harmonizer)
        : base(harmonizer)
    {
        this.Target = this.RequireMethod<ShopMenu>(nameof(ShopMenu.chargePlayer));
    }

    #region harmony patches

    /// <summary>Set up Clint's forge shop.</summary>
    [HarmonyPrefix]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:Element should begin with upper-case letter", Justification = "Preference for inner functions.")]
    private static bool ShopMenuChargePlayerPrefix(Farmer who, int currencyType, int amount)
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

            var leftover = amount;
            for (var i = 0; i < who.Items.Count; i++)
            {
                var item = who.Items[i];
                if (item.ParentSheetIndex != currencyType)
                {
                    continue;
                }

                if (item.Stack >= leftover)
                {
                    item.Stack -= leftover;
                    if (item.Stack <= 0)
                    {
                        who.Items[i] = null;
                    }

                    break;
                }

                leftover -= item.Stack;
                who.Items[i] = null;
            }

            if (leftover <= 0)
            {
                return false; // don't run original logic
            }

            Log.E(leftover == amount
                ? $"{who} was allowed to spend {amount} {getCurrencyName(currencyType)} but didn't have any."
                : $"{who} was allowed to spend {amount} {getCurrencyName(currencyType)} but only had {amount - leftover}.");
            return false; // don't run original logic
        }
        catch (Exception ex)
        {
            Log.E($"Failed in {MethodBase.GetCurrentMethod()?.Name}:\n{ex}");
            return true; // default to original logic
        }

        string getCurrencyName(int currencyIndex)
        {
            return currencyIndex switch
            {
                ObjectIds.DragonTooth => "Dragon Tooth",
                _ => currencyType == JsonAssetsIntegration.DwarvenScrapIndex!.Value ? "Dwarven Scrap" : "Elderwood",
            };
        }
    }

    #endregion harmony patches
}
