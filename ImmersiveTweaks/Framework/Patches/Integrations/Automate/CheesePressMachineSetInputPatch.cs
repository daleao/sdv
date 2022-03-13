namespace DaLion.Stardew.Tweaks.Framework.Patches.Integrations.Automate;

#region using directives

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;

using Common.Extensions;
using Common.Harmony;

using SObject = StardewValley.Object;

#endregion using directives

internal class CheesePressMachineSetInput : BasePatch
{
    private static MethodInfo _GetSample;

    /// <summary>Construct an instance.</summary>
    internal CheesePressMachineSetInput()
    {
        try
        {
            Original = "Pathoschild.Stardew.Automate.Framework.Machines.Objects.CheesePressMachine".ToType()
                .MethodNamed("SetInput");
        }
        catch
        {
            // ignored
        }
    }

    #region harmony patches

    /// <summary>Replaces large milk output quality with quantity.</summary>
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
                        typeof(CheesePressMachineSetInput).MethodNamed(
                            nameof(SetInputSubroutine)))
                );
        }
        catch (Exception ex)
        {
            Log.E($"Failed while patching automated Cheese Press.\nHelper returned {ex}");
            transpilationFailed = true;
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches

    #region private methods

    private static void SetInputSubroutine(SObject machine, object consumable)
    {
        if (!ModEntry.Config.LargeProducsYieldQuantityOverQuality) return;

        _GetSample ??= consumable.GetType().PropertyGetter("Sample");
        if (_GetSample.Invoke(consumable, null) is not SObject input) return;

        var output = machine.heldObject.Value;
        if (!input.Name.ContainsAnyOf("Large", "L.")) return;
        
        output.Stack = 2;
        output.Quality = SObject.lowQuality;
    }

    #endregion private methods
}