namespace DaLion.Taxes.Framework.Patchers;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Menus;

#endregion using directives

[UsedImplicitly]
internal sealed class ShopMenuTryToPurchaseItemPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="ShopMenuTryToPurchaseItemPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal ShopMenuTryToPurchaseItemPatcher(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
    {
        this.Target = this.RequireMethod<ShopMenu>("tryToPurchaseItem");
    }

    #region harmony patches

    /// <summary>Patch to deduct tool and other expenses.</summary>
    [HarmonyTranspiler]
    [UsedImplicitly]
    private static IEnumerable<CodeInstruction>? ShopMenuTryToPurchaseItemTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);
        try
        {
            helper
                .ForEach(
                    [
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(ShopMenu).RequireMethod(nameof(ShopMenu.chargePlayer))),
                    ],
                    _ =>
                    {
                        var resumeExecution = generator.DefineLabel();
                        helper
                            .Move()
                            .AddLabels(resumeExecution)
                            .Insert(
                                [
                                    new CodeInstruction(OpCodes.Ldarg_0),
                                    new CodeInstruction(OpCodes.Ldarg_1),
                                    new CodeInstruction(
                                        OpCodes.Call,
                                        typeof(ShopMenuTryToPurchaseItemPatcher).RequireMethod(
                                            nameof(TryToPurchaseItemSubroutine))),
                                ]);
                    });
        }
        catch (Exception ex)
        {
            Log.E($"Failed to inject expense deductions.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    private static void TryToPurchaseItemSubroutine(ShopMenu menu, ISalable item)
    {
        var deductible = 0;
        switch (item)
        {
            case Tool:
                deductible = (int)(menu.itemPriceAndStock[item].Price * Config.DeductibleToolExpenses);
                break;
            case SObject @object:
                if (@object.Category == SObject.SeedsCategory)
                {
                    deductible = (int)(menu.itemPriceAndStock[item].Price * Config.DeductibleSeedExpenses);
                }
                else if (@object.ParentSheetIndex is 104 or 178)
                {
                    deductible = (int)(menu.itemPriceAndStock[item].Price * Config.DeductibleAnimalExpenses);
                }
                else if (Config.DeductibleExtras.TryGetValue(@object.Name, out var pct))
                {
                    deductible = (int)(menu.itemPriceAndStock[item].Price * pct);
                }

                break;
        }

        if (deductible <= 0)
        {
            return;
        }

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
