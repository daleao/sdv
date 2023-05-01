namespace DaLion.Overhaul.Modules.Taxes.Patchers;

#region using directives

using DaLion.Overhaul.Modules.Taxes.Extensions;
using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Menus;

#endregion using directives

[UsedImplicitly]
internal sealed class PurchaseAnimalsSetUpForReturnAfterPurchasingAnimalPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="PurchaseAnimalsSetUpForReturnAfterPurchasingAnimalPatcher"/> class.</summary>
    internal PurchaseAnimalsSetUpForReturnAfterPurchasingAnimalPatcher()
    {
        this.Target = this.RequireMethod<PurchaseAnimalsMenu>(nameof(PurchaseAnimalsMenu.setUpForReturnAfterPurchasingAnimal));
    }

    #region harmony patches

    /// <summary>Patch to deduct animal expenses.</summary>
    [HarmonyPostfix]
    private static void PurchaseAnimalsMenuReceiveLeftClickPostfix(FarmAnimal ___animalBeingPurchased, int ___priceOfAnimal)
    {
        if (!TaxesModule.Config.DeductibleAnimalExpenses)
        {
            return;
        }

        if (Game1.player.ShouldPayTaxes())
        {
            Game1.player.Increment(DataKeys.BusinessExpenses, ___priceOfAnimal);
        }
        else
        {
            Broadcaster.MessageHost(
                ___priceOfAnimal.ToString(),
                OverhaulModule.Taxes.Namespace + DataKeys.BusinessExpenses);
        }
    }

    #endregion harmony patches
}
