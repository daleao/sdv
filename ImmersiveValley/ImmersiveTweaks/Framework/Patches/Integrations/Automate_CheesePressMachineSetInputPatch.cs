namespace DaLion.Stardew.Tweex.Framework.Patches;

#region using directives

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using StardewValley;

using Common;
using Common.Extensions;
using Common.Extensions.Reflection;
using Common.Harmony;

using SObject = StardewValley.Object;

#endregion using directives

[UsedImplicitly]
internal sealed class CheesePressMachineSetInputPatch : BasePatch
{
    private delegate Item GetSampleDelegate(object instance);

    private static GetSampleDelegate _GetSample;

    /// <summary>Construct an instance.</summary>
    internal CheesePressMachineSetInputPatch()
    {
        try
        {
            Target = "Pathoschild.Stardew.Automate.Framework.Machines.Objects.CheesePressMachine".ToType()
                .RequireMethod("SetInput");
        }
        catch
        {
            // ignored
        }
    }

    #region harmony patches

    /// <summary>Replaces large milk output quality with quantity.</summary>
    private static IEnumerable<CodeInstruction> CheesePressMachineSetInputTranspiler(
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
                        typeof(CheesePressMachineSetInputPatch).RequireMethod(nameof(SetInputSubroutine)))
                );
        }
        catch (Exception ex)
        {
            Log.E($"Failed while patching automated Cheese Press.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches

    #region injected subroutines

    private static void SetInputSubroutine(SObject machine, object consumable)
    {
        if (!ModEntry.Config.LargeProducsYieldQuantityOverQuality) return;

        _GetSample ??= consumable.GetType().RequirePropertyGetter("Sample").CreateDelegate<GetSampleDelegate>();
        if (_GetSample(consumable) is not SObject input) return;

        var output = machine.heldObject.Value;
        if (!input.Name.ContainsAnyOf("Large", "L.")) return;

        output.Stack = 2;
        output.Quality = SObject.lowQuality;
    }

    #endregion injected subroutines
}