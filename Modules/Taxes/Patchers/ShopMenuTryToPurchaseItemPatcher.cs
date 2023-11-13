namespace DaLion.Overhaul.Modules.Taxes.Patchers;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Overhaul.Modules.Taxes.Extensions;
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
                    _ =>
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
        var deductible = 0;
        switch (item)
        {
            case Tool:
                deductible = (int)(menu.itemPriceAndStock[item][0] * TaxesModule.Config.DeductibleToolExpenses);
                break;
            case SObject @object:
                if (@object.Category == SObject.SeedsCategory)
                {
                    deductible = (int)(menu.itemPriceAndStock[item][0] * TaxesModule.Config.DeductibleSeedExpenses);
                }
                else if (@object.ParentSheetIndex is 104 or 178)
                {
                    deductible = (int)(menu.itemPriceAndStock[item][0] * TaxesModule.Config.DeductibleAnimalExpenses);
                }
                else if (TaxesModule.Config.DeductibleExtras.TryGetValue(@object.Name, out var pct))
                {
                    deductible = (int)(menu.itemPriceAndStock[item][0] * pct);
                }

                break;
        }

        if (deductible <= 0)
        {
            return;
        }

        if (Game1.player.ShouldPayTaxes())
        {
            Game1.player.Increment(DataKeys.BusinessExpenses, deductible);
        }
        else
        {
            Broadcaster.MessageHost(deductible.ToString(), OverhaulModule.Taxes.Namespace + '/' + DataKeys.BusinessExpenses);
        }
    }

    #endregion harmony patches
}
