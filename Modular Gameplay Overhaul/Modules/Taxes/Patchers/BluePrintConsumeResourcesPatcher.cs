namespace DaLion.Overhaul.Modules.Taxes.Patchers;

#region using directives

using DaLion.Overhaul.Modules.Taxes.Extensions;
using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Harmony;
using HarmonyLib;

#endregion using directives

[UsedImplicitly]
internal sealed class BluePrintConsumeResourcesPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="BluePrintConsumeResourcesPatcher"/> class.</summary>
    internal BluePrintConsumeResourcesPatcher()
    {
        this.Target = this.RequireMethod<BluePrint>(nameof(BluePrint.consumeResources));
    }

    #region harmony patches

    /// <summary>Patch to deduct building expenses.</summary>
    [HarmonyPostfix]
    private static void BluePrintConsumeResourcesPostfix(BluePrint __instance)
    {
        if ((__instance.magical && TaxesModule.Config.ExemptMagicalBuilding) ||
            !TaxesModule.Config.DeductibleBuildingExpenses)
        {
            return;
        }

        if (Game1.player.ShouldPayTaxes())
        {
            Game1.player.Increment(DataKeys.BusinessExpenses, __instance.moneyRequired);
        }
        else
        {
            Broadcaster.MessageHost(
                __instance.moneyRequired.ToString(),
                OverhaulModule.Taxes.Namespace + DataKeys.BusinessExpenses);
        }
    }

    #endregion harmony patches
}
