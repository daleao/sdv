namespace DaLion.Overhaul.Modules.Taxes.Patchers;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Shared.Extensions;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Menus;

#endregion using directives

[UsedImplicitly]
internal sealed class ShopMenuTryToPurchaseItemPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="ShopMenuTryToPurchaseItemPatcher"/> class.</summary>
    internal ShopMenuTryToPurchaseItemPatcher()
    {
        this.Target = this.RequireMethod<ShopMenu>("tryToPurchaseItem");
    }

    #region harmony patches

    /// <summary>Patch to deduct tool and other expenses.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? ShopMenuTryToPurchaseItemTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);
        try
        {
            helper
                .ForEach(
                    new[]
                    {
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(ShopMenu).RequireMethod(nameof(ShopMenu.chargePlayer))),
                    },
                    () =>
                    {
                        var resumeExecution = generator.DefineLabel();
                        helper
                            .Move()
                            .AddLabels(resumeExecution)
                            .Insert(
                                new[]
                                {
                                    new CodeInstruction(OpCodes.Ldarg_0),
                                    new CodeInstruction(OpCodes.Ldarg_1),
                                    new CodeInstruction(
                                        OpCodes.Call,
                                        typeof(ShopMenuTryToPurchaseItemPatcher).RequireMethod(
                                            nameof(TryToPurchaseItemSubroutine))),
                                });
                    });
        }
        catch (Exception ex)
        {
            Log.E($"Failed to add Mill quality preservation.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    private static void TryToPurchaseItemSubroutine(ShopMenu menu, ISalable item)
    {
        var isDeductibleToolExpense = item is Tool && TaxesModule.Config.DeductibleToolExpenses;
        if (!isDeductibleToolExpense)
        {
            var isDeductibleSeedExpense = item is SObject { Category: SObject.SeedsCategory } &&
                                          TaxesModule.Config.DeductibleSeedExpenses;
            if (!isDeductibleSeedExpense)
            {
                var isDeductibleAnimalExpense = item is SObject { ParentSheetIndex: 104 or 178 } &&
                                                TaxesModule.Config.DeductibleAnimalExpenses; // hay or heater
                if (!isDeductibleAnimalExpense)
                {
                    var isDeductibleOtherExpense =
                        item is SObject obj && obj.Name.IsIn(TaxesModule.Config.DeductibleObjects);
                    if (!isDeductibleOtherExpense)
                    {
                        return;
                    }
                }
            }
        }

        Game1.player.Increment(DataFields.BusinessExpenses, menu.itemPriceAndStock[item][0]);
    }

    #endregion harmony patches
}
