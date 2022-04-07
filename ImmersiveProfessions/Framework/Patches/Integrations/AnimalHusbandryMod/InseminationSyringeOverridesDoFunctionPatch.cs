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
using Extensions;

#endregion using directives

[UsedImplicitly]
internal class InseminationSyringeOverridesDoFunctionPatch : BasePatch
{
    /// <summary>Construct an instance.</summary>
    internal InseminationSyringeOverridesDoFunctionPatch()
    {
        try
        {
            Original = "AnimalHusbandryMod.tools.InseminationSyringeOverrides".ToType().RequireMethod("DoFunction");
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

        var daysUntilBirth = helper.Locals[5];
        var isNotBreeder = generator.DefineLabel();
        var isNotPrestiged = generator.DefineLabel();
        var resumeDivision = generator.DefineLabel();
        try
        {
            helper
                .FindFirst(
                    new CodeInstruction(OpCodes.Ldloc_1),
                    new CodeInstruction(OpCodes.Ldloc_S, daysUntilBirth),
                    new CodeInstruction(OpCodes.Call)
                )
                .StripLabels(out var labels)
                .AddLabels(isNotBreeder)
                .InsertWithLabels(
                    labels,
                    new CodeInstruction(OpCodes.Ldarg_S, (byte) 5) // arg 5 = Farmer who
                )
                .InsertProfessionCheck((int) Profession.Breeder, forLocalPlayer: false)
                .Insert(
                    new CodeInstruction(OpCodes.Brfalse_S, isNotBreeder),
                    new CodeInstruction(OpCodes.Ldloc_S, daysUntilBirth),
                    new CodeInstruction(OpCodes.Conv_R8),
                    new CodeInstruction(OpCodes.Ldarg_S, (byte) 5)
                )
                .InsertProfessionCheck((int) Profession.Breeder + 100, forLocalPlayer: false)
                .Insert(
                    new CodeInstruction(OpCodes.Brfalse_S, isNotPrestiged),
                    new CodeInstruction(OpCodes.Ldc_R8, 3.0),
                    new CodeInstruction(OpCodes.Br_S, resumeDivision)
                )
                .InsertWithLabels(
                    new[] {isNotPrestiged},
                    new CodeInstruction(OpCodes.Ldc_R8, 2.0)
                )
                .InsertWithLabels(
                    new[] {resumeDivision},
                    new CodeInstruction(OpCodes.Div),
                    new CodeInstruction(OpCodes.Call,
                        typeof(Math).RequireMethod(nameof(Math.Round), new[] {typeof(double)})),
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