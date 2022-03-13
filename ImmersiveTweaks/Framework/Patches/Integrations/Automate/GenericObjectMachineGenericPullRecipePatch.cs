namespace DaLion.Stardew.Tweaks.Framework.Patches.Integrations.Automate;

#region using directives

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;

using Common.Extensions;
using Common.Harmony;

using SObject = StardewValley.Object;

#endregion using directives

[UsedImplicitly]
internal class GenericObjectMachineGenericPullRecipePatch : BasePatch
{
    private static MethodInfo _GetSample;

    /// <summary>Construct an instance.</summary>
    internal GenericObjectMachineGenericPullRecipePatch()
    {
        try
        {
            Original = "Pathoschild.Stardew.Automate.Framework.GenericObjectMachine`1".ToType()
                .MakeGenericType(typeof(SObject))
                .GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.DeclaredOnly)
                .FirstOrDefault(m => m.Name == "GenericPullRecipe" && m.GetParameters().Length == 3);
        }
        catch
        {
            // ignored
        }
    }

    #region harmony patches

    /// <summary>Replaces large egg output quality with quantity + add flower memory to automated kegs.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> GenericObjectMachineGenericPullRecipeTranspiler(
        IEnumerable<CodeInstruction> instructions, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        /// Injected: GenericPullRecipeSubroutine(this, consumable)
        /// Before: return true;

        try
        {
            helper
                .FindFirst(
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Call)
                )
                .GetInstructions(out var got, 2)
                .FindNext(
                    new CodeInstruction(OpCodes.Ldc_I4_1),
                    new CodeInstruction(OpCodes.Ret)
                )
                .Insert(got)
                .Insert(
                    new CodeInstruction(OpCodes.Ldloc_0),
                    new CodeInstruction(OpCodes.Call,
                        typeof(GenericObjectMachineGenericPullRecipePatch).MethodNamed(
                            nameof(GenericPullRecipeSubroutine)))
                );
        }
        catch (Exception ex)
        {
            Log.E($"Failed while patching modded Artisan behavior to generic Automate machines.\nHelper returned {ex}");
            transpilationFailed = true;
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches

    #region private methods

    private static void GenericPullRecipeSubroutine(SObject machine, object consumable)
    {
        if (machine.name != "Mayonnaise Machine" || machine.name != "Keg") return;

        _GetSample ??= consumable.GetType().PropertyGetter("Sample");
        if (_GetSample.Invoke(consumable, null) is not SObject input) return;


        var output = machine.heldObject.Value;
        switch (machine.Name)
        {
            case "Mayonnaise Machine" when ModEntry.Config.LargeProducsYieldQuantityOverQuality:
                if (input.Name.ContainsAnyOf("Large", "L."))
                {
                    output.Stack = 2;
                    output.Quality = SObject.lowQuality;
                }
                else
                {
                    switch (input.ParentSheetIndex)
                    {
                        // ostrich mayonnaise keeps giving x10 output but doesn't respect input quality without Artisan
                        case 289 when !ModEntry.ModHelper.ModRegistry.IsLoaded(
                            "ughitsmegan.ostrichmayoForProducerFrameworkMod"):
                            output.Quality = SObject.lowQuality;
                            break;
                        // golden mayonnaise keeps giving gives single output but keeps golden quality
                        case 928 when !ModEntry.ModHelper.ModRegistry.IsLoaded(
                            "ughitsmegan.goldenmayoForProducerFrameworkMod"):
                            output.Stack = 1;
                            break;
                    }
                }

                break;

            case "Keg" when input.ParentSheetIndex == 340 && input.preservedParentSheetIndex.Value > 0 && ModEntry.Config.KegsRememberHoneyFlower:
                output.name = input.name.Split(" Honey")[0] + " Mead";
                output.preservedParentSheetIndex.Value = input.preservedParentSheetIndex.Value;
                output.Price = input.Price * 2;
                break;
        }
    }

    #endregion private methods
}