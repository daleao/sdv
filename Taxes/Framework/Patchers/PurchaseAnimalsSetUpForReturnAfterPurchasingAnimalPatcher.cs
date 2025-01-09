namespace DaLion.Taxes.Framework.Patchers;

#region using directives

using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Menus;

#endregion using directives

[UsedImplicitly]
internal sealed class PurchaseAnimalsSetUpForReturnAfterPurchasingAnimalPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="PurchaseAnimalsSetUpForReturnAfterPurchasingAnimalPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    internal PurchaseAnimalsSetUpForReturnAfterPurchasingAnimalPatcher(Harmonizer harmonizer)
        : base(harmonizer)
    {
        this.Target = this.RequireMethod<PurchaseAnimalsMenu>(nameof(PurchaseAnimalsMenu.setUpForReturnAfterPurchasingAnimal));
    }

    #region harmony patches

    /// <summary>Patch to deduct animal expenses.</summary>
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void PurchaseAnimalsMenuSetUpForReturnAfterPurchasingAnimalPostfix(FarmAnimal ___animalBeingPurchased, int ___priceOfAnimal)
    {
        if (Config.DeductibleAnimalExpenses <= 0f)
        {
            return;
        }

        var deductible = (int)(___priceOfAnimal * Config.DeductibleAnimalExpenses);
        if (Game1.player.ShouldPayTaxes())
        {
            Data.Increment(Game1.player, DataKeys.BusinessExpenses, deductible);
        }
        else
        {
            Broadcaster.MessageHost(deductible.ToString(), DataKeys.BusinessExpenses);
        }
    }

    #endregion harmony patches
}
