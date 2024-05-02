namespace DaLion.Taxes.Framework.Patchers;

#region using directives

using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Menus;

#endregion using directives

[UsedImplicitly]
internal sealed class CarpenterMenuConsumeResourcesPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="CarpenterMenuConsumeResourcesPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    internal CarpenterMenuConsumeResourcesPatcher(Harmonizer harmonizer)
        : base(harmonizer)
    {
        this.Target = this.RequireMethod<CarpenterMenu>(nameof(CarpenterMenu.ConsumeResources));
    }

    #region harmony patches

    /// <summary>Patch to deduct building expenses.</summary>
    [HarmonyPostfix]
    private static void CarpenterMenuConsumeResourcesPostfix(CarpenterMenu __instance)
    {
        var blueprint = __instance.Blueprint;
        if ((blueprint.MagicalConstruction && Config.ExemptMagicalBuildings) ||
            Config.DeductibleBuildingExpenses <= 0f)
        {
            return;
        }

        var deductible = (int)(blueprint.BuildCost * Config.DeductibleBuildingExpenses);
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
