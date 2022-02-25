namespace DaLion.Stardew.Professions.Framework.Patches.Integrations.AnimalHusbandryMod;

#region using directives

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;

using Stardew.Common.Extensions;
using Stardew.Common.Harmony;

#endregion using directives

[UsedImplicitly]
internal class InseminationSyringeOverridesDoFunctionPatch : BasePatch
{
    /// <summary>Construct an instance.</summary>
    internal InseminationSyringeOverridesDoFunctionPatch()
    {
        try
        {
            Original = "AnimalHusbandryMod.tools.InseminationSyringeOverrides".ToType()
                .MethodNamed("DoFunction");
        }
        catch
        {
            // ignored
        }
    }

    #region harmony patches

    /// <summary>Patch to reduce gestation of animals inseminated by Breeder.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> InseminationSyringeOverridesDoFunctionTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        /// Injected: if (who.professions.Contains(<breeder_id>)) daysUntilBirth /= who.professions.Contains(<breeder_id> + 100) ? 3.0 : 2.0
        /// Before: PregnancyController.AddPregnancy(animal, daysUtillBirth);

        var isNotBreeder = generator.DefineLabel();
        var isNotPrestiged = generator.DefineLabel();
        var resumeDivision = generator.DefineLabel();
        try
        {
            helper
                .FindFirst(
                    new CodeInstruction(OpCodes.Ldloc_S, $"{typeof(int)} (5)")
                )
                .GetOperand(out var daysUntilBirth)
                .FindNext(
                    new CodeInstruction(OpCodes.Ldloc_1),
                    new CodeInstruction(OpCodes.Ldloc_S, daysUntilBirth),
                    new CodeInstruction(OpCodes.Call)
                )
                .StripLabels(out var labels)
                .AddLabels(isNotBreeder)
                .Insert(
                    labels,
                    new CodeInstruction(OpCodes.Ldarg_S, (byte) 5) // arg 5 = Farmer who
                )
                .InsertProfessionCheckForPlayerOnStack((int) Profession.Breeder, isNotBreeder)
                .Insert(
                    new CodeInstruction(OpCodes.Ldloc_S, daysUntilBirth),
                    new CodeInstruction(OpCodes.Conv_R8),
                    new CodeInstruction(OpCodes.Ldarg_S, (byte) 5)
                )
                .InsertProfessionCheckForPlayerOnStack((int) Profession.Breeder + 100, isNotPrestiged)
                .Insert(
                    new CodeInstruction(OpCodes.Ldc_R8, 3.0),
                    new CodeInstruction(OpCodes.Br_S, resumeDivision)
                )
                .Insert(
                    new[] {isNotPrestiged},
                    new CodeInstruction(OpCodes.Ldc_R8, 2.0)
                )
                .Insert(
                    new[] {resumeDivision},
                    new CodeInstruction(OpCodes.Div),
                    new CodeInstruction(OpCodes.Call,
                        typeof(Math).MethodNamed(nameof(Math.Round), new[] {typeof(double)})),
                    new CodeInstruction(OpCodes.Conv_I4),
                    new CodeInstruction(OpCodes.Stloc_S, daysUntilBirth)
                );
        }
        catch (Exception ex)
        {
            Log.E($"Failed while patching inseminated pregnancy time for Breeder.\nHelper returned {ex}");
            transpilationFailed = true;
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}