namespace DaLion.Redux.Framework.Professions.Patches.Integrations;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Redux.Framework.Professions.Extensions;
using DaLion.Shared.Attributes;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using HarmonyPatch = DaLion.Shared.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
[Integration("DIGUS.ANIMALHUSBANDRYMOD")]
internal sealed class InseminationSyringeOverridesDoFunctionPatch : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="InseminationSyringeOverridesDoFunctionPatch"/> class.</summary>
    internal InseminationSyringeOverridesDoFunctionPatch()
    {
        this.Target = "AnimalHusbandryMod.tools.InseminationSyringeOverrides"
            .ToType()
            .RequireMethod("DoFunction");
    }

    #region harmony patches

    /// <summary>Patch to reduce gestation of animals inseminated by Breeder.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? InseminationSyringeOverridesDoFunctionTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new IlHelper(original, instructions);

        // Injected: if (who.professions.Contains(<breeder_id>)) daysUntilBirth /= who.professions.Contains(<breeder_id> + 100) ? 3.0 : 2.0
        // Before: PregnancyController.AddPregnancy(animal, daysUtillBirth);
        try
        {
            var daysUntilBirth = helper.Locals[5];
            var isNotBreeder = generator.DefineLabel();
            var isNotPrestiged = generator.DefineLabel();
            var resumeDivision = generator.DefineLabel();
            helper
                .FindFirst(
                    new CodeInstruction(OpCodes.Ldloc_1),
                    new CodeInstruction(OpCodes.Ldloc_S, daysUntilBirth),
                    new CodeInstruction(OpCodes.Call))
                .StripLabels(out var labels)
                .AddLabels(isNotBreeder)
                .InsertWithLabels(
                    labels,
                    new CodeInstruction(OpCodes.Ldarg_S, (byte)5)) // arg 5 = Farmer who
                .InsertProfessionCheck(Profession.Breeder.Value, forLocalPlayer: false)
                .InsertInstructions(
                    new CodeInstruction(OpCodes.Brfalse_S, isNotBreeder),
                    new CodeInstruction(OpCodes.Ldloc_S, daysUntilBirth),
                    new CodeInstruction(OpCodes.Conv_R8),
                    new CodeInstruction(OpCodes.Ldarg_S, (byte)5))
                .InsertProfessionCheck(Profession.Breeder.Value + 100, forLocalPlayer: false)
                .InsertInstructions(
                    new CodeInstruction(OpCodes.Brfalse_S, isNotPrestiged),
                    new CodeInstruction(OpCodes.Ldc_R8, 3.0),
                    new CodeInstruction(OpCodes.Br_S, resumeDivision))
                .InsertWithLabels(
                    new[] { isNotPrestiged },
                    new CodeInstruction(OpCodes.Ldc_R8, 2.0))
                .InsertWithLabels(
                    new[] { resumeDivision },
                    new CodeInstruction(OpCodes.Div),
                    new CodeInstruction(
                        OpCodes.Call,
                        typeof(Math).RequireMethod(nameof(Math.Round), new[] { typeof(double) })),
                    new CodeInstruction(OpCodes.Conv_I4),
                    new CodeInstruction(OpCodes.Stloc_S, daysUntilBirth));
        }
        catch (Exception ex)
        {
            Log.E("Immersive Professions failed while patching inseminated pregnancy time for Breeder." +
                  "\n—-- Do NOT report this to Animal Husbandry's author. ---" +
                  $"\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}
