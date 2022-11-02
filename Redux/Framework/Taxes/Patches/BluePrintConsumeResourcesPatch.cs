namespace DaLion.Redux.Framework.Taxes.Patches;

#region using directives

using DaLion.Shared.Extensions.Stardew;
using HarmonyLib;
using HarmonyPatch = DaLion.Shared.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
internal sealed class BluePrintConsumeResourcesPatch : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="BluePrintConsumeResourcesPatch"/> class.</summary>
    internal BluePrintConsumeResourcesPatch()
    {
        this.Target = this.RequireMethod<BluePrint>(nameof(BluePrint.consumeResources));
    }

    #region harmony patches

    /// <summary>Patch to deduct building expenses.</summary>
    [HarmonyPostfix]
    private static void BluePrintConsumeResourcesPostfix(BluePrint __instance)
    {
        if (!ModEntry.Config.Taxes.DeductibleBuildingExpenses)
        {
            return;
        }

        Game1.player.Increment(DataFields.BusinessExpenses, __instance.moneyRequired);
    }

    #endregion harmony patches
}
