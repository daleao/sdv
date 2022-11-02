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
[Integration("Pathoschild.Automate")]
internal sealed class BeeHouseMachineResetPatch : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="BeeHouseMachineResetPatch"/> class.</summary>
    internal BeeHouseMachineResetPatch()
    {
        this.Target = "Pathoschild.Stardew.Automate.Framework.Machines.Objects.BeeHouseMachine"
            .ToType()
            .RequireMethod("Reset");
    }

    #region harmony patches

    /// <summary>Patch to increase production frequency of Producer Bee House.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? ObjectDayUpdateTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new IlHelper(original, instructions);

        // From: machine.MinutesUntilReady = Utility.CalculateMinutesUntilMorning(Game1.timeOfDay, 4);
        // To: machine.MinutesUntilReady = Utility.CalculateMinutesUntilMorning(Game1.timeOfDay, machine.DoesOwnerHaveProfession(<producer_id>)
        //     ? machine.DoesOwnerHaveProfession(100 + <producer_id>
        //         ? 1
        //         : 2
        //     : 4);
        try
        {
            var isNotProducer = generator.DefineLabel();
            var isNotPrestiged = generator.DefineLabel();
            var resumeExecution = generator.DefineLabel();
            helper
                .FindFirst(
                    new CodeInstruction(OpCodes.Ldc_I4_4),
                    new CodeInstruction(
                        OpCodes.Call,
                        typeof(Utility).RequireMethod(
                            nameof(Utility.CalculateMinutesUntilMorning),
                            new[] { typeof(int), typeof(int) })))
                .AddLabels(isNotProducer)
                .InsertInstructions(
                    new CodeInstruction(OpCodes.Ldloc_0), // local 0 = SObject machine
                    new CodeInstruction(OpCodes.Ldc_I4_3), // 3 = Profession.Producer
                    new CodeInstruction(OpCodes.Ldc_I4_0), // false for not prestiged
                    new CodeInstruction(
                        OpCodes.Call,
                        typeof(SObjectExtensions).RequireMethod(
                            nameof(SObjectExtensions.DoesOwnerHaveProfession),
                            new[] { typeof(SObject), typeof(int), typeof(bool) })),
                    new CodeInstruction(OpCodes.Brfalse_S, isNotProducer),
                    new CodeInstruction(OpCodes.Ldloc_0),
                    new CodeInstruction(OpCodes.Ldc_I4_3),
                    new CodeInstruction(OpCodes.Ldc_I4_1), // true for prestiged
                    new CodeInstruction(
                        OpCodes.Call,
                        typeof(SObjectExtensions).RequireMethod(
                            nameof(SObjectExtensions.DoesOwnerHaveProfession),
                            new[] { typeof(SObject), typeof(int), typeof(bool) })),
                    new CodeInstruction(OpCodes.Brfalse_S, isNotPrestiged),
                    new CodeInstruction(OpCodes.Ldc_I4_1),
                    new CodeInstruction(OpCodes.Br_S, resumeExecution),
                    new CodeInstruction(OpCodes.Ldc_I4_2),
                    new CodeInstruction(OpCodes.Br_S, resumeExecution))
                .Retreat(2)
                .AddLabels(isNotPrestiged)
                .Return()
                .Advance()
                .AddLabels(resumeExecution);
        }
        catch (Exception ex)
        {
            Log.E("Immersive Professions failed while patching automated Bee House production speed for Producers." +
                  "\n—-- Do NOT report this to Automate's author. ---" +
                  $"\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}
